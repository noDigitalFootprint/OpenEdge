using System;

namespace OpenEdge;

public static class ScriptSettingPredicateEvaluator
{
	public static bool TryEvaluate(
		string token,
		Func<string, bool> isSettingEnabled,
		Func<string, bool> isSettingAnswered,
		Func<string, bool> isSettingDeclined,
		Func<string, int, int> getSettingValue,
		Func<string, int> parseExpectedValue,
		out bool result)
	{
		result = true;
		if (string.IsNullOrWhiteSpace(token))
		{
			return false;
		}

		if (TryReadKey(token, "ISSETTING:", out string key))
		{
			result = isSettingEnabled(key);
			return true;
		}
		if (TryReadKey(token, "ISNOSETTING:", out key))
		{
			result = !isSettingEnabled(key);
			return true;
		}
		if (TryReadKey(token, "ISSETTINGASKED:", out key))
		{
			result = isSettingAnswered(key);
			return true;
		}
		if (TryReadKey(token, "ISSETTINGDECLINED:", out key))
		{
			result = isSettingDeclined(key);
			return true;
		}
		if (TryReadKey(token, "ISNOSETTINGASKED:", out key))
		{
			result = !isSettingAnswered(key);
			return true;
		}
		if (TryReadKey(token, "ISNOSETTINGDECLINED:", out key))
		{
			result = !isSettingDeclined(key);
			return true;
		}
		if (TryReadKey(token, "SETTINGVALUEATLEAST:", out string valueExpression))
		{
			string[] parts = valueExpression.Split(new char[1] { ',' }, 2);
			if (parts.Length != 2)
			{
				result = false;
				return true;
			}
			result = getSettingValue(parts[0].Trim(), 0) >= parseExpectedValue(parts[1].Trim());
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
}
