using System;

namespace OpenEdge;

public static class ScriptStructuredPredicateEvaluator
{
	public static bool TryEvaluate(
		string token,
		Func<string, bool> isAnalStage,
		Func<string, bool> isAnalPreference,
		Func<bool> hasCompletedFirstAnalSession,
		Func<bool> hasDeclinedAnalTraining,
		Func<bool> hasActiveAnalTraining,
		Func<bool> isPetPlayAdvancedDeclined,
		Func<bool> isPetPlayAdvancedEnabled,
		Func<string, bool> isOutsideSessionRuleActive,
		Func<bool> isLobRuntimeEnabled,
		Func<string, bool> hasSettingText,
		out bool result)
	{
		result = true;
		if (string.IsNullOrWhiteSpace(token))
		{
			return false;
		}

		if (TryReadKey(token, "ANALSTAGE:", out string key))
		{
			result = isAnalStage(key);
			return true;
		}
		if (TryReadKey(token, "ANALPREFERENCE:", out key))
		{
			result = isAnalPreference(key);
			return true;
		}
		if (IsToken(token, "ISANALFIRSTDONE"))
		{
			result = hasCompletedFirstAnalSession();
			return true;
		}
		if (IsToken(token, "ISNOANALFIRSTDONE"))
		{
			result = !hasCompletedFirstAnalSession();
			return true;
		}
		if (IsToken(token, "ISANALTRAININGDECLINED"))
		{
			result = hasDeclinedAnalTraining();
			return true;
		}
		if (IsToken(token, "ISANALTRAINING"))
		{
			result = hasActiveAnalTraining();
			return true;
		}
		if (IsToken(token, "ISPETPLAYADVANCEDDECLINED"))
		{
			result = isPetPlayAdvancedDeclined();
			return true;
		}
		if (IsToken(token, "ISNOPETPLAYADVANCEDDECLINED"))
		{
			result = !isPetPlayAdvancedDeclined();
			return true;
		}
		if (IsToken(token, "ISPETPLAYADVANCED"))
		{
			result = isPetPlayAdvancedEnabled();
			return true;
		}
		if (IsToken(token, "ISNOPETPLAYADVANCED"))
		{
			result = !isPetPlayAdvancedEnabled();
			return true;
		}
		if (TryReadKey(token, "ISOUTSIDERULEACTIVE:", out key))
		{
			result = isOutsideSessionRuleActive(key);
			return true;
		}
		if (TryReadKey(token, "ISNOOUTSIDERULEACTIVE:", out key))
		{
			result = !isOutsideSessionRuleActive(key);
			return true;
		}
		if (IsToken(token, "ISLOBRUNTIMEON"))
		{
			result = isLobRuntimeEnabled();
			return true;
		}
		if (TryReadKey(token, "SETTINGTEXTSET:", out key))
		{
			result = hasSettingText(key);
			return true;
		}
		if (TryReadKey(token, "SETTINGTEXTEMPTY:", out key))
		{
			result = !hasSettingText(key);
			return true;
		}
		return false;
	}

	private static bool TryReadKey(string token, string prefix, out string key)
	{
		if (token.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
		{
			key = token.Substring(prefix.Length).Trim();
			return true;
		}
		key = "";
		return false;
	}

	private static bool IsToken(string token, string expected)
	{
		return string.Equals(token.Trim(), expected, StringComparison.OrdinalIgnoreCase);
	}
}
