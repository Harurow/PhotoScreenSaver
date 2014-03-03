using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace PhotoScreenSaver.Lib
{
	public static class NativeMethods
	{
		[DllImport("user32.dll")]
		public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out Int32Rect lpRect);
	}
}