using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Win32;

namespace OpenEdge;

public partial class CompatibilityToolsPage : Page, IComponentConnector
{
	private readonly CompatibilityStateService compatibilityStateService;

	private readonly SettingsRegistry settingsRegistry;

	public CompatibilityToolsPage(CompatibilityStateService compatibilityStateService, SettingsRegistry settingsRegistry)
	{
		this.compatibilityStateService = compatibilityStateService;
		this.settingsRegistry = settingsRegistry;
		InitializeComponent();
		RefreshSummary();
	}

	private void RefreshSummary()
	{
		CompatibilityStateSummary summary = compatibilityStateService.GetSummary();
		SummaryText.Text = "Compatibility state file: " + RuntimePaths.CompatibilityStateFile + "\nLegacy flags directory: " + RuntimePaths.FlagsDir + "\nBackups: " + RuntimePaths.CompatibilityBackupsDir + "\nTransfer packages: " + RuntimePaths.CompatibilityTransfersDir;
		StatusText.Text = "Persistent entries tracked: " + summary.PersistentEntryCount + "\nLegacy flag files found: " + summary.LegacyFlagFileCount + "\nCompatibility state exists: " + summary.StateFileExists + "\nLast legacy import (UTC): " + (summary.LastLegacyImportUtc?.ToString("u") ?? "not yet imported");
	}

	private void Migrate_Click(object sender, RoutedEventArgs e)
	{
		compatibilityStateService.MigrateCurrentRuntimeState(createBackup: true);
		RefreshSummary();
		MessageBox.Show("Legacy persistent state was imported into the compatibility registry and a backup snapshot was created.", "OpenEdge", MessageBoxButton.OK, MessageBoxImage.Information);
	}

	private void Export_Click(object sender, RoutedEventArgs e)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog
		{
			Filter = "Compatibility package (*.json)|*.json",
			InitialDirectory = RuntimePaths.CompatibilityTransfersDir,
			FileName = "openedge-compatibility-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".json"
		};
		if (saveFileDialog.ShowDialog() == true)
		{
			compatibilityStateService.ExportTransferPackage(saveFileDialog.FileName, createBackup: false);
			RefreshSummary();
			MessageBox.Show("Compatibility package exported successfully.", "OpenEdge", MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}

	private void ExportDiagnostics_Click(object sender, RoutedEventArgs e)
	{
		Directory.CreateDirectory(RuntimePaths.DebugDir);
		string filePath = Path.Combine(RuntimePaths.DebugDir, "compatibility-diagnostics-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".md");
		File.WriteAllText(filePath, BuildDiagnosticsReport(), Encoding.UTF8);
		MessageBox.Show("Diagnostics exported to:\n" + filePath, "OpenEdge", MessageBoxButton.OK, MessageBoxImage.Information);
	}

	private string BuildDiagnosticsReport()
	{
		CompatibilityStateSummary summary = compatibilityStateService.GetSummary();
		StringBuilder builder = new StringBuilder();
		builder.AppendLine("# OpenEdge Compatibility Diagnostics");
		builder.AppendLine();
		builder.AppendLine("Generated: " + DateTime.Now.ToString("u"));
		builder.AppendLine();
		builder.AppendLine("## Compatibility state");
		builder.AppendLine();
		builder.AppendLine("- State file: `" + RuntimePaths.CompatibilityStateFile + "`");
		builder.AppendLine("- Persistent entries: " + summary.PersistentEntryCount);
		builder.AppendLine("- Legacy flag files: " + summary.LegacyFlagFileCount);
		builder.AppendLine("- State file exists: " + summary.StateFileExists);
		builder.AppendLine("- Last legacy import UTC: " + (summary.LastLegacyImportUtc?.ToString("u") ?? "not yet imported"));
		builder.AppendLine();
		builder.AppendLine("## Canonical setting aliases");
		builder.AppendLine();
		foreach (SettingDefinition definition in settingsRegistry.GetDefinitions().Where(HasAlias).OrderBy((SettingDefinition item) => item.Key, StringComparer.OrdinalIgnoreCase))
		{
			builder.AppendLine("- `" + definition.Key + "`: " + string.Join(", ", BuildAliasParts(definition)));
		}
		builder.AppendLine();
		builder.AppendLine("## Media tag stores");
		builder.AppendLine();
		builder.AppendLine("- Primary identity index: `" + Path.Combine(RuntimePaths.RuntimeRoot, "media-tag-index.json") + "` exists=" + File.Exists(Path.Combine(RuntimePaths.RuntimeRoot, "media-tag-index.json")));
		builder.AppendLine("- Legacy tags mirror: `" + RuntimePaths.TagsFile + "` lines=" + (File.Exists(RuntimePaths.TagsFile) ? File.ReadAllLines(RuntimePaths.TagsFile).Length : 0));
		builder.AppendLine();
		builder.AppendLine("## Script migration diagnostics");
		builder.AppendLine();
		builder.AppendLine("Run `powershell -ExecutionPolicy Bypass -File docs/recovery/audit-legacy-state.ps1` from the repository root for exact remaining legacy command locations.");
		return builder.ToString();
	}

	private static bool HasAlias(SettingDefinition definition)
	{
		return !string.IsNullOrWhiteSpace(definition.LegacyEnabledFlag) || !string.IsNullOrWhiteSpace(definition.LegacyDisabledFlag) || !string.IsNullOrWhiteSpace(definition.LegacyValueKey) || definition.RelatedLegacyKeys.Count > 0;
	}

	private static IEnumerable<string> BuildAliasParts(SettingDefinition definition)
	{
		if (!string.IsNullOrWhiteSpace(definition.LegacyEnabledFlag)) yield return "enabled=" + definition.LegacyEnabledFlag;
		if (!string.IsNullOrWhiteSpace(definition.LegacyDisabledFlag)) yield return "disabled=" + definition.LegacyDisabledFlag;
		if (!string.IsNullOrWhiteSpace(definition.LegacyValueKey)) yield return "value=" + definition.LegacyValueKey;
		foreach (string relatedLegacyKey in definition.RelatedLegacyKeys) yield return "related=" + relatedLegacyKey;
	}

	private void Import_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			Filter = "Compatibility package (*.json)|*.json",
			InitialDirectory = RuntimePaths.CompatibilityTransfersDir
		};
		if (openFileDialog.ShowDialog() == true)
		{
			compatibilityStateService.ImportTransferPackage(openFileDialog.FileName, createBackup: true);
			RefreshSummary();
			MessageBox.Show("Compatibility package imported and mirrored back into the legacy file layout.", "OpenEdge", MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}

	private void Refresh_Click(object sender, RoutedEventArgs e)
	{
		RefreshSummary();
	}

	private void Back_Click(object sender, RoutedEventArgs e)
	{
		NavigationService?.GoBack();
	}
}
