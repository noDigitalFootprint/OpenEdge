using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;

namespace OpenEdge;

public sealed class MediaCatalogService
{
	private const string LegacyTagSeparator = "kljnrbkrbasxalkxmbt";

	private static readonly string[] ImageExtensions = new string[6] { ".jpg", ".jpeg", ".png", ".bmp", ".webp", ".gif" };

	private static readonly string[] VideoExtensions = new string[4] { ".mp4", ".mpeg", ".mp3", ".wmv" };

	private readonly string sourcesFilePath;

	private readonly string identityFilePath;

	private readonly MediaTagCompatibilityAdapter tagCompatibilityAdapter;

	private MediaCatalogSnapshot snapshot = new MediaCatalogSnapshot();
	private readonly object snapshotLock = new object();

	public MediaCatalogService()
	{
		sourcesFilePath = Path.Combine(RuntimePaths.RuntimeRoot, "media-sources.json");
		identityFilePath = Path.Combine(RuntimePaths.RuntimeRoot, "media-tag-index.json");
		tagCompatibilityAdapter = new MediaTagCompatibilityAdapter(RuntimePaths.TagsFile, LegacyTagSeparator);
	}

	public MediaCatalogSnapshot Snapshot
	{
		get
		{
			lock (snapshotLock)
			{
				return snapshot;
			}
		}
	}

	public void Reload()
	{
		var newSnapshot = new MediaCatalogSnapshot();
		List<MediaSourceDefinition> sources = LoadSources();
		foreach (MediaSourceDefinition source in sources.OrderBy(delegate(MediaSourceDefinition item)
		{
			return item.SortOrder;
		}))
		{
			newSnapshot.Sources.Add(source);
			if (source.IsEnabled)
			{
				LoadSourceItems(source, newSnapshot);
			}
		}
		BindIdentities(newSnapshot);
		tagCompatibilityAdapter.LoadLegacyTags(newSnapshot, TryResolveLegacyTagClaim);
		BackfillTaggedIdentityFingerprints(newSnapshot);
		SaveIdentityStore(newSnapshot);
		
		lock (snapshotLock)
		{
			snapshot = newSnapshot;
		}
	}

	public IReadOnlyList<MediaSourceDefinition> GetSources()
	{
		lock (snapshotLock)
		{
			return snapshot.Sources;
		}
	}

	public void SaveSources(IReadOnlyList<MediaSourceDefinition> sources)
	{
		if (File.Exists(sourcesFilePath))
		{
			File.Copy(sourcesFilePath, sourcesFilePath + ".backup", overwrite: true);
		}
		string json = JsonSerializer.Serialize(sources, new JsonSerializerOptions
		{
			WriteIndented = true
		});
		File.WriteAllText(sourcesFilePath, json);
	}

	public List<string> GetActiveImagePaths()
	{
		lock (snapshotLock)
		{
			return snapshot.ActiveItems.Where(delegate(MediaItemRecord item)
			{
				return item.Kind == MediaKind.Image;
			}).Select(delegate(MediaItemRecord item)
			{
				return item.RelativePath;
			}).ToList();
		}
	}

	public List<string> GetActiveGifPaths()
	{
		lock (snapshotLock)
		{
			return snapshot.ActiveItems.Where(delegate(MediaItemRecord item)
			{
				return item.Kind == MediaKind.Gif;
			}).Select(delegate(MediaItemRecord item)
			{
				return item.RelativePath;
			}).ToList();
		}
	}

	public List<string> GetActiveVideoPaths()
	{
		lock (snapshotLock)
		{
			return snapshot.ActiveItems.Where(delegate(MediaItemRecord item)
			{
				return item.Kind == MediaKind.Video;
			}).Select(delegate(MediaItemRecord item)
			{
				return item.RelativePath;
			}).ToList();
		}
	}

	public List<string> GetTaggedImagePaths(string lookedForTag)
	{
		lock (snapshotLock)
		{
			return GetTaggedMedia(new string[1] { lookedForTag }, Array.Empty<string>()).Where(delegate(string path)
			{
				return path.StartsWith("\\images\\", StringComparison.OrdinalIgnoreCase) || path.StartsWith("/images/", StringComparison.OrdinalIgnoreCase);
			}).ToList();
		}
	}

	public List<string> GetTaggedVideoPaths(string lookedForTag)
	{
		lock (snapshotLock)
		{
			return GetTaggedMedia(new string[1] { lookedForTag }, Array.Empty<string>()).Where(delegate(string path)
			{
				return path.StartsWith("\\videos\\", StringComparison.OrdinalIgnoreCase) || path.StartsWith("/videos/", StringComparison.OrdinalIgnoreCase);
			}).ToList();
		}
	}

	public bool IsVideoPath(string relativePath)
	{
		lock (snapshotLock)
		{
			MediaItemRecord item = snapshot.Items.Find(delegate(MediaItemRecord candidate)
			{
				return string.Equals(candidate.RelativePath, relativePath, StringComparison.OrdinalIgnoreCase);
			});
			return item != null && item.Kind == MediaKind.Video;
		}
	}

	public bool IsImagePath(string relativePath)
	{
		lock (snapshotLock)
		{
			MediaItemRecord item = snapshot.Items.Find(delegate(MediaItemRecord candidate)
			{
				return string.Equals(candidate.RelativePath, relativePath, StringComparison.OrdinalIgnoreCase);
			});
			return item != null && (item.Kind == MediaKind.Image || item.Kind == MediaKind.Gif);
		}
	}

