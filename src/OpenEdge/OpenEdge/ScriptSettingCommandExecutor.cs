using System;

namespace OpenEdge;

public sealed class ScriptSettingCommandExecutor
{
	private readonly Action<string, bool> setSettingEnabled;
	private readonly Action<string, int> setSettingValue;
	private readonly Action<string, string> setSettingText;
	private readonly Action<string> setAnalStage;
	private readonly Action<string> setAnalPreference;
	private readonly Action<string> setAnalTraining;
	private readonly Action<string> setPetPersona;
	private readonly Action<string, int> setOutsideRule;
	private readonly Action<int, int> setLobWindow;
	private readonly Action<int> setCuckStage;
	private readonly Action<bool> setCuckFriday;
	private readonly Action<string, string> setChastityState;
	private readonly Action<int> setCensorIntensity;
	private readonly Action<int> setBreathTime;
	private readonly Func<string, string> interpretValue;

	public ScriptSettingCommandExecutor(
		Action<string, bool> setSettingEnabled,
		Action<string, int> setSettingValue,
		Action<string, string> setSettingText,
		Action<string> setAnalStage,
		Action<string> setAnalPreference,
		Action<string> setAnalTraining,
		Action<string> setPetPersona,
		Action<string, int> setOutsideRule,
		Action<int, int> setLobWindow,
		Func<string, string> interpretValue,
		Action<int> setCuckStage = null,
		Action<bool> setCuckFriday = null,
		Action<string, string> setChastityState = null,
		Action<int> setCensorIntensity = null,
		Action<int> setBreathTime = null)
	{
		this.setSettingEnabled = setSettingEnabled;
		this.setSettingValue = setSettingValue;
		this.setSettingText = setSettingText;
		this.setAnalStage = setAnalStage;
		this.setAnalPreference = setAnalPreference;
		this.setAnalTraining = setAnalTraining;
		this.setPetPersona = setPetPersona;
		this.setOutsideRule = setOutsideRule;
		this.setLobWindow = setLobWindow;
		this.interpretValue = interpretValue;
		this.setCuckStage = setCuckStage;
		this.setCuckFriday = setCuckFriday;
		this.setChastityState = setChastityState;
		this.setCensorIntensity = setCensorIntensity;
		this.setBreathTime = setBreathTime;
	}

