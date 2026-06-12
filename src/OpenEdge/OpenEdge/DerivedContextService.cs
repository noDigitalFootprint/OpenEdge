using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace OpenEdge;

public static class DerivedContextService
{
	private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true,
		WriteIndented = true
	};

	public static IReadOnlyList<DerivedContextDefinition> LoadEnabledContexts()
	{
		List<DerivedContextDefinition> contexts = new List<DerivedContextDefinition>();
		contexts.AddRange(LoadCoreContexts());
		contexts.AddRange(ModService.GetEnabledContextDefinitions());
		return contexts.Where((DerivedContextDefinition context) => !string.IsNullOrWhiteSpace(context.Key)).ToList();
	}

	public static bool IsContextActive(MainWindow mw, string contextKey)
	{
		DerivedContextDefinition context = FindContext(contextKey);
		return context != null && IsContextActive(mw, context);
	}

	public static string GetContextMediaTags(string contextKey)
	{
		DerivedContextDefinition context = FindContext(contextKey);
		if (context == null || context.MediaTags == null)
		{
			return "";
		}
		return string.Join(",", context.MediaTags.Where((string tag) => !string.IsNullOrWhiteSpace(tag)).Select((string tag) => tag.Trim()));
	}

	private static DerivedContextDefinition FindContext(string contextKey)
	{
		return LoadEnabledContexts().FirstOrDefault((DerivedContextDefinition context) => string.Equals(context.Key, contextKey.Trim(), StringComparison.OrdinalIgnoreCase));
	}

	private static bool IsContextActive(MainWindow mw, DerivedContextDefinition context)
	{
		foreach (string setting in context.Settings ?? new List<string>())
		{
			if (!mw.isSettingEnabled(setting.Trim()))
			{
				return false;
			}
		}

		string mediaTags = string.Join(",", (context.MediaTags ?? new List<string>()).Where((string tag) => !string.IsNullOrWhiteSpace(tag)).Select((string tag) => tag.Trim()));
		if (!string.IsNullOrWhiteSpace(mediaTags))
		{
			int minimumMedia = context.MinimumMedia <= 0 ? 1 : context.MinimumMedia;
			if (!mw.atLeastXMedia(mediaTags, minimumMedia))
			{
				return false;
			}
		}

		return true;
	}

	private static List<DerivedContextDefinition> LoadCoreContexts()
	{
		Directory.CreateDirectory(RuntimePaths.ContextsDir);
		if (!File.Exists(RuntimePaths.ContextsFile))
		{
			return new List<DerivedContextDefinition>();
		}
		try
		{
			DerivedContextsFile contextsFile = JsonSerializer.Deserialize<DerivedContextsFile>(File.ReadAllText(RuntimePaths.ContextsFile), JsonOptions);
			return contextsFile?.Contexts ?? new List<DerivedContextDefinition>();
		}
		catch
		{
			return new List<DerivedContextDefinition>();
		}
	}
}
