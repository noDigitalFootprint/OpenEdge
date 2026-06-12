using System;
using System.Collections.Generic;

namespace OpenEdge;

public enum MediaKind
{
	Image,
	Video,
	Gif
}

public sealed class MediaSourceDefinition
{
	public string Id { get; set; } = Guid.NewGuid().ToString("N");

	public string Name { get; set; } = "";

	public string RootPath { get; set; } = "";

	public bool IsEnabled { get; set; } = true;

	public bool ImagesEnabled { get; set; } = true;

	public bool VideosEnabled { get; set; } = true;

	public bool IsLegacy { get; set; }

	public int SortOrder { get; set; }

	public List<MediaFolderRule> FolderRules { get; set; } = new List<MediaFolderRule>();
}

public sealed class MediaFolderRule
{
	public string RelativeFolderPath { get; set; } = "";

	public bool? IsIncluded { get; set; }
}

public sealed class MediaItemRecord
{
	public string MediaId { get; set; } = "";

	public string RelativePath { get; set; } = "";

	public string SourceId { get; set; } = "";

	public MediaKind Kind { get; set; }

	public string FullPath { get; set; } = "";

	public string FileName { get; set; } = "";

	public long SizeBytes { get; set; }

	public long LastWriteUtcTicks { get; set; }

	public string QuickFingerprint { get; set; } = "";
}

public sealed class MediaIdentityRecord
{
	public string MediaId { get; set; } = Guid.NewGuid().ToString("N");

	public string SourceId { get; set; } = "";

	public MediaKind Kind { get; set; }

	public string CurrentRelativePath { get; set; } = "";

	public List<string> KnownRelativePaths { get; set; } = new List<string>();

	public string FileName { get; set; } = "";

	public long SizeBytes { get; set; }

	public long LastWriteUtcTicks { get; set; }

	public string QuickFingerprint { get; set; } = "";

	public string Tags { get; set; } = "";

	public bool IsMissing { get; set; }
}

public sealed class MediaIdentityStore
{
	public List<MediaIdentityRecord> Items { get; set; } = new List<MediaIdentityRecord>();
}

public sealed class LegacyTagImportResult
{
	public int LinesRead { get; set; }

	public int ImportedCount { get; set; }

	public int ExactMatches { get; set; }

	public int AliasMatches { get; set; }

	public int FingerprintMatches { get; set; }

	public int FilenameMatches { get; set; }

	public int SkippedExistingCount { get; set; }

	public int AmbiguousCount { get; set; }

	public int UnmatchedCount { get; set; }

	public string ReportPath { get; set; } = "";

	public List<string> Details { get; set; } = new List<string>();
}

public sealed class BulkMoveResult
{
	public int RequestedCount { get; set; }

	public int MovedCount { get; set; }

	public int SkippedCount { get; set; }

	public int RenamedCount { get; set; }
}

public sealed class MediaCatalogSnapshot
{
	public List<MediaSourceDefinition> Sources { get; } = new List<MediaSourceDefinition>();

	public List<MediaItemRecord> Items { get; } = new List<MediaItemRecord>();

	public Dictionary<string, MediaIdentityRecord> IdentitiesByPath { get; } = new Dictionary<string, MediaIdentityRecord>(StringComparer.OrdinalIgnoreCase);

	public Dictionary<string, MediaIdentityRecord> IdentitiesById { get; } = new Dictionary<string, MediaIdentityRecord>(StringComparer.OrdinalIgnoreCase);

	public IEnumerable<MediaItemRecord> ActiveItems => Items.FindAll(delegate(MediaItemRecord item)
	{
		MediaSourceDefinition source = Sources.Find(delegate(MediaSourceDefinition candidate)
		{
			return candidate.Id == item.SourceId;
		});
		if (source == null || !source.IsEnabled)
		{
			return false;
		}
		if (item.Kind == MediaKind.Image || item.Kind == MediaKind.Gif)
		{
			return source.ImagesEnabled;
		}
		return source.VideosEnabled;
	});
}