	public bool TryExecute(string command)
	{
		if (string.IsNullOrWhiteSpace(command))
		{
			return false;
		}
		if (TryReadPayload(command, "SETSETTING:", out string payload))
		{
			string[] parts = payload.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length < 2)
			{
				return true;
			}
			string key = parts[0].Trim();
			string value = FirstToken(Interpret(parts[1].Trim()));
			bool enabled = IsEnabledValue(value);
			SessionTraceLogger.Info("state-write", "set setting " + key + "=" + enabled);
			setSettingEnabled(key, enabled);
			return true;
		}
		if (TryReadPayload(command, "SETSETTINGVALUE:", out payload))
		{
			string[] parts = payload.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length < 2)
			{
				return true;
			}
			string key = parts[0].Trim();
			string value = Interpret(parts[1].Trim());
			if (int.TryParse(FirstToken(value), out int numericValue))
			{
				SessionTraceLogger.Info("state-write", "set setting value " + key + "=" + numericValue);
				setSettingValue(key, numericValue);
			}
			else
			{
				SessionTraceLogger.Info("state-write", "set setting text " + key + "=" + value);
				setSettingText(key, value);
			}
			return true;
		}
		if (TryReadPayload(command, "SETANALSTAGE:", out payload))
		{
			string value = Interpret(payload.Trim());
			SessionTraceLogger.Info("state-write", "set anal stage " + value);
			setAnalStage(value);
			return true;
		}
		if (TryReadPayload(command, "SETANALPREFERENCE:", out payload))
		{
			string value = Interpret(payload.Trim());
			SessionTraceLogger.Info("state-write", "set anal preference " + value);
			setAnalPreference(value);
			return true;
		}
		if (TryReadPayload(command, "SETANALTRAINING:", out payload))
		{
			string value = Interpret(payload.Trim());
			SessionTraceLogger.Info("state-write", "set anal training " + value);
			setAnalTraining(value);
			return true;
		}
		if (TryReadPayload(command, "SETPETPERSONA:", out payload))
		{
			string value = Interpret(payload.Trim());
			SessionTraceLogger.Info("state-write", "set pet persona " + value);
			setPetPersona(value);
			return true;
		}
		if (TryReadPayload(command, "SETOUTSIDERULE:", out payload))
		{
			string[] parts = payload.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length < 2)
			{
				return true;
			}
			string key = parts[0].Trim();
			if (int.TryParse(FirstToken(Interpret(parts[1].Trim())), out int value))
			{
				SessionTraceLogger.Info("state-write", "set outside rule " + key + "=" + value);
				setOutsideRule(key, value);
			}
			return true;
		}
		if (TryReadPayload(command, "SETLOBWINDOW:", out payload))
		{
			string[] parts = payload.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length < 2)
			{
				return true;
			}
			if (int.TryParse(FirstToken(Interpret(parts[0].Trim())), out int earlyHour) && int.TryParse(FirstToken(Interpret(parts[1].Trim())), out int lateHour))
			{
				SessionTraceLogger.Info("state-write", "set lob window " + earlyHour + "," + lateHour);
				setLobWindow(earlyHour, lateHour);
			}
			return true;
		}
		if (TryReadPayload(command, "SETCUCKSTAGE:", out payload))
		{
			if (int.TryParse(FirstToken(Interpret(payload.Trim())), out int stage))
			{
				SessionTraceLogger.Info("state-write", "set cuck stage " + stage);
				setCuckStage?.Invoke(stage);
			}
			return true;
		}
		if (TryReadPayload(command, "SETCUCKFRIDAY:", out payload))
		{
			bool passed = IsEnabledValue(FirstToken(Interpret(payload.Trim())));
			SessionTraceLogger.Info("state-write", "set cuck friday " + passed);
			setCuckFriday?.Invoke(passed);
			return true;
		}
		if (TryReadPayload(command, "SETCHASTITYSTATE:", out payload))
		{
			string[] parts = payload.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length >= 2)
			{
				string key = parts[0].Trim();
				string value = Interpret(parts[1].Trim());
				SessionTraceLogger.Info("state-write", "set chastity state " + key + "=" + value);
				setChastityState?.Invoke(key, value);
			}
			return true;
		}
		if (TryReadPayload(command, "SETCENSORINTENSITY:", out payload))
		{
			if (int.TryParse(FirstToken(Interpret(payload.Trim())), out int intensity))
			{
				SessionTraceLogger.Info("state-write", "set censor intensity " + intensity);
				setCensorIntensity?.Invoke(intensity);
			}
			return true;
		}
		if (TryReadPayload(command, "SETBREATHTIME:", out payload))
		{
			if (int.TryParse(FirstToken(Interpret(payload.Trim())), out int seconds))
			{
				SessionTraceLogger.Info("state-write", "set breath time " + seconds);
				setBreathTime?.Invoke(seconds);
			}
			return true;
		}
		return false;
	}

	private string Interpret(string value)
	{
		return interpretValue == null ? value : interpretValue(value);
	}

	private static string FirstToken(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return "";
		}
		return value.Trim().Split(new char[0], StringSplitOptions.RemoveEmptyEntries)[0];
	}

	private static bool IsEnabledValue(string value)
	{
		return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "on", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase) || value == "1";
	}

	private static bool TryReadPayload(string command, string prefix, out string payload)
	{
		if (command.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
		{
			payload = command.Substring(prefix.Length);
			return true;
		}
		payload = "";
		return false;
	}
}
