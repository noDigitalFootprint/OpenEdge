using System.IO;

namespace OpenEdge;

// Compatibility boundary for short-lived FLAGT/DELFLAGT session markers.
// Temp/session flags intentionally do not promote into canonical settings.
public sealed class SessionFlagStore
{
	public bool Exists(string flagName)
	{
		return File.Exists(RuntimePaths.TempFlag(flagName));
	}

	public void Set(string flagName, string value = "True")
	{
		string path = RuntimePaths.TempFlag(flagName);
		Directory.CreateDirectory(Path.GetDirectoryName(path) ?? RuntimePaths.FlagsDir);
		File.WriteAllText(path, value);
	}

	public void Delete(string flagName)
	{
		string path = RuntimePaths.TempFlag(flagName);
		if (File.Exists(path))
		{
			File.Delete(path);
		}
	}
}
