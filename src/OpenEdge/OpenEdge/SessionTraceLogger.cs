using System;
using System.Diagnostics;
using System.IO;

namespace OpenEdge;

public static class SessionTraceLogger
{
	private static readonly object LogLock = new object();

	public static string LogFile => Path.Combine(RuntimePaths.DebugDir, "session-trace.log");

	public static void Reset(string reason)
	{
		try
		{
			Directory.CreateDirectory(RuntimePaths.DebugDir);
			lock (LogLock)
			{
				File.WriteAllText(LogFile, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " [INFO] logger - reset: " + reason + Environment.NewLine);
			}
		}
		catch
		{
		}
	}

	public static void Info(string category, string message)
	{
		Write("INFO", category, message, null);
	}

	public static void Error(string category, string message, Exception exception = null)
	{
		Write("ERROR", category, message, exception);
	}

	public static void Memory(string category, string message = "")
	{
		long managedBytes = GC.GetTotalMemory(forceFullCollection: false);
		long privateBytes = 0;
		try
		{
			privateBytes = Process.GetCurrentProcess().PrivateMemorySize64;
		}
		catch
		{
		}
		Write("MEMORY", category, message + " managed=" + FormatBytes(managedBytes) + " private=" + FormatBytes(privateBytes), null);
	}

	private static void Write(string level, string category, string message, Exception exception)
	{
		try
		{
			Directory.CreateDirectory(RuntimePaths.DebugDir);
			string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " [" + level + "] " + category + " - " + message;
			if (exception != null)
			{
				line += " | " + exception.GetType().Name + ": " + exception.Message;
			}
			lock (LogLock)
			{
				File.AppendAllText(LogFile, line + Environment.NewLine);
			}
		}
		catch
		{
		}
	}

	private static string FormatBytes(long bytes)
	{
		if (bytes <= 0)
		{
			return "0 MB";
		}
		return Math.Round(bytes / 1024.0 / 1024.0, 1) + " MB";
	}
}
