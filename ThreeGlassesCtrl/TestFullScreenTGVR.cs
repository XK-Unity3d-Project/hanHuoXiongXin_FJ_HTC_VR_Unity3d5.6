using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Text;
using ThreeGlasses;

public class TestFullScreenTGVR : MonoBehaviour {

	// Use this for initialization
	void Awake()
	{
		//Screen.SetResolution(1920, 1080, false);
		#if !UNITY_EDITOR
		TestAAMakeGameFullScreenView();
		//Invoke("TestAAMakeGameFullScreenView", 3f);
		//ThreeGlassesInterfaces.SetPositionAndResolution();
		#endif
	}

	[DllImport("user32")]
	static extern IntPtr SetWindowLong (IntPtr hwnd, int _nIndex , int dwNewLong);  
	[DllImport("user32")]  
	static extern bool SetWindowPos (IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);  
	[DllImport("user32")]  
	static extern IntPtr GetForegroundWindow ();
	[DllImport("user32")]
	static extern bool GetWindowRect(IntPtr hWnd, ref Rect rect);
	[DllImport("user32")]
	static extern int GetWindowLong(IntPtr hWnd, int nIndex);
	[DllImport("user32")]
	static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);
	[DllImport("user32")]
	static extern int GetSystemMetrics(int nIndex);
	[DllImport("user32")]
	static extern int SetForegroundWindow(IntPtr hwnd);
	[DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
	static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
	[DllImport("user32")]
	static extern bool SetMenu(IntPtr hWnd, IntPtr hMenu);
	
	const int GWL_STYLE = -16;  
	const int WS_BORDER = 1;  
	const int WS_POPUP = 0x800000;
	const int WS_SYSMENU = 0x80000;
	const int SWP_NOSIZE = 0x0001;
	const int SWP_NOMOVE = 0x0002;
	const uint SW_SHOWNORMAL = 1;
	const int HWND_NOTOPMOST = 0xffffffe;
	
	const int WS_CAPTION = (int)0x00C00000; 
	const int WS_CHILD = (int)0x40000000; 
	
	const uint SWP_SHOWWINDOW = 0x0040;
	const uint SWP_HIDEWINDOW = 0x0080;
	const uint SWP_DRAWFRAME = 0x0020;
	const uint SWP_DEFERERASE = 0x2000;
	const uint SWP_FRAMECHANGED = 0x0020;
	const int HWND_TOPMOST = -1;
	const int HWND_TOP = 0;
	const int HWND_BOTTOM = 1;
	int SM_CXSCREEN = 0;
	int SM_CYSCREEN = 1;
	
	[DllImport("kernel32.dll")]
	static extern uint GetCurrentThreadId();
	
	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
	
	public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
	
	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);
	
	private const string UnityWindowClassName = "UnityWndClass";
	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
	
	private IntPtr windowHandle = IntPtr.Zero;

	void TestAAMakeGameFullScreenView()
	{
		uint threadId = GetCurrentThreadId();
		EnumThreadWindows(threadId, (hWnd, lParam) =>
		{
			var classText = new StringBuilder(UnityWindowClassName.Length + 1);
			GetClassName(hWnd, classText, classText.Capacity);
			if (classText.ToString() != UnityWindowClassName) {
				return true;
			}
			windowHandle = hWnd;
			return false;
		}, IntPtr.Zero);

		//windowHandle = FindWindow(null, "HelicopterVR");
		if (windowHandle != IntPtr.Zero) {
			int iScreenW = GetSystemMetrics(SM_CXSCREEN);
			int iScreenH = GetSystemMetrics(SM_CYSCREEN);
			SetWindowLong(windowHandle, GWL_STYLE, WS_BORDER);
			SetWindowPos(windowHandle, HWND_TOP, 0, 0, iScreenW , iScreenH, SWP_SHOWWINDOW);
			Invoke("SetGameWindowToTop", 3f);
		}
	}

	void SetGameWindowToTop()
	{
		IntPtr hwnd = FindWindow(null, "HelicopterVR");
		SetForegroundWindow(hwnd);
	}

	void TestMakeGameFullScreenView()
	{
//		if (IsTestXiaoScreen) {
//			return;
//		}

		IntPtr m_hWnd = GetForegroundWindow();
		
//		IntPtr pParentWndSave = new IntPtr(); //父窗口句柄.
//		IntPtr pParentWndSaveTmp = new IntPtr(); //父窗口句柄.
		int dwWindowStyleSave = 0; //窗口风格
//		Rect rcWndRectSave = new Rect(0, 0, 0, 0); //窗口位置.
		
		int iScreenW = GetSystemMetrics(SM_CXSCREEN);
		int iScreenH = GetSystemMetrics(SM_CYSCREEN);
		
		dwWindowStyleSave = GetWindowLong(m_hWnd, GWL_STYLE); //保存窗口风格.
		
//		GetWindowRect(m_hWnd, ref rcWndRectSave); //保存窗口位置.

//		pParentWndSave = SetParent(m_hWnd, pParentWndSaveTmp); //保存父窗口句柄/设置父窗口.
		
		SetWindowLong(m_hWnd, GWL_STYLE,
		              dwWindowStyleSave & (~WS_CHILD) & (~WS_CAPTION) & (~WS_BORDER));//使窗口不具有CAPTION风格.
		
		//uint SWP = SWP_DRAWFRAME | SWP_DEFERERASE | SWP_FRAMECHANGED | SWP_SHOWWINDOW;
		uint SWP = SWP_SHOWWINDOW;
		//SetWindowPos(m_hWnd, HWND_BOTTOM, 0, 0, iScreenW, iScreenH, SWP);
		SetWindowPos(m_hWnd, HWND_TOP, 0, 0, iScreenW, iScreenH, SWP);
		
		IntPtr ptr = new IntPtr();
		m_hWnd = new IntPtr();
		SetMenu(m_hWnd, ptr); //取消边框.
	}
}