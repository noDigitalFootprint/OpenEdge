using System.Runtime.InteropServices;

namespace OpenEdge;

public static class WindowsServices
{
	private const int WS_EX_TRANSPARENT = 32;

	private const int GWL_EXSTYLE = -20;

	[DllImport("user32.dll")]
	private static extern int GetWindowLong(nint hwnd, int index);

	[DllImport("user32.dll")]
	private static extern int SetWindowLong(nint hwnd, int index, int newStyle);

	public static void SetWindowExTransparent(nint hwnd)
	{
		int windowLong = GetWindowLong(hwnd, -20);
		SetWindowLong(hwnd, -20, windowLong | 0x20);
	}

	public static void SetWindowUndoExTransparent(nint hwnd)
	{
		int windowLong = GetWindowLong(hwnd, -20);
		SetWindowLong(hwnd, -20, windowLong & -33);
	}
}
