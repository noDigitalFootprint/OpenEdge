using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenEdge;

public sealed class MediaTagCompatibilityDiagnostics
{
	public int LegacyLinesRead { get; set; }

	public int ExactPathMatches { get; set; }

	public int AliasMatches { get; set; }

	public int ResolverMatches { get; set; }

	public int UnmatchedClaims { get; set; }

	public int LegacyLinesWritten { get; set; }
}

public delegate bool LegacyTagClaimResolver(MediaCatalogSnapshot snapshot, string legacyPath, string tags, out MediaIdentityRecord matchedIdentity);

// Compatibility boundary for legacy tags.txt path-based claims.
// MediaCatalogService remains canonical; this adapter imports/mirrors old tag storage.
public sealed class MediaTagCompatibilityAdapter
{
	private readonly string tagsFilePath;
	private readonly string legacyTagSeparator;

	public MediaTagCompatibilityAdapter(string tagsFilePath, string legacyTagSeparator)
	{
		this.tagsFilePath = tagsFilePath;
		this.legacyTagSeparator = legacyTagSeparator;
	}

	public MediaTagCompatibilityDiagnostics LastDiagnostics { get; private set; } = new MediaTagCompatibilityDiagnostics();

	public void LoadLegacyTags(MediaCatalogSnapshot targetSnapshot, LegacyTagClaimResolver resolver)
	{
		MediaTagCompatibilityDiagnostics diagnostics = new MediaTagCompatibilityDiagnostics();
		if (!File.Exists(tagsFilePath))
		{
			LastDiagnostics = diagnostics;
			return;
		}
		foreach (string line in File.ReadAllLines(tagsFilePath))
		{
			diagnostics.LegacyLinesRead++;
			string[] parts = line.Split(new string[1] { legacyTagSeparator }, StringSplitOptions.None);
			if (parts.Length < 2)
			{
				diagnostics.UnmatchedClaims++;
				continue;
			}
			string legacyPath = parts[0];
			string tags = parts[1];
			if (targetSnapshot.IdentitiesByPath.TryGetValue(legacyPath, out MediaIdentityRecord exactIdentity))
			{
				exactIdentity.Tags = tags;
				diagnostics.ExactPathMatches++;
				continue;
			}
			MediaIdentityRecord aliasIdentity = targetSnapshot.IdentitiesById.Values.FirstOrDefault(delegate(MediaIdentityRecord candidate)
			{
				return candidate.KnownRelativePaths.Contains(legacyPath, StringComparer.OrdinalIgnoreCase);
			});
			if (aliasIdentity != null)
			{
				aliasIdentity.Tags = tags;
				diagnostics.AliasMatches++;
				continue;
			}
			if (resolver != null && resolver(targetSnapshot, legacyPath, tags, out MediaIdentityRecord matchedIdentity))
			{
				matchedIdentity.Tags = tags;
				if (!matchedIdentity.KnownRelativePaths.Contains(legacyPath, StringComparer.OrdinalIgnoreCase))
				{
					matchedIdentity.KnownRelativePaths.Add(legacyPath);
				}
				diagnostics.ResolverMatches++;
				continue;
			}
			diagnostics.UnmatchedClaims++;
			SessionTraceLogger.Info("media-tags", "unmatched legacy tag claim path=" + legacyPath);
		}
		LastDiagnostics = diagnostics;
		SessionTraceLogger.Info("media-tags", "legacy import lines=" + diagnostics.LegacyLinesRead + " exact=" + diagnostics.ExactPathMatches + " alias=" + diagnostics.AliasMatches + " resolved=" + diagnostics.ResolverMatches + " unmatched=" + diagnostics.UnmatchedClaims);
	}

	public void SaveLegacyTags(MediaCatalogSnapshot targetSnapshot)
	{
		LastDiagnostics.LegacyLinesWritten = 0;
		SessionTraceLogger.Info("media-tags", "legacy tags.txt mirror is read-only; skipped write");
	}
}