	public string GetTags(string relativePath)
	{
		lock (snapshotLock)
		{
			if (snapshot.IdentitiesByPath.TryGetValue(relativePath, out MediaIdentityRecord identity))
			{
				return identity.Tags;
			}
			return "";
		}
	}

	public void SetTags(string relativePath, string tags)
	{
		lock (snapshotLock)
		{
			SetTagsNoSave(relativePath, tags);
			tagCompatibilityAdapter.SaveLegacyTags(snapshot);
			SaveIdentityStore(snapshot);
		}
	}

	public LegacyTagImportResult ImportLegacyTags(string tagsFilePath, IEnumerable<string> oldMediaRoots, IProgress<string> progress = null, bool overwriteExistingTags = true)
	{
		lock (snapshotLock)
		{
			LegacyTagImportResult result = new LegacyTagImportResult();
			List<string> roots = (oldMediaRoots ?? Array.Empty<string>()).Where((string root) => !string.IsNullOrWhiteSpace(root) && Directory.Exists(root)).Select((string root) => root.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
			if (string.IsNullOrWhiteSpace(tagsFilePath) || !File.Exists(tagsFilePath))
			{
				result.ReportPath = WriteLegacyTagImportReport(result, tagsFilePath, roots);
				progress?.Report("Legacy tags file was not found.");
				return result;
			}
			string[] lines = File.ReadAllLines(tagsFilePath);
			progress?.Report("Reading " + lines.Length + " legacy tag entries...");
			HashSet<string> assignedIdentityIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (string line in lines)
			{
				result.LinesRead++;
				if (result.LinesRead % 100 == 0)
				{
					progress?.Report("Importing legacy tags: " + result.LinesRead + " / " + lines.Length + " read, " + result.ImportedCount + " imported...");
				}
				string[] parts = line.Split(new string[1] { LegacyTagSeparator }, StringSplitOptions.None);
				if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
				{
					result.UnmatchedCount++;
					result.Details.Add("unmatched malformed line " + result.LinesRead);
					continue;
				}
				string legacyPath = parts[0].Trim();
				string tags = parts[1].Trim();
				if (TryImportLegacyTagClaim(legacyPath, tags, roots, result, assignedIdentityIds, overwriteExistingTags))
				{
					result.ImportedCount++;
				}
			}
			progress?.Report("Writing canonical tag index and legacy mirror...");
			tagCompatibilityAdapter.SaveLegacyTags(snapshot);
			SaveIdentityStore(snapshot);
			result.ReportPath = WriteLegacyTagImportReport(result, tagsFilePath, roots);
			progress?.Report("Import complete: " + result.ImportedCount + " imported, " + result.SkippedExistingCount + " existing skipped, " + result.AmbiguousCount + " ambiguous, " + result.UnmatchedCount + " unmatched.");
			return result;
		}
	}

	public BulkMoveResult MoveMedia(IEnumerable<string> relativePaths, string destinationFolder)
	{
		lock (snapshotLock)
		{
			BulkMoveResult result = new BulkMoveResult();
			if (string.IsNullOrWhiteSpace(destinationFolder))
			{
				return result;
			}
			Directory.CreateDirectory(destinationFolder);
			List<string> list = relativePaths.Where(delegate(string path)
			{
				return !string.IsNullOrWhiteSpace(path);
			}).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
			result.RequestedCount = list.Count;
			foreach (string relativePath in list)
			{
				string sourcePath = RuntimePaths.ResolveRuntimePath(relativePath);
				if (!File.Exists(sourcePath))
				{
					result.SkippedCount++;
					continue;
				}
				string destinationPath = GetAvailableDestinationPath(destinationFolder, Path.GetFileName(sourcePath));
				if (!string.Equals(Path.GetFileName(sourcePath), Path.GetFileName(destinationPath), StringComparison.OrdinalIgnoreCase))
				{
					result.RenamedCount++;
				}
				File.Move(sourcePath, destinationPath);
				string destinationRelativePath = RuntimePaths.ToRuntimeRelativePath(destinationPath);
				if (snapshot.IdentitiesByPath.TryGetValue(relativePath, out MediaIdentityRecord identity))
				{
					snapshot.IdentitiesByPath.Remove(relativePath);
					identity.CurrentRelativePath = destinationRelativePath;
					identity.FileName = Path.GetFileName(destinationPath);
					FileInfo fileInfo = new FileInfo(destinationPath);
					identity.SizeBytes = fileInfo.Length;
					identity.LastWriteUtcTicks = fileInfo.LastWriteTimeUtc.Ticks;
					identity.SourceId = FindSourceIdForPath(destinationPath);
					identity.IsMissing = false;
					if (!identity.KnownRelativePaths.Contains(destinationRelativePath, StringComparer.OrdinalIgnoreCase))
					{
						identity.KnownRelativePaths.Add(destinationRelativePath);
					}
					snapshot.IdentitiesByPath[destinationRelativePath] = identity;
				}
				result.MovedCount++;
			}
			if (result.MovedCount > 0)
			{
				tagCompatibilityAdapter.SaveLegacyTags(snapshot);
				SaveIdentityStore(snapshot);
			}
			return result;
		}
	}

	public int ApplyBulkTags(IEnumerable<string> relativePaths, IReadOnlyCollection<string> selectedTags, BulkTagOperationMode mode, IReadOnlyList<string> knownTagsInDisplayOrder)
	{
		lock (snapshotLock)
		{
			List<string> list = relativePaths.Where(delegate(string path)
			{
				return !string.IsNullOrWhiteSpace(path);
			}).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
			if (list.Count == 0)
			{
				return 0;
			}
			HashSet<string> hashSet = new HashSet<string>(selectedTags.Where(delegate(string tag)
			{
				return !string.IsNullOrWhiteSpace(tag);
			}), StringComparer.Ordinal);
			List<string> list2 = knownTagsInDisplayOrder.Where(delegate(string tag)
			{
				return !string.IsNullOrWhiteSpace(tag);
			}).Distinct(StringComparer.Ordinal).ToList();
			List<string> list3 = list2.Where(hashSet.Contains).ToList();
			List<string> list4 = list2.OrderByDescending((string tag) => tag.Length).ThenBy((string tag) => tag, StringComparer.Ordinal).ToList();
			int num = 0;
			foreach (string item in list)
			{
				string text = GetTagsNoLock(item);
				string text2 = ApplyBulkTagOperation(text, list3, hashSet, list4, mode);
				if (!string.Equals(text, text2, StringComparison.Ordinal))
				{
					SetTagsNoSave(item, text2);
					num++;
				}
			}
			if (num > 0)
			{
				tagCompatibilityAdapter.SaveLegacyTags(snapshot);
				SaveIdentityStore(snapshot);
			}
			return num;
		}
	}

	public Dictionary<string, string> GetTagMapSnapshot()
	{
		lock (snapshotLock)
		{
			Dictionary<string, string> tagMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			foreach (MediaItemRecord item in snapshot.ActiveItems)
			{
				if (snapshot.IdentitiesByPath.TryGetValue(item.RelativePath, out MediaIdentityRecord identity) && !string.IsNullOrWhiteSpace(identity.Tags))
				{
					tagMap[item.RelativePath] = identity.Tags;
				}
			}
			return tagMap;
		}
	}

	public List<string> GetTaggedMedia(string[] requiredTags, string[] disallowedTags)
	{
		lock (snapshotLock)
		{
			List<string> media = new List<string>();
			foreach (MediaItemRecord item in snapshot.ActiveItems)
			{
				string tags = GetTags(item.RelativePath);
				if (!MatchesTags(tags, requiredTags, disallowedTags))
				{
					continue;
				}
				media.Add(item.RelativePath);
			}
			return media;
		}
	}

	private bool TryImportLegacyTagClaim(string legacyPath, string tags, List<string> oldMediaRoots, LegacyTagImportResult result, HashSet<string> assignedIdentityIds, bool overwriteExistingTags)
	{
		string normalizedLegacyPath = NormalizeRuntimeRelativePath(legacyPath);
		if (snapshot.IdentitiesByPath.TryGetValue(normalizedLegacyPath, out MediaIdentityRecord exactIdentity))
		{
			if (!TryApplyLegacyImportTags(exactIdentity, legacyPath, tags, "exact", result, assignedIdentityIds, overwriteExistingTags))
			{
				return false;
			}
			result.ExactMatches++;
			return true;
		}
		MediaIdentityRecord aliasIdentity = snapshot.IdentitiesById.Values.FirstOrDefault(delegate(MediaIdentityRecord candidate)
		{
			return candidate.KnownRelativePaths.Contains(normalizedLegacyPath, StringComparer.OrdinalIgnoreCase) || candidate.KnownRelativePaths.Contains(legacyPath, StringComparer.OrdinalIgnoreCase);
		});
		if (aliasIdentity != null)
		{
			if (!TryApplyLegacyImportTags(aliasIdentity, legacyPath, tags, "alias", result, assignedIdentityIds, overwriteExistingTags))
			{
				return false;
			}
			result.AliasMatches++;
			return true;
		}
		MediaIdentityRecord fingerprintIdentity = ResolveLegacyImportByFingerprint(legacyPath, oldMediaRoots);
		if (fingerprintIdentity != null)
		{
			if (!TryApplyLegacyImportTags(fingerprintIdentity, legacyPath, tags, "fingerprint", result, assignedIdentityIds, overwriteExistingTags))
			{
				return false;
			}
			AddKnownLegacyPath(fingerprintIdentity, normalizedLegacyPath);
			result.FingerprintMatches++;
			return true;
		}
		string fileName = Path.GetFileName(legacyPath);
		List<MediaItemRecord> filenameCandidates = snapshot.Items.Where(delegate(MediaItemRecord item)
		{
			return string.Equals(item.FileName, fileName, StringComparison.OrdinalIgnoreCase);
		}).ToList();
		if (filenameCandidates.Count == 1 && snapshot.IdentitiesByPath.TryGetValue(filenameCandidates[0].RelativePath, out MediaIdentityRecord filenameIdentity))
		{
			if (!TryApplyLegacyImportTags(filenameIdentity, legacyPath, tags, "filename", result, assignedIdentityIds, overwriteExistingTags))
			{
				return false;
			}
			AddKnownLegacyPath(filenameIdentity, normalizedLegacyPath);
			result.FilenameMatches++;
			return true;
		}
		if (filenameCandidates.Count > 1)
		{
			result.AmbiguousCount++;
			result.Details.Add("ambiguous " + legacyPath + " candidates=" + filenameCandidates.Count);
			return false;
		}
		result.UnmatchedCount++;
		result.Details.Add("unmatched " + legacyPath);
		return false;
	}

	private static bool TryApplyLegacyImportTags(MediaIdentityRecord identity, string legacyPath, string tags, string matchType, LegacyTagImportResult result, HashSet<string> assignedIdentityIds, bool overwriteExistingTags)
	{
		if (identity == null)
		{
			return false;
		}
		if (assignedIdentityIds.Contains(identity.MediaId))
		{
			result.AmbiguousCount++;
			result.Details.Add("ambiguous duplicate-target " + legacyPath + " -> " + identity.CurrentRelativePath + " already matched earlier in this import");
			return false;
		}
		if (!overwriteExistingTags && !string.IsNullOrWhiteSpace(identity.Tags))
		{
			result.SkippedExistingCount++;
			result.Details.Add("skipped-existing " + legacyPath + " -> " + identity.CurrentRelativePath);
			return false;
		}
		identity.Tags = tags;
		assignedIdentityIds.Add(identity.MediaId);
		result.Details.Add(matchType + " " + legacyPath + " -> " + identity.CurrentRelativePath);
		return true;
	}

	private MediaIdentityRecord ResolveLegacyImportByFingerprint(string legacyPath, List<string> oldMediaRoots)
	{
		foreach (string oldFullPath in GetLegacyImportCandidatePaths(legacyPath, oldMediaRoots))
		{
			if (!File.Exists(oldFullPath))
			{
				continue;
			}
			FileInfo fileInfo = new FileInfo(oldFullPath);
			string fingerprint = ComputeQuickFingerprint(oldFullPath, fileInfo.Length);
			List<MediaItemRecord> matches = snapshot.Items.Where(delegate(MediaItemRecord item)
			{
				return item.SizeBytes == fileInfo.Length && EnsureFingerprint(item) == fingerprint;
			}).ToList();
			if (matches.Count == 1 && snapshot.IdentitiesByPath.TryGetValue(matches[0].RelativePath, out MediaIdentityRecord identity))
			{
				return identity;
			}
			if (matches.Count > 1)
			{
				SessionTraceLogger.Info("media-tags", "ambiguous legacy import fingerprint path=" + legacyPath + " candidates=" + matches.Count);
				return null;
			}
		}
		return null;
	}

	private static IEnumerable<string> GetLegacyImportCandidatePaths(string legacyPath, List<string> oldMediaRoots)
	{
		string trimmed = legacyPath.Trim().TrimStart('\\', '/').Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
		if (Path.IsPathRooted(legacyPath))
		{
			yield return legacyPath;
		}
		foreach (string root in oldMediaRoots)
		{
			yield return Path.Combine(root, trimmed);
			int separatorIndex = trimmed.IndexOf(Path.DirectorySeparatorChar);
			if (separatorIndex > 0)
			{
				string withoutFirstFolder = trimmed.Substring(separatorIndex + 1);
				yield return Path.Combine(root, withoutFirstFolder);
			}
		}
	}

	private static string NormalizeRuntimeRelativePath(string path)
	{
		string trimmed = (path ?? "").Trim().TrimStart('\\', '/').Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
		return Path.DirectorySeparatorChar + trimmed;
	}

	private static void AddKnownLegacyPath(MediaIdentityRecord identity, string legacyPath)
	{
		if (!identity.KnownRelativePaths.Contains(legacyPath, StringComparer.OrdinalIgnoreCase))
		{
			identity.KnownRelativePaths.Add(legacyPath);
		}
	}

	private static string WriteLegacyTagImportReport(LegacyTagImportResult result, string tagsFilePath, List<string> oldMediaRoots)
	{
		Directory.CreateDirectory(RuntimePaths.DebugDir);
		string reportPath = Path.Combine(RuntimePaths.DebugDir, "legacy-tag-import-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".md");
		List<string> lines = new List<string>
		{
			"# Legacy tag import report",
			"",
			"Generated: " + DateTime.Now.ToString("u"),
			"Tags file: `" + (tagsFilePath ?? "") + "`",
			"Old media roots: " + (oldMediaRoots.Count == 0 ? "(none)" : string.Join(", ", oldMediaRoots.Select((string root) => "`" + root + "`"))),
			"",
			"## Summary",
			"",
			"- Lines read: " + result.LinesRead,
			"- Imported: " + result.ImportedCount,
			"- Exact matches: " + result.ExactMatches,
			"- Alias matches: " + result.AliasMatches,
			"- Fingerprint matches: " + result.FingerprintMatches,
			"- Filename fallback matches: " + result.FilenameMatches,
			"- Existing tags skipped: " + result.SkippedExistingCount,
			"- Ambiguous skipped: " + result.AmbiguousCount,
			"- Unmatched: " + result.UnmatchedCount,
			"",
			"## Details",
			""
		};
		lines.AddRange(result.Details.Select((string detail) => "- " + detail));
		File.WriteAllLines(reportPath, lines);
		return reportPath;
	}

	private List<MediaSourceDefinition> LoadSources()
	{
		List<MediaSourceDefinition> savedSources = TryLoadSourcesFile(sourcesFilePath);
		if (savedSources != null && savedSources.Count > 0)
		{
			return savedSources;
		}
		savedSources = TryLoadSourcesFile(sourcesFilePath + ".backup");
		if (savedSources != null && savedSources.Count > 0)
		{
			SaveSources(savedSources);
			return savedSources;
		}
		List<MediaSourceDefinition> legacySources = new List<MediaSourceDefinition>
		{
			new MediaSourceDefinition
			{
				Id = "legacy-images",
				Name = "Legacy Images",
				RootPath = RuntimePaths.ImagesDir,
				IsEnabled = true,
				ImagesEnabled = true,
				VideosEnabled = false,
				IsLegacy = true,
				SortOrder = 0
			},
			new MediaSourceDefinition
			{
				Id = "legacy-videos",
				Name = "Legacy Videos",
				RootPath = RuntimePaths.VideosDir,
				IsEnabled = true,
				ImagesEnabled = false,
				VideosEnabled = true,
				IsLegacy = true,
				SortOrder = 1
			}
		};
		SaveSources(legacySources);
		return legacySources;
	}

	private static List<MediaSourceDefinition> TryLoadSourcesFile(string path)
	{
		try
		{
			if (File.Exists(path))
			{
				return JsonSerializer.Deserialize<List<MediaSourceDefinition>>(File.ReadAllText(path));
			}
		}
		catch
		{
		}
		return null;
	}

	private void LoadSourceItems(MediaSourceDefinition source, MediaCatalogSnapshot targetSnapshot)
	{
		if (!Directory.Exists(source.RootPath))
		{
			return;
		}
		foreach (string fullPath in SearchSubFolders(source.RootPath))
		{
			string folderPath = Path.GetDirectoryName(fullPath) ?? source.RootPath;
			if (!IsFolderIncluded(source, folderPath))
			{
				continue;
			}
			FileInfo fileInfo = new FileInfo(fullPath);
			string extension = Path.GetExtension(fullPath).ToLowerInvariant();
			MediaKind? kind = GetMediaKind(extension);
			if (!kind.HasValue)
			{
				continue;
			}
			if ((kind.Value == MediaKind.Image || kind.Value == MediaKind.Gif) && !source.ImagesEnabled)
			{
				continue;
			}
			if (kind.Value == MediaKind.Video && !source.VideosEnabled)
			{
				continue;
			}
			targetSnapshot.Items.Add(new MediaItemRecord
			{
				SourceId = source.Id,
				Kind = kind.Value,
				FullPath = fullPath,
				RelativePath = RuntimePaths.ToRuntimeRelativePath(fullPath),
				FileName = fileInfo.Name,
				SizeBytes = fileInfo.Length,
				LastWriteUtcTicks = fileInfo.LastWriteTimeUtc.Ticks
			});
		}
	}

	private void BindIdentities(MediaCatalogSnapshot targetSnapshot)
	{
		MediaIdentityStore store = LoadIdentityStore();
		Dictionary<string, MediaIdentityRecord> identitiesByPath = new Dictionary<string, MediaIdentityRecord>(StringComparer.OrdinalIgnoreCase);
		foreach (MediaIdentityRecord identity in store.Items)
		{
			targetSnapshot.IdentitiesById[identity.MediaId] = identity;
			if (!string.IsNullOrWhiteSpace(identity.CurrentRelativePath))
			{
				identitiesByPath[identity.CurrentRelativePath] = identity;
			}
			foreach (string knownPath in identity.KnownRelativePaths)
			{
				if (!identitiesByPath.ContainsKey(knownPath))
				{
					identitiesByPath[knownPath] = identity;
				}
			}
		}

		List<MediaIdentityRecord> unboundIdentities = new List<MediaIdentityRecord>(store.Items);
		foreach (MediaItemRecord item in targetSnapshot.Items)
		{
			MediaIdentityRecord identity = ResolveIdentity(item, identitiesByPath, unboundIdentities);
			if (identity == null)
			{
				identity = new MediaIdentityRecord
				{
					SourceId = item.SourceId,
					Kind = item.Kind
				};
				store.Items.Add(identity);
				targetSnapshot.IdentitiesById[identity.MediaId] = identity;
			}
			identity.SourceId = item.SourceId;
			identity.Kind = item.Kind;
			identity.CurrentRelativePath = item.RelativePath;
			identity.FileName = item.FileName;
			identity.SizeBytes = item.SizeBytes;
			identity.LastWriteUtcTicks = item.LastWriteUtcTicks;
			identity.IsMissing = false;
			if (!identity.KnownRelativePaths.Contains(item.RelativePath, StringComparer.OrdinalIgnoreCase))
			{
				identity.KnownRelativePaths.Add(item.RelativePath);
			}
			item.MediaId = identity.MediaId;
			item.QuickFingerprint = identity.QuickFingerprint;
			targetSnapshot.IdentitiesByPath[item.RelativePath] = identity;
		}
		foreach (MediaIdentityRecord remainingIdentity in store.Items)
		{
			if (targetSnapshot.Items.All(delegate(MediaItemRecord item)
			{
				return item.MediaId != remainingIdentity.MediaId;
			}))
			{
				remainingIdentity.IsMissing = true;
				if (!string.IsNullOrWhiteSpace(remainingIdentity.CurrentRelativePath) && targetSnapshot.Items.Any(delegate(MediaItemRecord item)
				{
					return !string.Equals(item.MediaId, remainingIdentity.MediaId, StringComparison.OrdinalIgnoreCase) && string.Equals(item.RelativePath, remainingIdentity.CurrentRelativePath, StringComparison.OrdinalIgnoreCase);
				}))
				{
					remainingIdentity.CurrentRelativePath = "";
				}
			}
		}
	}

	public MediaTagCompatibilityDiagnostics GetTagCompatibilityDiagnostics()
	{
		return tagCompatibilityAdapter.LastDiagnostics;
	}

	private MediaIdentityStore LoadIdentityStore()
	{
		if (File.Exists(identityFilePath))
		{
			MediaIdentityStore store = JsonSerializer.Deserialize<MediaIdentityStore>(File.ReadAllText(identityFilePath));
			if (store != null)
			{
				return store;
			}
		}
		return new MediaIdentityStore();
	}

	private void SaveIdentityStore(MediaCatalogSnapshot targetSnapshot)
	{
		MediaIdentityStore store = new MediaIdentityStore
		{
			Items = targetSnapshot.IdentitiesById.Values.OrderBy(delegate(MediaIdentityRecord item)
			{
				return item.CurrentRelativePath;
			}).ToList()
		};
		string json = JsonSerializer.Serialize(store, new JsonSerializerOptions
		{
			WriteIndented = true
		});
		File.WriteAllText(identityFilePath, json);
	}

	private MediaIdentityRecord ResolveIdentity(MediaItemRecord item, Dictionary<string, MediaIdentityRecord> identitiesByPath, List<MediaIdentityRecord> unboundIdentities)
	{
		if (identitiesByPath.TryGetValue(item.RelativePath, out MediaIdentityRecord exactIdentity))
		{
			if (string.IsNullOrWhiteSpace(exactIdentity.Tags))
			{
				List<MediaIdentityRecord> taggedFingerprintCandidates = unboundIdentities.Where(delegate(MediaIdentityRecord identity)
				{
					return !ReferenceEquals(identity, exactIdentity) && identity.Kind == item.Kind && identity.SizeBytes == item.SizeBytes && !string.IsNullOrWhiteSpace(identity.Tags) && !string.IsNullOrWhiteSpace(identity.QuickFingerprint);
				}).ToList();
				MediaIdentityRecord taggedFingerprintMatch = ResolveByStoredFingerprint(item, taggedFingerprintCandidates);
				if (taggedFingerprintMatch != null)
				{
					unboundIdentities.Remove(taggedFingerprintMatch);
					exactIdentity.IsMissing = true;
					exactIdentity.CurrentRelativePath = "";
					return taggedFingerprintMatch;
				}
			}
			unboundIdentities.Remove(exactIdentity);
			return exactIdentity;
		}
		List<MediaIdentityRecord> sourceCandidates = unboundIdentities.Where(delegate(MediaIdentityRecord identity)
		{
			return identity.SourceId == item.SourceId && string.Equals(identity.FileName, item.FileName, StringComparison.OrdinalIgnoreCase) && identity.SizeBytes == item.SizeBytes;
		}).ToList();
		if (sourceCandidates.Count == 1)
		{
			unboundIdentities.Remove(sourceCandidates[0]);
			return sourceCandidates[0];
		}
		List<MediaIdentityRecord> metadataCandidates = unboundIdentities.Where(delegate(MediaIdentityRecord identity)
		{
			return identity.Kind == item.Kind && identity.SizeBytes == item.SizeBytes && identity.LastWriteUtcTicks == item.LastWriteUtcTicks;
		}).ToList();
		if (metadataCandidates.Count == 1)
		{
			unboundIdentities.Remove(metadataCandidates[0]);
			return metadataCandidates[0];
		}
		if (metadataCandidates.Count > 1)
		{
			MediaIdentityRecord fingerprintMatch = ResolveByStoredFingerprint(item, metadataCandidates);
			if (fingerprintMatch != null)
			{
				unboundIdentities.Remove(fingerprintMatch);
				return fingerprintMatch;
			}
		}
		List<MediaIdentityRecord> fingerprintCandidates = unboundIdentities.Where(delegate(MediaIdentityRecord identity)
		{
			return identity.Kind == item.Kind && identity.SizeBytes == item.SizeBytes && !string.IsNullOrWhiteSpace(identity.QuickFingerprint);
		}).ToList();
		if (fingerprintCandidates.Count > 0)
		{
			MediaIdentityRecord fingerprintMatch = ResolveByStoredFingerprint(item, fingerprintCandidates);
			if (fingerprintMatch != null)
			{
				unboundIdentities.Remove(fingerprintMatch);
				return fingerprintMatch;
			}
		}
		return null;
	}

	private MediaIdentityRecord ResolveByStoredFingerprint(MediaItemRecord item, List<MediaIdentityRecord> candidates)
	{
		if (candidates.Count == 0)
		{
			return null;
		}
		string itemFingerprint = EnsureFingerprint(item);
		List<MediaIdentityRecord> fingerprintMatches = candidates.Where(delegate(MediaIdentityRecord identity)
		{
			return !string.IsNullOrWhiteSpace(identity.QuickFingerprint) && identity.QuickFingerprint == itemFingerprint;
		}).ToList();
		if (fingerprintMatches.Count == 1)
		{
			return fingerprintMatches[0];
		}
		if (fingerprintMatches.Count > 1)
		{
			List<MediaIdentityRecord> sameSourceAndNameMatches = fingerprintMatches.Where(delegate(MediaIdentityRecord identity)
			{
				return string.Equals(identity.SourceId, item.SourceId, StringComparison.OrdinalIgnoreCase) && string.Equals(identity.FileName, item.FileName, StringComparison.OrdinalIgnoreCase);
			}).ToList();
			if (sameSourceAndNameMatches.Count == 1)
			{
				return sameSourceAndNameMatches[0];
			}
			SessionTraceLogger.Info("media-identity", "ambiguous fingerprint matches path=" + item.RelativePath + " candidates=" + fingerprintMatches.Count);
		}
		return null;
	}

	private bool TryResolveLegacyTagClaim(MediaCatalogSnapshot targetSnapshot, string legacyPath, string tags, out MediaIdentityRecord matchedIdentity)
	{
		matchedIdentity = null;
		string legacyName = Path.GetFileName(legacyPath);
		List<MediaItemRecord> sizeCandidates = targetSnapshot.Items.Where(delegate(MediaItemRecord item)
		{
			return string.Equals(item.FileName, legacyName, StringComparison.OrdinalIgnoreCase);
		}).ToList();
		if (sizeCandidates.Count == 1)
		{
			matchedIdentity = targetSnapshot.IdentitiesByPath[sizeCandidates[0].RelativePath];
			return true;
		}
		return false;
	}

	private void BackfillTaggedIdentityFingerprints(MediaCatalogSnapshot targetSnapshot)
	{
		foreach (MediaItemRecord item in targetSnapshot.Items)
		{
			if (!targetSnapshot.IdentitiesById.TryGetValue(item.MediaId, out MediaIdentityRecord identity))
			{
				continue;
			}
			if (!string.IsNullOrWhiteSpace(identity.Tags) && string.IsNullOrWhiteSpace(identity.QuickFingerprint))
			{
				identity.QuickFingerprint = EnsureFingerprint(item);
			}
		}
	}

	private string EnsureFingerprint(MediaItemRecord item)
	{
		if (!string.IsNullOrWhiteSpace(item.QuickFingerprint))
		{
			return item.QuickFingerprint;
		}
		if (item == null || string.IsNullOrWhiteSpace(item.FullPath) || !File.Exists(item.FullPath))
		{
			SessionTraceLogger.Info("media-fingerprint-skip", "Skipped fingerprint for missing media: " + (item?.FullPath ?? ""));
			return "";
		}
		try
		{
			item.QuickFingerprint = ComputeQuickFingerprint(item.FullPath, item.SizeBytes);
		}
		catch (IOException ex)
		{
			SessionTraceLogger.Error("media-fingerprint", "Failed to fingerprint media: " + item.FullPath, ex);
			return "";
		}
		catch (UnauthorizedAccessException ex)
		{
			SessionTraceLogger.Error("media-fingerprint", "Failed to access media for fingerprint: " + item.FullPath, ex);
			return "";
		}
		if (snapshot.IdentitiesById.TryGetValue(item.MediaId, out MediaIdentityRecord identity))
		{
			identity.QuickFingerprint = item.QuickFingerprint;
		}
		return item.QuickFingerprint;
	}

	private static string ComputeQuickFingerprint(string fullPath, long fileSize)
	{
		using SHA256 sha256 = SHA256.Create();
		using FileStream stream = File.OpenRead(fullPath);
		int sampleSize = (int)Math.Min(65536, fileSize);
		byte[] startBuffer = new byte[sampleSize];
		stream.Read(startBuffer, 0, sampleSize);
		sha256.TransformBlock(startBuffer, 0, sampleSize, null, 0);
		if (fileSize > sampleSize)
		{
			stream.Position = Math.Max(0, fileSize - sampleSize);
			byte[] endBuffer = new byte[sampleSize];
			stream.Read(endBuffer, 0, sampleSize);
			sha256.TransformBlock(endBuffer, 0, sampleSize, null, 0);
		}
		byte[] sizeBytes = BitConverter.GetBytes(fileSize);
		sha256.TransformFinalBlock(sizeBytes, 0, sizeBytes.Length);
		return Convert.ToHexString(sha256.Hash);
	}

	private static IEnumerable<string> SearchSubFolders(string currentDirectory)
	{
		List<string> files = new List<string>();
		foreach (string directory in Directory.GetDirectories(currentDirectory))
		{
			files.AddRange(SearchSubFolders(directory));
		}
		files.AddRange(Directory.GetFiles(currentDirectory));
		return files;
	}

	private static bool IsFolderIncluded(MediaSourceDefinition source, string fullFolderPath)
	{
		if (source.FolderRules == null || source.FolderRules.Count == 0)
		{
			return true;
		}
		string relativeFolderPath = NormalizeRelativeFolderPath(Path.GetRelativePath(source.RootPath, fullFolderPath));
		bool isIncluded = true;
		int bestMatchLength = -1;
		foreach (MediaFolderRule rule in source.FolderRules)
		{
			if (!rule.IsIncluded.HasValue)
			{
				continue;
			}
			string rulePath = NormalizeRelativeFolderPath(rule.RelativeFolderPath);
			bool isMatch = string.IsNullOrEmpty(rulePath) || string.Equals(relativeFolderPath, rulePath, StringComparison.OrdinalIgnoreCase) || relativeFolderPath.StartsWith(rulePath + "/", StringComparison.OrdinalIgnoreCase);
			if (isMatch && rulePath.Length > bestMatchLength)
			{
				bestMatchLength = rulePath.Length;
				isIncluded = rule.IsIncluded.Value;
			}
		}
		return isIncluded;
	}

	private static string NormalizeRelativeFolderPath(string relativeFolderPath)
	{
		if (string.IsNullOrWhiteSpace(relativeFolderPath) || relativeFolderPath == ".")
		{
			return "";
		}
		return relativeFolderPath.Replace('\\', '/').Trim('/');
	}

	private string FindSourceIdForPath(string fullPath)
	{
		MediaSourceDefinition source = snapshot.Sources.Where(delegate(MediaSourceDefinition candidate)
		{
			return IsSameDirectoryOrChild(fullPath, candidate.RootPath);
		}).OrderByDescending(delegate(MediaSourceDefinition candidate)
		{
			return candidate.RootPath?.Length ?? 0;
		}).FirstOrDefault();
		return source?.Id ?? "";
	}

	private static string GetAvailableDestinationPath(string destinationFolder, string fileName)
	{
		string destinationPath = Path.Combine(destinationFolder, fileName);
		if (!File.Exists(destinationPath))
		{
			return destinationPath;
		}
		string name = Path.GetFileNameWithoutExtension(fileName);
		string extension = Path.GetExtension(fileName);
		int index = 1;
		do
		{
			destinationPath = Path.Combine(destinationFolder, name + " (" + index + ")" + extension);
			index++;
		}
		while (File.Exists(destinationPath));
		return destinationPath;
	}

	private static bool IsSameDirectoryOrChild(string path, string parentPath)
	{
		if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(parentPath))
		{
			return false;
		}
		if (string.Equals(path, parentPath, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		string value = parentPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
		return path.StartsWith(value, StringComparison.OrdinalIgnoreCase);
	}

	private static MediaKind? GetMediaKind(string extension)
	{
		if (ImageExtensions.Contains(extension))
		{
			return extension == ".gif" ? MediaKind.Gif : MediaKind.Image;
		}
		if (VideoExtensions.Contains(extension))
		{
			return MediaKind.Video;
		}
		return null;
	}

	private static bool MatchesTags(string tags, string[] requiredTags, string[] disallowedTags)
	{
		foreach (string required in requiredTags)
		{
			if (required != "" && !tags.Contains(required))
			{
				return false;
			}
		}
		foreach (string disallowed in disallowedTags)
		{
			if (disallowed != "" && tags.Contains(disallowed))
			{
				return false;
			}
		}
		return true;
	}

	private string GetTagsNoLock(string relativePath)
	{
		if (snapshot.IdentitiesByPath.TryGetValue(relativePath, out MediaIdentityRecord value))
		{
			return value.Tags ?? "";
		}
		return "";
	}

	private void SetTagsNoSave(string relativePath, string tags)
	{
		MediaIdentityRecord orCreateIdentity = GetOrCreateIdentity(relativePath);
		orCreateIdentity.Tags = (string.IsNullOrWhiteSpace(tags) ? "" : tags);
		MediaItemRecord currentItem = snapshot.Items.FirstOrDefault(delegate(MediaItemRecord item)
		{
			return string.Equals(item.RelativePath, relativePath, StringComparison.OrdinalIgnoreCase);
		});
		if (currentItem != null && string.IsNullOrWhiteSpace(orCreateIdentity.QuickFingerprint))
		{
			orCreateIdentity.QuickFingerprint = EnsureFingerprint(currentItem);
		}
	}

	private MediaIdentityRecord GetOrCreateIdentity(string relativePath)
	{
		if (!snapshot.IdentitiesByPath.TryGetValue(relativePath, out MediaIdentityRecord value))
		{
			value = new MediaIdentityRecord
			{
				CurrentRelativePath = relativePath,
				KnownRelativePaths = new List<string> { relativePath }
			};
			snapshot.IdentitiesByPath[relativePath] = value;
			snapshot.IdentitiesById[value.MediaId] = value;
		}
		else if (!value.KnownRelativePaths.Contains(relativePath, StringComparer.OrdinalIgnoreCase))
		{
			value.KnownRelativePaths.Add(relativePath);
		}
		value.CurrentRelativePath = relativePath;
		return value;
	}

	private static string ApplyBulkTagOperation(string currentTags, IReadOnlyList<string> mergeTags, IReadOnlySet<string> selectedTags, IReadOnlyList<string> removalOrder, BulkTagOperationMode mode)
	{
		string text = currentTags ?? "";
		switch (mode)
		{
		case BulkTagOperationMode.Merge:
			foreach (string mergeTag in mergeTags)
			{
				if (!text.Contains(mergeTag, StringComparison.Ordinal))
				{
					text += mergeTag;
				}
			}
			break;
		case BulkTagOperationMode.RemoveSpecific:
			foreach (string item in removalOrder.Where(selectedTags.Contains))
			{
				text = text.Replace(item, "", StringComparison.Ordinal);
			}
			break;
		case BulkTagOperationMode.KeepOnlySelected:
			foreach (string item2 in removalOrder)
			{
				if (!selectedTags.Contains(item2))
				{
					text = text.Replace(item2, "", StringComparison.Ordinal);
				}
			}
			break;
		case BulkTagOperationMode.RemoveAll:
			text = "";
			break;
		}
		return text;
	}
}
