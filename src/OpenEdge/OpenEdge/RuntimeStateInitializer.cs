using System;
using System.IO;

namespace OpenEdge;

public static class RuntimeStateInitializer
{
	public static void EnsureFirstRunState()
	{
		Directory.CreateDirectory(RuntimePaths.AudioDir);
		Directory.CreateDirectory(RuntimePaths.DebugDir);
		Directory.CreateDirectory(RuntimePaths.FlagsDir);
		Directory.CreateDirectory(RuntimePaths.TempFlagsDir);
		Directory.CreateDirectory(RuntimePaths.ImagesDir);
		Directory.CreateDirectory(RuntimePaths.VideosDir);
		Directory.CreateDirectory(RuntimePaths.ResourcesDir);
		Directory.CreateDirectory(RuntimePaths.LinesDir);
		Directory.CreateDirectory(RuntimePaths.ModsDir);
		Directory.CreateDirectory(RuntimePaths.ContextsDir);
		Directory.CreateDirectory(RuntimePaths.CompatibilityBackupsDir);
		Directory.CreateDirectory(RuntimePaths.CompatibilityTransfersDir);

		CreateFileIfMissing(RuntimePaths.TasksFile, "");
		CreateFileIfMissing(RuntimePaths.TagGroupsFile, string.Join(Environment.NewLine, new string[5]) + Environment.NewLine);
	}

	private static void CreateFileIfMissing(string path, string content)
	{
		if (File.Exists(path))
		{
			return;
		}
		string directoryName = Path.GetDirectoryName(path);
		if (!string.IsNullOrWhiteSpace(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		File.WriteAllText(path, content ?? "");
	}
}
