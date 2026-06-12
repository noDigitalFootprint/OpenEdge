using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace OpenEdge;

public sealed class CompatibilityStateService
{
	private readonly object gate = new object();

	private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
	{
		WriteIndented = true
	};

	private CompatibilityStateStore state;

	private readonly string stateFilePath = RuntimePaths.CompatibilityStateFile;

	private readonly string sourcesFilePath = Path.Combine(RuntimePaths.RuntimeRoot, "media-sources.json");

	private readonly string identityFilePath = Path.Combine(RuntimePaths.RuntimeRoot, "media-tag-index.json");

	public void EnsureInitialized()
	{
		lock (gate)
		{
			Directory.CreateDirectory(RuntimePaths.FlagsDir);
			Directory.CreateDirectory(RuntimePaths.TempFlagsDir);
			Directory.CreateDirectory(RuntimePaths.CompatibilityBackupsDir);
			Directory.CreateDirectory(RuntimePaths.CompatibilityTransfersDir);
			state = LoadStateFromDisk();
			if (state == null)
			{
				state = BuildStateFromLegacyFiles();
				SaveStateToDisk();
			}
			else
			{
				SyncMissingLegacyEntriesIntoState();
			}
		}
	}

	public CompatibilityStateSummary GetSummary()
	{
		lock (gate)
		{
			EnsureInitialized();
			return new CompatibilityStateSummary
			{
				PersistentEntryCount = state.PersistentEntries.Count,
				LegacyFlagFileCount = GetLegacyFlagFilePaths().Count,
				StateFileExists = File.Exists(stateFilePath),
				LastLegacyImportUtc = state.LastLegacyImportUtc
			};
		}
	}

	public void MigrateCurrentRuntimeState(bool createBackup)
	{
		lock (gate)
		{
			EnsureInitialized();
			if (createBackup)
			{
				CreateBackupSnapshot();
			}
			state = BuildStateFromLegacyFiles();
			SaveStateToDisk();
		}
	}

	public void ExportTransferPackage(string filePath, bool createBackup)
	{
		lock (gate)
		{
			EnsureInitialized();
			if (createBackup)
			{
				CreateBackupSnapshot();
			}
			CompatibilityTransferPackage compatibilityTransferPackage = new CompatibilityTransferPackage
			{
				PersistentEntries = new Dictionary<string, string>(state.PersistentEntries, StringComparer.OrdinalIgnoreCase),
				OptionsContent = ReadOptionalText(RuntimePaths.OptionsFile),
				TaskLines = ReadOptionalLines(RuntimePaths.TasksFile),
				LegacyTagLines = ReadOptionalLines(RuntimePaths.TagsFile),
				MediaSources = ReadOptionalJson<List<MediaSourceDefinition>>(sourcesFilePath) ?? new List<MediaSourceDefinition>(),
				MediaIdentityStore = ReadOptionalJson<MediaIdentityStore>(identityFilePath) ?? new MediaIdentityStore()
			};
			File.WriteAllText(filePath, JsonSerializer.Serialize(compatibilityTransferPackage, jsonOptions));
		}
	}

	public void ImportTransferPackage(string filePath, bool createBackup)
	{
		lock (gate)
		{
			EnsureInitialized();
			if (createBackup)
			{
				CreateBackupSnapshot();
			}
			CompatibilityTransferPackage compatibilityTransferPackage = JsonSerializer.Deserialize<CompatibilityTransferPackage>(File.ReadAllText(filePath));
			if (compatibilityTransferPackage == null)
			{
				throw new InvalidOperationException("Migration package was empty or invalid.");
			}
			state = new CompatibilityStateStore
			{
				SchemaVersion = compatibilityTransferPackage.SchemaVersion,
				CreatedAtUtc = DateTime.UtcNow,
				LastLegacyImportUtc = DateTime.UtcNow,
				PersistentEntries = new Dictionary<string, string>(compatibilityTransferPackage.PersistentEntries ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase)
			};
			SaveStateToDisk();
			MirrorPersistentEntriesToLegacyFiles();
			WriteOptionalText(RuntimePaths.OptionsFile, compatibilityTransferPackage.OptionsContent);
			WriteOptionalLines(RuntimePaths.TasksFile, compatibilityTransferPackage.TaskLines);
			WriteOptionalLines(RuntimePaths.TagsFile, compatibilityTransferPackage.LegacyTagLines);
			WriteOptionalJson(sourcesFilePath, compatibilityTransferPackage.MediaSources);
			WriteOptionalJson(identityFilePath, compatibilityTransferPackage.MediaIdentityStore);
		}
	}

	public bool PersistentEntryExists(string name)
	{
		lock (gate)
		{
			EnsureInitialized();
			if (state.PersistentEntries.ContainsKey(name))
			{
				return true;
			}
			if (TryReadLegacyFlagValue(name, out string value))
			{
				state.PersistentEntries[name] = value;
				SaveStateToDisk();
				return true;
			}
			return false;
		}
	}

	public string GetPersistentValue(string name)
	{
		lock (gate)
		{
			EnsureInitialized();
			if (state.PersistentEntries.TryGetValue(name, out string value))
			{
				return value;
			}
			if (TryReadLegacyFlagValue(name, out value))
			{
				state.PersistentEntries[name] = value;
				SaveStateToDisk();
				return value;
			}
			return null;
		}
	}

	public void SetPersistentValue(string name, string value)
	{
		lock (gate)
		{
			EnsureInitialized();
			state.PersistentEntries[name] = value ?? "";
			SaveStateToDisk();
			File.WriteAllText(RuntimePaths.Flag(name), value ?? "");
		}
	}

