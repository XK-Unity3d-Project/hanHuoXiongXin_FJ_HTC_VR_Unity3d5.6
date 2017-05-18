using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class TestOpenCloseScreen : MonoBehaviour {
	private const uint WM_SYSCOMMAND = 0x0112; //系统消息.
	private const uint SC_MONITORPOWER = 0xF170; //关闭显示器的系统命令.
	private const int SC_CLOSE = 2;  //2 为关闭. 1为省电状态.
	private const int SC_OPEN = -1; //-1为开机.
	//广播消息，所有顶级窗体都会接收.
	private static readonly IntPtr Handle = new IntPtr(0xffff);
	[DllImport("user32.dll")]
	public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, uint wParam, int lParam);
	int TestIndex = 0;
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.P)) {
			if (TestIndex > 1) {
				TestIndex = -1;
			}

			switch (TestIndex) {
			case 0:
				CloseScreen();
				break;
			case 1:
				OpenScreen();
				break;
			}
			TestIndex++;
		}
	}

	private void CloseScreen()
	{
		Debug.Log("CloseScreen...");
		SendMessage(Handle, WM_SYSCOMMAND, SC_MONITORPOWER, SC_CLOSE); //关闭显示器.
	}
	
	private void OpenScreen()
	{
		Debug.Log("OpenScreen...");
		SendMessage(Handle, WM_SYSCOMMAND, SC_MONITORPOWER, SC_OPEN); //打开显示器.
	}
}