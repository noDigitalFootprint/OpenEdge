using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace OpenEdge;

public static class ModService
{
	private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true,
		WriteIndented = true
	};

	public static IReadOnlyList<LoadedMod> LoadEnabledMods()
	{
		return LoadAllMods().Where((LoadedMod mod) => mod.Manifest.Enabled).ToList();
	}

	public static IReadOnlyList<LoadedMod> LoadAllMods()
	{
		EnsureModsDirectory();
		List<LoadedMod> loadedMods = new List<LoadedMod>();
		foreach (string modDirectory in Directory.GetDirectories(RuntimePaths.ModsDir).OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
		{
			LoadedMod loadedMod = TryLoadMod(modDirectory, out _);
			if (loadedMod != null)
			{
				loadedMods.Add(loadedMod);
			}
		}
		return loadedMods;
	}

	public static IReadOnlyList<ModSummary> GetModSummaries()
	{
		EnsureModsDirectory();
		List<ModSummary> summaries = new List<ModSummary>();
		List<LoadedMod> loadedMods = new List<LoadedMod>();
		foreach (string modDirectory in Directory.GetDirectories(RuntimePaths.ModsDir).OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
		{
			LoadedMod loadedMod = TryLoadMod(modDirectory, out string error);
			if (loadedMod == null)
			{
				summaries.Add(CreateErrorSummary(modDirectory, error));
				continue;
			}
			loadedMods.Add(loadedMod);
			summaries.Add(new ModSummary
			{
				RootPath = modDirectory,
				Id = loadedMod.Manifest.Id,
				Name = loadedMod.Manifest.Name,
				Author = loadedMod.Manifest.Author,
				Version = loadedMod.Manifest.Version,
				Enabled = loadedMod.Manifest.Enabled,
				SettingCount = loadedMod.Settings.Count,
				TagCount = loadedMod.Tags.Count,
				ContextCount = loadedMod.Contexts.Count,
				HasLines = IsUsableLineRoot(loadedMod.LinesRootPath),
				Error = ValidateLoadedMod(loadedMod)
			});
		}
		AppendGlobalWarnings(summaries, loadedMods.Where((LoadedMod mod) => mod.Manifest.Enabled).ToList());
		return summaries;
	}

	public static IReadOnlyList<string> GetEnabledLineRoots()
	{
		return LoadEnabledMods().Select((LoadedMod mod) => mod.LinesRootPath).Where(IsUsableLineRoot).ToList();
	}

	public static IReadOnlyList<SettingDefinition> GetEnabledSettingDefinitions()
	{
		return LoadEnabledMods().SelectMany((LoadedMod mod) => mod.Settings).ToList();
	}

	public static IReadOnlyList<ModTagDefinition> GetEnabledTagDefinitions()
	{
		return LoadEnabledMods().SelectMany((LoadedMod mod) => mod.Tags).ToList();
	}

	public static IReadOnlyList<DerivedContextDefinition> GetEnabledContextDefinitions()
	{
		return LoadEnabledMods().SelectMany((LoadedMod mod) => mod.Contexts).ToList();
	}

	public static void EnsureModsDirectory()
	{
		Directory.CreateDirectory(RuntimePaths.ModsDir);
	}

	public static void SetModEnabled(string modRootPath, bool enabled)
	{
		Directory.CreateDirectory(modRootPath);
		ModManifest manifest = LoadManifest(modRootPath);
		if (string.IsNullOrWhiteSpace(manifest.Id))
		{
			manifest.Id = Path.GetFileName(modRootPath);
		}
		if (string.IsNullOrWhiteSpace(manifest.Name))
		{
			manifest.Name = manifest.Id;
		}
		manifest.Enabled = enabled;
		SaveManifest(modRootPath, manifest);
	}

	public static string CreateExampleMod()
	{
		EnsureModsDirectory();
		string modRoot = Path.Combine(RuntimePaths.ModsDir, "example-mod");
		Directory.CreateDirectory(Path.Combine(modRoot, "settings"));
		Directory.CreateDirectory(Path.Combine(modRoot, "tags"));
		Directory.CreateDirectory(Path.Combine(modRoot, "contexts"));
		Directory.CreateDirectory(Path.Combine(modRoot, "lines", "Scripts", "Extend"));
		Directory.CreateDirectory(Path.Combine(modRoot, "lines", "Vocab", "Extend"));
		SaveManifest(modRoot, new ModManifest
		{
			Id = "example-mod",
			Name = "Example Mod",
			Author = "OpenEdge",
			Version = "1.0.0",
			Enabled = false
		});
		WriteIfMissing(Path.Combine(modRoot, "settings", "settings.json"), "{\r\n  \"settings\": [\r\n    {\r\n      \"key\": \"latex\",\r\n      \"label\": \"Latex\",\r\n      \"kind\": \"Toggle\",\r\n      \"legacyEnabledFlag\": \"latex\",\r\n      \"legacyDisabledFlag\": \"latexNo\",\r\n      \"queueableAsk\": true,\r\n      \"mediaDiscoveryTags\": [\"Latex\"],\r\n      \"mediaDiscoveryMinimum\": 2\r\n    }\r\n  ]\r\n}\r\n");
		WriteIfMissing(Path.Combine(modRoot, "tags", "tags.json"), "{\r\n  \"tags\": [\r\n    {\r\n      \"key\": \"latex\",\r\n      \"label\": \"Latex\",\r\n      \"group\": \"Fetishes\"\r\n    }\r\n  ]\r\n}\r\n");
		WriteIfMissing(Path.Combine(modRoot, "contexts", "contexts.json"), "{\r\n  \"contexts\": [\r\n    {\r\n      \"key\": \"latexBondage\",\r\n      \"label\": \"Latex Bondage\",\r\n      \"settings\": [\"latex\"],\r\n      \"mediaTags\": [\"Latex\", \"Bondage\"],\r\n      \"minimumMedia\": 2\r\n    }\r\n  ]\r\n}\r\n");
		WriteIfMissing(Path.Combine(modRoot, "lines", "Vocab", "Extend", "tags.txt"), "Latex\r\n");
		WriteIfMissing(Path.Combine(modRoot, "lines", "Scripts", "Extend", "example.txt"), "This is an example mod line.\r\n");
		return modRoot;
	}

	private static LoadedMod TryLoadMod(string modDirectory, out string error)
	{
		error = "";
		try
		{
			ModManifest manifest = LoadManifest(modDirectory);
			if (string.IsNullOrWhiteSpace(manifest.Id))
			{
				manifest.Id = Path.GetFileName(modDirectory);
			}
			if (string.IsNullOrWhiteSpace(manifest.Name))
			{
				manifest.Name = manifest.Id;
			}
			return new LoadedMod
			{
				RootPath = modDirectory,
				Manifest = manifest,
				Settings = LoadSettings(modDirectory, manifest),
				Tags = LoadTags(modDirectory),
				Contexts = LoadContexts(modDirectory),
				LinesRootPath = Path.Combine(modDirectory, "lines")
			};
		}
		catch (Exception ex)
		{
			error = ex.Message;
			return null;
		}
	}

	private static ModManifest LoadManifest(string modDirectory)
	{
		string manifestPath = Path.Combine(modDirectory, "mod.json");
		if (!File.Exists(manifestPath))
		{
			return new ModManifest
			{
				Id = Path.GetFileName(modDirectory),
				Name = Path.GetFileName(modDirectory),
				Enabled = true
			};
		}
		return ReadJsonFile<ModManifest>(manifestPath) ?? new ModManifest();
	}

	private static List<SettingDefinition> LoadSettings(string modDirectory, ModManifest manifest)
	{
		string settingsPath = Path.Combine(modDirectory, "settings", "settings.json");
		if (!File.Exists(settingsPath))
		{
			return new List<SettingDefinition>();
		}
		ModSettingsFile settingsFile = ReadJsonFile<ModSettingsFile>(settingsPath);
		if (settingsFile?.Settings == null)
		{
			return new List<SettingDefinition>();
		}
		return settingsFile.Settings.Select((ModSettingDefinition setting) => ToSettingDefinition(setting, manifest)).Where((SettingDefinition definition) => !string.IsNullOrWhiteSpace(definition.Key)).ToList();
	}

	private static SettingDefinition ToSettingDefinition(ModSettingDefinition modSetting, ModManifest manifest)
	{
		string sourceName = string.IsNullOrWhiteSpace(manifest.Name) ? manifest.Id : manifest.Name;
		return new SettingDefinition
		{
			Key = CleanKey(modSetting.Key),
			Label = string.IsNullOrWhiteSpace(modSetting.Label) ? CleanKey(modSetting.Key) : modSetting.Label.Trim(),
			Kind = ParseSettingKind(modSetting.Kind),
			LegacyEnabledFlag = CleanKey(modSetting.LegacyEnabledFlag),
			LegacyDisabledFlag = CleanKey(modSetting.LegacyDisabledFlag),
			LegacyValueKey = CleanKey(modSetting.LegacyValueKey),
			QueueableAsk = modSetting.QueueableAsk,
			MediaDiscoveryTags = (modSetting.MediaDiscoveryTags ?? new List<string>()).Where((string tag) => !string.IsNullOrWhiteSpace(tag)).Select((string tag) => tag.Trim()).ToList(),
			MediaDiscoveryMinimum = modSetting.MediaDiscoveryMinimum <= 0 ? 2 : modSetting.MediaDiscoveryMinimum,
			Description = (modSetting.Description ?? "").Trim(),
			ProgressionNote = string.IsNullOrWhiteSpace(modSetting.Warning) ? "Added by an enabled mod." : modSetting.Warning.Trim(),
			Group = (modSetting.Group ?? "").Trim(),
			SourceName = sourceName.Trim()
		};
	}

	private static List<ModTagDefinition> LoadTags(string modDirectory)
	{
		string tagsPath = Path.Combine(modDirectory, "tags", "tags.json");
		if (!File.Exists(tagsPath))
		{
			return new List<ModTagDefinition>();
		}
		ModTagsFile tagsFile = ReadJsonFile<ModTagsFile>(tagsPath);
		if (tagsFile?.Tags == null)
		{
			return new List<ModTagDefinition>();
		}
		return tagsFile.Tags.Where((ModTagDefinition tag) => !string.IsNullOrWhiteSpace(tag.Key) || !string.IsNullOrWhiteSpace(tag.Label)).ToList();
	}

	private static List<DerivedContextDefinition> LoadContexts(string modDirectory)
	{
		string contextsPath = Path.Combine(modDirectory, "contexts", "contexts.json");
		if (!File.Exists(contextsPath))
		{
			return new List<DerivedContextDefinition>();
		}
		ModContextsFile contextsFile = ReadJsonFile<ModContextsFile>(contextsPath);
		if (contextsFile?.Contexts == null)
		{
			return new List<DerivedContextDefinition>();
		}
		return contextsFile.Contexts.Where((DerivedContextDefinition context) => !string.IsNullOrWhiteSpace(context.Key)).ToList();
	}

	private static SettingKind ParseSettingKind(string kind)
	{
		if (Enum.TryParse(kind, ignoreCase: true, out SettingKind parsed))
		{
			return parsed;
		}
		return SettingKind.Toggle;
	}

	private static string CleanKey(string value)
	{
		return (value ?? "").Trim();
	}

	private static void SaveManifest(string modDirectory, ModManifest manifest)
	{
		Directory.CreateDirectory(modDirectory);
		File.WriteAllText(Path.Combine(modDirectory, "mod.json"), JsonSerializer.Serialize(manifest, JsonOptions));
	}

	private static void WriteIfMissing(string path, string content)
	{
		if (!File.Exists(path))
		{
			File.WriteAllText(path, content);
		}
	}

	private static ModSummary CreateErrorSummary(string modDirectory, string error)
	{
		return new ModSummary
		{
			RootPath = modDirectory,
			Id = Path.GetFileName(modDirectory),
			Name = Path.GetFileName(modDirectory),
			Enabled = false,
			Error = error
		};
	}

	private static string ValidateLoadedMod(LoadedMod mod)
	{
		List<string> warnings = new List<string>();
		if (string.IsNullOrWhiteSpace(mod.Manifest.Id))
		{
			warnings.Add("missing id");
		}
		if (mod.Settings.GroupBy((SettingDefinition setting) => setting.Key, StringComparer.OrdinalIgnoreCase).Any((IGrouping<string, SettingDefinition> group) => group.Count() > 1))
		{
			warnings.Add("duplicate setting keys");
		}
		if (mod.Tags.GroupBy((ModTagDefinition tag) => string.IsNullOrWhiteSpace(tag.Key) ? tag.Label : tag.Key, StringComparer.OrdinalIgnoreCase).Any((IGrouping<string, ModTagDefinition> group) => group.Count() > 1))
		{
			warnings.Add("duplicate tag keys");
		}
		if (mod.Contexts.GroupBy((DerivedContextDefinition context) => context.Key, StringComparer.OrdinalIgnoreCase).Any((IGrouping<string, DerivedContextDefinition> group) => group.Count() > 1))
		{
			warnings.Add("duplicate context keys");
		}
		return string.Join("; ", warnings);
	}

	private static T ReadJsonFile<T>(string path)
	{
		try
		{
			return JsonSerializer.Deserialize<T>(File.ReadAllText(path), JsonOptions);
		}
		catch (JsonException ex)
		{
			string location = "";
			if (ex.LineNumber.HasValue || ex.BytePositionInLine.HasValue)
			{
				location = " at line " + (ex.LineNumber ?? 0) + ", byte " + (ex.BytePositionInLine ?? 0);
			}
			throw new InvalidDataException(path + location + ": invalid JSON - " + ex.Message, ex);
		}
		catch (Exception ex)
		{
			throw new InvalidDataException(path + ": " + ex.Message, ex);
		}
	}

	private static void AppendGlobalWarnings(List<ModSummary> summaries, List<LoadedMod> enabledMods)
	{
		AppendDuplicateWarnings(summaries, enabledMods.SelectMany((LoadedMod mod) => mod.Settings.Select((SettingDefinition setting) => new KeyValuePair<string, string>(setting.Key, mod.RootPath))), "setting");
		AppendDuplicateWarnings(summaries, enabledMods.SelectMany((LoadedMod mod) => mod.Tags.Select((ModTagDefinition tag) => new KeyValuePair<string, string>(string.IsNullOrWhiteSpace(tag.Key) ? tag.Label : tag.Key, mod.RootPath))), "tag");
		AppendDuplicateWarnings(summaries, enabledMods.SelectMany((LoadedMod mod) => mod.Contexts.Select((DerivedContextDefinition context) => new KeyValuePair<string, string>(context.Key, mod.RootPath))), "context");
	}

	private static void AppendDuplicateWarnings(List<ModSummary> summaries, IEnumerable<KeyValuePair<string, string>> keys, string kind)
	{
		foreach (IGrouping<string, KeyValuePair<string, string>> duplicateGroup in keys.Where((KeyValuePair<string, string> pair) => !string.IsNullOrWhiteSpace(pair.Key)).GroupBy((KeyValuePair<string, string> pair) => pair.Key, StringComparer.OrdinalIgnoreCase).Where((IGrouping<string, KeyValuePair<string, string>> group) => group.Select((KeyValuePair<string, string> pair) => pair.Value).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1))
		{
			HashSet<string> affectedRoots = duplicateGroup.Select((KeyValuePair<string, string> pair) => pair.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
			foreach (ModSummary summary in summaries.Where((ModSummary summary) => affectedRoots.Contains(summary.RootPath)))
			{
				AppendWarning(summary, "duplicate enabled " + kind + " key: " + duplicateGroup.Key);
			}
		}
	}

	private static void AppendWarning(ModSummary summary, string warning)
	{
		if (string.IsNullOrWhiteSpace(summary.Error))
		{
			summary.Error = warning;
		}
		else if (!summary.Error.Contains(warning, StringComparison.OrdinalIgnoreCase))
		{
			summary.Error += "; " + warning;
		}
	}

	private static bool IsUsableLineRoot(string path)
	{
		return Directory.Exists(Path.Combine(path, "Scripts")) && Directory.Exists(Path.Combine(path, "Vocab"));
	}
}