	public void DeletePersistentValue(string name)
	{
		lock (gate)
		{
			EnsureInitialized();
			state.PersistentEntries.Remove(name);
			SaveStateToDisk();
			string text = RuntimePaths.Flag(name);
			if (File.Exists(text))
			{
				File.Delete(text);
			}
		}
	}

	private CompatibilityStateStore LoadStateFromDisk()
	{
		if (!File.Exists(stateFilePath))
		{
			return null;
		}
		return JsonSerializer.Deserialize<CompatibilityStateStore>(File.ReadAllText(stateFilePath));
	}

	private CompatibilityStateStore BuildStateFromLegacyFiles()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (string legacyFlagFilePath in GetLegacyFlagFilePaths())
		{
			dictionary[Path.GetFileNameWithoutExtension(legacyFlagFilePath)] = File.ReadAllText(legacyFlagFilePath);
		}
		return new CompatibilityStateStore
		{
			PersistentEntries = dictionary,
			LastLegacyImportUtc = DateTime.UtcNow
		};
	}

	private void SyncMissingLegacyEntriesIntoState()
	{
		bool flag = false;
		foreach (string legacyFlagFilePath in GetLegacyFlagFilePaths())
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(legacyFlagFilePath);
			if (!state.PersistentEntries.ContainsKey(fileNameWithoutExtension))
			{
				state.PersistentEntries[fileNameWithoutExtension] = File.ReadAllText(legacyFlagFilePath);
				flag = true;
			}
		}
		if (flag)
		{
			state.LastLegacyImportUtc = DateTime.UtcNow;
			SaveStateToDisk();
		}
	}

	private void SaveStateToDisk()
	{
		File.WriteAllText(stateFilePath, JsonSerializer.Serialize(state, jsonOptions));
	}

	private void CreateBackupSnapshot()
	{
		string text = Path.Combine(RuntimePaths.CompatibilityBackupsDir, DateTime.Now.ToString("yyyyMMdd-HHmmss"));
		Directory.CreateDirectory(text);
		CopyOptionalFile(RuntimePaths.OptionsFile, text);
		CopyOptionalFile(RuntimePaths.TasksFile, text);
		CopyOptionalFile(RuntimePaths.TagsFile, text);
		CopyOptionalFile(RuntimePaths.CompatibilityStateFile, text);
		CopyOptionalFile(sourcesFilePath, text);
		CopyOptionalFile(identityFilePath, text);
		CopyDirectory(RuntimePaths.FlagsDir, Path.Combine(text, "flags"));
	}

	private void MirrorPersistentEntriesToLegacyFiles()
	{
		foreach (string legacyFlagFilePath in GetLegacyFlagFilePaths())
		{
			File.Delete(legacyFlagFilePath);
		}
		foreach (KeyValuePair<string, string> persistentEntry in state.PersistentEntries)
		{
			File.WriteAllText(RuntimePaths.Flag(persistentEntry.Key), persistentEntry.Value ?? "");
		}
	}

	private static List<string> GetLegacyFlagFilePaths()
	{
		if (!Directory.Exists(RuntimePaths.FlagsDir))
		{
			return new List<string>();
		}
		return Directory.GetFiles(RuntimePaths.FlagsDir, "*.txt", SearchOption.TopDirectoryOnly).ToList();
	}

	private static bool TryReadLegacyFlagValue(string name, out string value)
	{
		string text = RuntimePaths.Flag(name);
		if (File.Exists(text))
		{
			value = File.ReadAllText(text);
			return true;
		}
		value = null;
		return false;
	}

	private static string ReadOptionalText(string path)
	{
		if (File.Exists(path))
		{
			return File.ReadAllText(path);
		}
		return "";
	}

	private static string[] ReadOptionalLines(string path)
	{
		if (File.Exists(path))
		{
			return File.ReadAllLines(path);
		}
		return Array.Empty<string>();
	}

	private T ReadOptionalJson<T>(string path) where T : class
	{
		if (!File.Exists(path))
		{
			return null;
		}
		return JsonSerializer.Deserialize<T>(File.ReadAllText(path));
	}

	private void WriteOptionalJson<T>(string path, T value) where T : class
	{
		if (value == null)
		{
			return;
		}
		File.WriteAllText(path, JsonSerializer.Serialize(value, jsonOptions));
	}

	private static void WriteOptionalText(string path, string value)
	{
		File.WriteAllText(path, value ?? "");
	}

	private static void WriteOptionalLines(string path, IReadOnlyList<string> lines)
	{
		if (lines == null)
		{
			return;
		}
		File.WriteAllLines(path, lines);
	}

	private static void CopyOptionalFile(string sourcePath, string destinationDirectory)
	{
		if (!File.Exists(sourcePath))
		{
			return;
		}
		File.Copy(sourcePath, Path.Combine(destinationDirectory, Path.GetFileName(sourcePath)), overwrite: true);
	}

	private static void CopyDirectory(string sourceDirectory, string destinationDirectory)
	{
		if (!Directory.Exists(sourceDirectory))
		{
			return;
		}
		Directory.CreateDirectory(destinationDirectory);
		foreach (string file in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
		{
			string relativePath = Path.GetRelativePath(sourceDirectory, file);
			string directoryName = Path.GetDirectoryName(Path.Combine(destinationDirectory, relativePath));
			if (!string.IsNullOrWhiteSpace(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.Copy(file, Path.Combine(destinationDirectory, relativePath), overwrite: true);
		}
	}
}
