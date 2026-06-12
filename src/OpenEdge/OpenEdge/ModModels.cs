using System.Collections.Generic;

namespace OpenEdge;

public sealed class ModManifest
{
	public string Id { get; set; } = "";

	public string Name { get; set; } = "";

	public string Author { get; set; } = "";

	public string Version { get; set; } = "";

	public bool Enabled { get; set; } = true;
}

public sealed class ModSummary
{
	public string RootPath { get; init; } = "";

	public string Id { get; init; } = "";

	public string Name { get; init; } = "";

	public string Author { get; init; } = "";

	public string Version { get; init; } = "";

	public bool Enabled { get; init; }

	public int SettingCount { get; init; }

	public int TagCount { get; init; }

	public int ContextCount { get; init; }

	public bool HasLines { get; init; }

	public string Error { get; set; } = "";
}

public sealed class LoadedMod
{
	public string RootPath { get; init; } = "";

	public ModManifest Manifest { get; init; } = new ModManifest();

	public List<SettingDefinition> Settings { get; init; } = new List<SettingDefinition>();

	public List<ModTagDefinition> Tags { get; init; } = new List<ModTagDefinition>();

	public List<DerivedContextDefinition> Contexts { get; init; } = new List<DerivedContextDefinition>();

	public string LinesRootPath { get; init; } = "";
}

public sealed class ModSettingsFile
{
	public List<ModSettingDefinition> Settings { get; set; } = new List<ModSettingDefinition>();
}

public sealed class ModSettingDefinition
{
	public string Key { get; set; } = "";

	public string Label { get; set; } = "";

	public string Kind { get; set; } = "Toggle";

	public string Group { get; set; } = "";

	public string Description { get; set; } = "";

	public string Warning { get; set; } = "";

	public string LegacyEnabledFlag { get; set; } = "";

	public string LegacyDisabledFlag { get; set; } = "";

	public string LegacyValueKey { get; set; } = "";

	public bool QueueableAsk { get; set; }

	public List<string> MediaDiscoveryTags { get; set; } = new List<string>();

	public int MediaDiscoveryMinimum { get; set; } = 2;
}

public sealed class ModTagsFile
{
	public List<ModTagDefinition> Tags { get; set; } = new List<ModTagDefinition>();
}

public sealed class ModContextsFile
{
	public List<DerivedContextDefinition> Contexts { get; set; } = new List<DerivedContextDefinition>();
}

public sealed class DerivedContextsFile
{
	public List<DerivedContextDefinition> Contexts { get; set; } = new List<DerivedContextDefinition>();
}

public sealed class DerivedContextDefinition
{
	public string Key { get; set; } = "";

	public string Label { get; set; } = "";

	public List<string> Settings { get; set; } = new List<string>();

	public List<string> MediaTags { get; set; } = new List<string>();

	public int MinimumMedia { get; set; } = 2;
}

public sealed class ModTagDefinition
{
	public string Key { get; set; } = "";

	public string Label { get; set; } = "";

	public string Group { get; set; } = "Custom";
}
