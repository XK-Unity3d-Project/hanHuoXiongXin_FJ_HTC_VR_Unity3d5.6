using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

public class FullScreen : MonoBehaviour
{
    private const string UnityWindowClassName = "UnityWndClass";

    [DllImport("kernel32.dll")]
    static extern uint GetCurrentThreadId();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);


    const uint SWP_SHOWWINDOW = 0x0040;
    const int GWL_STYLE = -16;
	const int WS_BORDER = 1;
	private IntPtr windowHandle = IntPtr.Zero;

    void Awake()
    {
#if !UNITY_EDITOR
		uint threadId = GetCurrentThreadId();
		EnumThreadWindows(threadId, (hWnd, lParam) =>
		                  {
			var classText = new StringBuilder(UnityWindowClassName.Length + 1);
			GetClassName(hWnd, classText, classText.Capacity);
			if (classText.ToString() != UnityWindowClassName) return true;
			windowHandle = hWnd;
			return false;
		}, IntPtr.Zero);
		if(windowHandle != IntPtr.Zero)
		{
			SetWindowLong(windowHandle, GWL_STYLE, WS_BORDER);
			bool result = SetWindowPos(windowHandle, 0, 0, 0, Screen.width , Screen.height, SWP_SHOWWINDOW);
		}
#endif
    }
}
