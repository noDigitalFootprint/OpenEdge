# Tag Identity System in OpenEdge

## Overview

OpenEdge now uses a dual-store tag system to maintain backward compatibility while enabling more resilient tagging that survives file moves and renames within configured media sources.

### Stores
1. **Primary Store**: `runtime/local/app/media-tag-index.json`
   - Identity-backed tag records
   - Survives moves/renames within the same source
   - Contains metadata used for matching (size, timestamp, quick fingerprint)

2. **Compatibility Mirror**: `runtime/local/app/tags.txt`
   - Legacy format: `<path>kljnrbkrbasxalkxmbt<tags>`
   - Updated whenever tags change in the primary store
   - Maintains compatibility with external tools (e.g., Python tagger)

## Identity Records

Each media item is represented by a `MediaIdentityRecord` containing:

| Field | Purpose |
|-------|---------|
| `MediaId` | Unique GUID assigned when first seen |
| `CurrentRelativePath` | Latest known path within the source |
| `KnownRelativePaths` | Historical paths this identity has been associated with |
| `Tags` | Comma-separated tag string |
| `SourceId` | ID of the media source containing the item |
| `Kind` | Image/Gif/Video |
| `FileName` | Filename (used for fallback matching) |
| `SizeBytes` | File size (used for matching) |
| `LastWriteUtcTicks` | Last write timestamp (used for matching) |
| `QuickFingerprint` | SHA-256 of first/last 64KB + size (used for matching) |
| `IsMissing` | True if identity exists but file not found in current scan |

## Binding Process

On catalog reload (`MediaCatalogService.Reload()`):

1. **Load Sources** from `media-sources.json`
2. **Scan Items** recursively through enabled sources
3. **Load Identities** from `media-tag-index.json`
4. **Bind Identities to Items** using this priority:
   - Exact path match (`IdentitiesByPath`)
   - Same source + same filename + same size
   - Same kind + same size + same last write time
   - Quick fingerprint match (when multiple candidates above)
   - Create new identity if no match found
5. **Apply Legacy Tags** from `tags.txt`:
   - Exact path match in identity store
   - Alias match (legacy path in `KnownRelativePaths`)
   - Filename fallback (if only one item with that name in source)
6. **Save Updates**:
   - Updated identities to `media-tag-index.json`
   - Legacy mirror to `tags.txt`

## Tag Operations

### Setting Tags (`MediaCatalogService.SetTags`)
```csharp
public void SetTags(string relativePath, string tags)
{
    // Get or create identity for path
    if (!snapshot.IdentitiesByPath.TryGetValue(relativePath, out MediaIdentityRecord identity))
    {
        // Create new identity
        identity = new MediaIdentityRecord { /* ... */ };
        snapshot.IdentitiesByPath[relativePath] = identity;
        snapshot.IdentitiesById[identity.MediaId] = identity;
    }
    
    // Update tags
    identity.Tags = string.IsNullOrWhiteSpace(tags) ? "" : tags;
    
    // Persist to both stores
    SaveLegacyTags();   // Updates tags.txt
    SaveIdentityStore(); // Updates media-tag-index.json
}
```

### Getting Tags (`MediaCatalogService.GetTags`)
```csharp
public string GetTags(string relativePath)
{
    return snapshot.IdentitiesByPath.TryGetValue(relativePath, out var identity)
        ? identity.Tags
        : "";
}
```

## Migration Rules

### From Legacy System
- Existing `tags.txt` is imported on first reload after enabling the new system
- Each legacy entry creates or updates an identity record
- Import uses the same legacy tag resolution logic:
  1. Exact path match
  2. Alias match via `KnownRelativePaths`
  3. Filename fallback (if unambiguous)
- After import, `tags.txt` continues to be updated as a mirror

### To New System
- No manual migration required
- System automatically maintains both stores
- External tools can continue using `tags.txt`
- New development should use the media catalog API (`GetTags`, `SetTags`)

## Resilience Characteristics

### Survives:
- File renames within same source
- File moves within same source directory tree
- Timestamp updates (if size and content similar enough for fingerprint match)
- Source enable/disable toggles (identities preserved)

### Limitations:
- Moving between sources requires new identity (SourceId is part of match key)
- Significant content changes may break fingerprint matching
- Simultaneous rename of multiple files with same size may cause ambiguity

## Performance Notes
- Identity store loaded once at startup
- Binding uses dictionary lookups (O(1)) for exact paths
- Fallback matching scans candidate lists (typically small)
- Quick fingerprint adds minimal overhead (reads first/last 64KB)

## File Format: media-tag-index.json
```json
{
  "Items": [
    {
      "MediaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "SourceId": "legacy-images",
      "Kind": 0,
      "CurrentRelativePath": "\\images\\photo.jpg",
      "KnownRelativePaths": [
        "\\images\\photo.jpg",
        "\\images\\old\\photo.jpg"
      ],
      "Tags": "nature,landscape",
      "FileName": "photo.jpg",
      "SizeBytes": 245760,
      "LastWriteUtcTicks": 132147360000000000,
      "QuickFingerprint": "a1b2c3d4e5f6...",
      "IsMissing": false
    }
  ]
}
```