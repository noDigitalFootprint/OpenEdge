using System;
using System.Globalization;

namespace OpenEdge;

// Compatibility boundary for old FLAG/DELFLAG/SETVAR script commands and old file-backed state.
// New code should prefer SettingsRegistry/structured state APIs; keep legacy promotion rules here.
public sealed class LegacyStateAdapter
{
	private readonly CompatibilityStateService compatibilityStateService;
	private readonly SettingsRegistry settingsRegistry;

	public LegacyStateAdapter(CompatibilityStateService compatibilityStateService, SettingsRegistry settingsRegistry)
	{
		this.compatibilityStateService = compatibilityStateService ?? throw new ArgumentNullException(nameof(compatibilityStateService));
		this.settingsRegistry = settingsRegistry ?? throw new ArgumentNullException(nameof(settingsRegistry));
	}

	public bool HasFlag(string flagName)
	{
		return compatibilityStateService.PersistentEntryExists(flagName);
	}

	public bool HasAnsweredFlag(string flagName)
	{
		return compatibilityStateService.PersistentEntryExists(flagName) || compatibilityStateService.PersistentEntryExists(flagName + "No");
	}

	public double GetFlagAgeSeconds(string flagName)
	{
		string persistentValue = compatibilityStateService.GetPersistentValue(flagName);
		if (string.IsNullOrWhiteSpace(persistentValue))
		{
			return 0.0;
		}
		double seconds = -1.0;
		try
		{
			seconds = (int)(DateTime.Now - DateTime.Parse(persistentValue, CultureInfo.CurrentCulture)).TotalSeconds;
		}
		catch
		{
		}
		if (seconds < 0.0)
		{
			compatibilityStateService.SetPersistentValue(flagName, DateTime.Now.ToString());
			return 0.0;
		}
		return seconds;
	}

	public string GetVar(string name, string defaultValue)
	{
		string persistentValue = compatibilityStateService.GetPersistentValue(name);
		if (persistentValue != null)
		{
			return persistentValue == "" ? defaultValue : persistentValue;
		}
		SetVar(name, defaultValue);
		return defaultValue;
	}

	public void SetVar(string name, string value)
	{
		compatibilityStateService.SetPersistentValue(name, value);
		if (TryApplyLegacyValueSet(name, value))
		{
			SessionTraceLogger.Info("legacy-state", "legacy value promoted key=" + name);
		}
		else
		{
			SessionTraceLogger.Info("legacy-state", "legacy value passthrough key=" + name);
		}
	}

	public void SetFlag(string flagName)
	{
		compatibilityStateService.SetPersistentValue(flagName, DateTime.Now.ToString());
		bool promoted = settingsRegistry.TryApplyLegacyFlagSet(flagName);
		settingsRegistry.DequeueAsk(flagName);
		if (flagName.EndsWith("No", StringComparison.OrdinalIgnoreCase))
		{
			settingsRegistry.DequeueAsk(flagName.Substring(0, flagName.Length - 2));
		}
		SessionTraceLogger.Info("legacy-state", promoted ? "legacy flag promoted key=" + flagName : "legacy flag passthrough key=" + flagName);
	}

	public void DeleteFlag(string flagName)
	{
		compatibilityStateService.DeletePersistentValue(flagName);
		bool handled = settingsRegistry.TryApplyLegacyFlagDelete(flagName);
		SessionTraceLogger.Info("legacy-state", handled ? "legacy delete handled key=" + flagName : "legacy delete passthrough key=" + flagName);
	}

	public bool TryApplyLegacyValueSet(string legacyValueKey, string value)
	{
		foreach (SettingDefinition definition in settingsRegistry.GetDefinitions())
		{
			if (string.Equals(definition.LegacyValueKey, legacyValueKey, StringComparison.OrdinalIgnoreCase))
			{
				settingsRegistry.SetRawValue(definition.Key, value);
				return true;
			}
		}
		return false;
	}
}
