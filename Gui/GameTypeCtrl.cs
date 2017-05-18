using UnityEngine;
using System.Collections;

public enum AppGameType
{
	Null,
	DanJiTanKe,
	DanJiFeiJi,
	LianJiTanKe,
	LianJiFeiJi,
	LianJiServer,
}

public class GameTypeCtrl : MonoBehaviour {
	//public AppGameType AppType = AppGameType.Null;
	public GameObject NetCtrlObj;
	public static AppGameType AppTypeStatic = AppGameType.Null;
	public static bool IsServer;
	public static GameTypeCtrl Instance;
	string ServerIp = "";
	public static PlayerEnum PlayerPCState = PlayerEnum.Null;
	//true -> TestClientPort. false -> TestServerPort.
	public bool IsTestClientPort = true;
	public bool IsTestWorkNet;
	void Awake()
	{
		Instance = this;
		if (pcvr.bIsHardWare) {
			ServerIp = NetworkServerNet.ServerPortIP;
		}
		else {
			ServerIp = HandleJson.GetInstance().ReadFromFilePathXml(NetworkServerNet.IpFile, "SERVER_IP");
			if (ServerIp == null) {
				ServerIp = "192.168.0.2";
				HandleJson.GetInstance().WriteToFilePathXml(NetworkServerNet.IpFile, "SERVER_IP", ServerIp);
			}
		}

		if (ServerIp == Network.player.ipAddress) {
			//server port -> 1P.
			if (IsTestWorkNet) {
				if (IsTestClientPort) {
					PlayerPCState = PlayerEnum.PlayerTwo;
				}
				else {
					PlayerPCState = PlayerEnum.PlayerOne;
				}
			}
			else {
				PlayerPCState = PlayerEnum.PlayerOne;
			}
			XKMasterServerCtrl.CheckMasterServerIP();
		}
		else {
			//client port -> 2P.
			PlayerPCState = PlayerEnum.PlayerTwo;
		}
		SetAppTypeVal(AppGameType.LianJiServer);

		if (AppTypeStatic == AppGameType.LianJiServer) {
			//IsServer = true;
			DelayCheckServerIP();
		}
	}

	public static void SetAppTypeVal(AppGameType val)
	{
		AppTypeStatic = val;
		switch (val) {
		case AppGameType.LianJiServer:
			IsServer = true;
			break;
		default:
			IsServer = false;
			break;
		}
	}

//	void Update()
//	{
//		if (Time.frameCount % 200 == 0) {
//			System.GC.Collect();
//			if (GameMovieCtrl.PlayerIPInfo == NetworkServerNet.ServerPortIP
//			    && XKCheckGameServerIP.IsCloseCmd
//			    && !Screen.fullScreen) {
//				//ResetFullScreen...
//				DelayResetFullScreen();
//			}
//		}
//	}

	public void DelayCheckServerIP()
	{
		if (!pcvr.bIsHardWare) {
			return;
		}
		Invoke("CheckServerPortIP", 5f);
		//Invoke("DelayCloseCmd", 15f);
	}

	void CheckServerPortIP()
	{
		if (Network.player.ipAddress == NetworkServerNet.ServerPortIP) {
			//XKCheckGameServerIP.RestartGame();
			return;
		}

		XKCheckGameServerIP.CheckServerIP();
	}

	public void DelayResetFullScreen()
	{
		CancelInvoke("ResetFullScreen");
		Invoke("ResetFullScreen", 3f);
	}

	void ResetFullScreen()
	{
		Debug.Log("ResetFullScreen...");
	}

	void DelayCloseCmd()
	{
		XKCheckGameServerIP.CloseCmd();
	}
}