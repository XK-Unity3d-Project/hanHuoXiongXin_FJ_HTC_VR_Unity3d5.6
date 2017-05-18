using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum ServerPassword
{
	Null,
	XKMovie,
	XKGame
}

public class NetworkServerNet : MonoBehaviour {
	public static string ServerPortIP = "192.168.0.2";
	public static string ip = "192.168.0.2";
	public static int ServerPortVal = 1000;
	public static int ClientPortVal = 1001;
	float TimeCheckNpcList;
	bool bIsLinkServer = true;
	bool IsActiveClient;
	bool bIsDisconnect = false;
	public static int ServerSendState = 0;
	public static string IpFile = "GameIP.info";
	List<GameObject> NpcObjList;
	float TimeCheckClientNetState;
	bool IsOnDisconnectedFromServer;
	bool IsDisconnectedServer;
	float TimeCheckServerNetState;
	float TimeValLoadingLevel;
	bool IsLoadingLevelCheck;
	bool IsClientExit;
	public static NetCtrl NetCtrlScript;
	private static NetworkServerNet _Instance;
	public static NetworkServerNet GetInstance()
	{
		if (GameTypeCtrl.AppTypeStatic  == AppGameType.Null
		    || GameTypeCtrl.AppTypeStatic  == AppGameType.DanJiFeiJi
		    || GameTypeCtrl.AppTypeStatic  == AppGameType.DanJiTanKe) {
			return null;
		}

		if (_Instance == null) {
			GameObject obj = new GameObject("_NetworkServerNet");
			DontDestroyOnLoad(obj);
			_Instance = obj.AddComponent<NetworkServerNet>();
			
			//RequestMasterServer.GetInstance();
		}
		return _Instance;
	}

	void Awake()
	{
		IsFindMasterServer = false;
		if (pcvr.bIsHardWare) {
			ip = ServerPortIP;
		}
		else {
			ip = HandleJson.GetInstance().ReadFromFilePathXml(IpFile, "SERVER_IP");
			if (ip == null) {
				ip = "192.168.0.2";
				HandleJson.GetInstance().WriteToFilePathXml(IpFile, "SERVER_IP", ip);
			}
		}
		NpcObjList = new List<GameObject>();
		HostServerIP = ip;

		MasterServer.ipAddress = ip;
		Network.natFacilitatorIP = ip;
		LoopRequestHostListFromMasterServer();
		//RequestHostListFromMasterServer();
	}

	void OnGUI()
	{
		string strA = "IsActiveClient "+IsActiveClient+", bIsLinkServer "+bIsLinkServer;
		GUI.Box(new Rect(10f, Screen.height - 25f, Screen.width, 25f), strA);
		if (!XkGameCtrl.IsShowDebugInfoBox) {
			return;
		}

		if (SceneManager.GetActiveScene().buildIndex != (int)GameLevel.Movie) {
			return;
		}
		GUI.Box(new Rect(0f, 0f, 300f, 30f), "isServer "+Network.isServer);
		GUI.Box(new Rect(0f, 30f, 300f, 30f), "isClient "+Network.isClient);
		GUI.Box(new Rect(0f, 60f, 300f, 30f), "AppType "+GameTypeCtrl.AppTypeStatic);
		GUI.Box(new Rect(0f, 90f, 300f, 30f), "connectionsLen "+Network.connections.Length);
		GUI.Box(new Rect(0f, 120f, 300f, 30f), "CountLinkServer "+NetCtrl.SelectLinkCount);
	}

	static string GameTypeName = "MyUniqueGameType";
	void LoopRequestHostListFromMasterServer()
	{
		CancelInvoke("RequestHostListFromMasterServer");
		InvokeRepeating("RequestHostListFromMasterServer", 0f, 1f);
	}

	void RequestHostListFromMasterServer()
	{
		//MasterServer.ClearHostList();
		MasterServer.RequestHostList(GameTypeName);
	}

	public void AddNpcObjList(GameObject obj)
	{
		if (NpcObjList.Contains(obj)) {
			return;
		}
		NpcObjList.Add(obj);
	}

	public void RemoveNpcObjList(GameObject obj)
	{
		if (!NpcObjList.Contains(obj)) {
			return;
		}
		NpcObjList.Remove(obj);
	}

	void CheckNpcObjList()
	{
		float dTime = Time.realtimeSinceStartup - TimeCheckNpcList;
		if (dTime < 0.1f) {
			return;
		}
		TimeCheckNpcList = Time.realtimeSinceStartup;

		if (NpcObjList == null) {
			return;
		}

		int max = NpcObjList.Count;
		for (int i = 0; i < max; i++) {
			if (NpcObjList[i] == null) {
				NpcObjList.RemoveAt(i);
				break;
			}
		}
	}

	public void ClearAllNetSpawnObj()
	{
		int max = NpcObjList.Count;
		GameObject[] ObjArray = new GameObject[max];
		for (int i = 0; i < max; i++) {
			ObjArray[i] = NpcObjList[i];
		}
		
		for (int i = 0; i < max; i++) {
			if (null != ObjArray[i]) {
				RemoveNpcObjList(ObjArray[i]);
				Network.Destroy(ObjArray[i]);
			}
		}
	}

	void TestSpawnNpc()
	{
		if (!Network.isServer) {
			return;
		}

		int playerID = int.Parse(Network.player.ToString());
		Transform tran = NetCtrlScript.TestNpc.transform;
		TestNpcObj = (GameObject)Network.Instantiate(NetCtrlScript.TestNpc, tran.position, tran.rotation, playerID);
		AddNpcObjList(TestNpcObj);
	}

	GameObject TestNpcObj;
	void TestRemoveNpc()
	{
		if (TestNpcObj == null) {
			return;
		}
		RemoveNpcObjList(TestNpcObj);
		Network.Destroy(TestNpcObj);
	}

	public void RemoveNetworkObj(GameObject netObj, float timeVal = 0f)
	{
		StartCoroutine(DelayRemoveNetworkObj(netObj, timeVal));
	}

	IEnumerator DelayRemoveNetworkObj(GameObject netObj, float timeVal)
	{
		yield return new WaitForSeconds(timeVal);
		if (netObj != null) {
			RemoveNpcObjList(netObj);
			NetworkView netViewCom = netObj.GetComponent<NetworkView>();
			if (netViewCom != null && Network.peerType != NetworkPeerType.Disconnected) {
				Network.Destroy(netObj);
			}
			else {
				Destroy(netObj);
			}
		}

	}

	void TestRemovePlayerRpc()
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}

		int max = Network.connections.Length;
		for (int i = 0; i < max; i++) {
			Network.RemoveRPCs(Network.connections[i]);
			Network.DestroyPlayerObjects(Network.connections[i]);
		}
	}

	void Update()
	{
		if (!pcvr.bIsHardWare) {
			if (SceneManager.GetActiveScene().buildIndex == (int)GameLevel.Movie) {
				if (Input.GetKeyUp(KeyCode.O)) {
					TestSpawnNpc();
				}
				
				if (Input.GetKeyUp(KeyCode.L)) {
					MakeClientDisconnect();
				}
				
				if (Input.GetKeyUp(KeyCode.C)) {
					//ClearAllNetSpawnObj();
					TryToLinkServer();
				}

				if (Input.GetKeyUp(KeyCode.M)) {
					MakeServerDisconnect(); //test
				}
			}
			else {
				if (Input.GetKeyUp(KeyCode.B)) {
					TryToCloseServerPort(); //test
				}
			}
		}
		CheckNpcObjList();

		if (GameTypeCtrl.AppTypeStatic  == AppGameType.Null
		    || GameTypeCtrl.AppTypeStatic  == AppGameType.DanJiFeiJi
		    || GameTypeCtrl.AppTypeStatic  == AppGameType.DanJiTanKe) {
			return;
		}

		if (!XkGameCtrl.IsLoadingLevel) {
			if (Network.peerType == NetworkPeerType.Server && IsCheckServerPortPlayerNum) {
				CheckServerPortPlayerNum(0);
			}
//			else if (XkGameCtrl.GameModeVal == GameMode.LianJi) {
//				CheckClientPortNetState(0);
//			}
		}

		if (Time.realtimeSinceStartup - TimeLastNet < 1f) {
			return;
		}

		switch(Network.peerType)
		{
		case NetworkPeerType.Disconnected:
			StartCreat();
			TryToConnectServerPort();
			break;

		case NetworkPeerType.Server:
			OnServer();
			break;

		case NetworkPeerType.Client:
			OnClient();
			break;

		case NetworkPeerType.Connecting:
			break;
		}
	}

	public void TryToCreateServer()
	{
		if (Network.peerType != NetworkPeerType.Disconnected){
			return;
		}
		
		if (bIsLinkServer) {
			return;
		}
		//ScreenLog.Log("try to create server...");
		bIsLinkServer = true;
	}

	public void TryToLinkServer()
	{
		if (Network.peerType != NetworkPeerType.Disconnected){
			return;
		}

		if (SceneManager.GetActiveScene().buildIndex == (int)(GameLevel.Movie)) {
			if (!bIsLinkServer && IsActiveClient) {
				IsActiveClient = false;
			}
		}

		if (bIsLinkServer || IsActiveClient) {
			return;
		}
		ScreenLog.Log("try to link server...");
		bIsLinkServer = true;
		IsActiveClient = true;
	}

	public static string HostNameP1 = "PlayerOne Game";
	public static string HostNameP2 = "PlayerTwo Game";
	public static string HostNameServer = "PlayGameServer";
	void StartCreat()
	{
//		if (SceneManager.GetActiveScene().buildIndex != (int)GameLevel.Movie) {
//			return;
//		}

		if (!bIsLinkServer) {
			return;
		}
		bIsLinkServer = false;

		if (GameModeCtrl.ModeVal != GameMode.LianJi) {
			return;
		}

		if (GameTypeCtrl.IsServer) {
			NetworkConnectionError error = NetworkConnectionError.CreateSocketOrThreadFailure;
			//Debug.Log("PlayerPCState "+GameTypeCtrl.PlayerPCState);
			switch (GameTypeCtrl.PlayerPCState) {
			case PlayerEnum.PlayerOne:
				error = Network.InitializeServer(30, ServerPortVal, true);
				MasterServer.RegisterHost(GameTypeName, HostNameP1, Network.player.ipAddress);
				break;
			case PlayerEnum.PlayerTwo:
				error = Network.InitializeServer(30, ClientPortVal, true);
				MasterServer.RegisterHost(GameTypeName, HostNameP2, Network.player.ipAddress);
				break;
			}
			Network.incomingPassword = ServerPassword.XKMovie.ToString();

//			if (SceneManager.GetActiveScene().buildIndex == (int)GameLevel.Movie) {
//				Network.incomingPassword = ServerPassword.XKMovie.ToString();
//			}
//			else {
//				Network.incomingPassword = ServerPassword.XKGame.ToString();
//			}
			ScreenLog.Log("start create to server, Password "+Network.incomingPassword);

			//ScreenLog.Log("NetworkServerNet -> current level is " + SceneManager.GetActiveScene().buildIndexName);
			ScreenLog.Log("creat server: info is " + error);
			if (error.ToString() != "NoError") {
				bIsLinkServer = true;
			}
		}
		else {
			if (!IsActiveClient) {
				return;
			}

			string passwordVal = ServerPassword.XKMovie.ToString();
//			if (SceneManager.GetActiveScene().buildIndex > (int)GameLevel.Movie) {
//				passwordVal = ServerPassword.XKGame.ToString();
//			}
			ScreenLog.Log("start connect to server, password "+passwordVal);
			switch (GameTypeCtrl.PlayerPCState) {
			case PlayerEnum.PlayerOne:
				Network.Connect(HostServerIP, ClientPortVal, passwordVal);
				break;
			case PlayerEnum.PlayerTwo:
				Network.Connect(ip, ServerPortVal, passwordVal);
				break;
			}
		}
	}
	public static string HostServerIP = "";

	IEnumerator TestSpawnClientPlayer()
	{
		if (!Network.isClient) {
			yield break;
		}

		bool isStop = false;
		do {
			if (NetCtrlScript != null) {
				isStop = true;
				int playerID = int.Parse(Network.player.ToString());
				Transform tran = NetCtrlScript.TestPlayer.transform;
				TestNpcObj = (GameObject)Network.Instantiate(NetCtrlScript.TestPlayer, tran.position, tran.rotation, playerID);
				AddNpcObjList(TestNpcObj);
			}
			else {
				yield return new WaitForSeconds(1f);
			}
		} while (!isStop);
	}

	public void SpawnNetPlayerObj(GameObject playerPrefab,
	                                   AiPathCtrl pathScript,
	                                   Vector3 pos,
	                                   Quaternion rot)
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			return;
		}

		int playerID = int.Parse(Network.player.ToString());
		GameObject obj = (GameObject)Network.Instantiate(playerPrefab, pos, rot, playerID);
		XkPlayerCtrl playerScript = obj.GetComponent<XkPlayerCtrl>();
		playerScript.SetAiPathScript(pathScript);
		AddNpcObjList(obj);
	}

	void OnConnectedToServer()
	{
		//当客户端链接上服务器，在此用来产生玩家的角色.
		//StartCoroutine(TestSpawnClientPlayer()); //test
		IsActiveClient = false;
	}

	void DelaySetIsLinkServer()
	{
		bIsLinkServer = true;
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{
		ScreenLog.Log("Could not connect to server: " + error);
		CancelInvoke("DelaySetIsLinkServer");
		Invoke("DelaySetIsLinkServer", 2f);
	}

	float TimeLastNet;
	void OnServer()
	{
		if(!bIsDisconnect) {
			return;
		}
		TimeLastNet = Time.realtimeSinceStartup;
		bIsLinkServer = true;
		bIsDisconnect = false;
		Network.Disconnect();
		MasterServer.UnregisterHost();
	}

	void OnClient()
	{
		if(!bIsDisconnect) {
			return;
		}
		TimeLastNet = Time.realtimeSinceStartup;
		bIsLinkServer = false;
		bIsDisconnect = false;
		Network.Disconnect(0);
		MasterServer.UnregisterHost();
	}

	void OnApplicationQuit()
	{
		Debug.Log("OnApplicationQuit...NetServer");
		XkGameCtrl.IsGameOnQuit = true;
		if (Network.peerType == NetworkPeerType.Server) {
			//ClearAllNetSpawnObj();
		}
		else if (Network.peerType == NetworkPeerType.Client) {
			MakeClientDisconnect();
		}
		XKMasterServerCtrl.CloseMasterServer();
	}

	void OnServerInitialized()
	{
		Debug.Log("OnServerInitialized...");
		if (!IsCheckServerPortPlayerNum) {
			Invoke("SetIsCheckServerPortPlayerNum", 10f);
		}

		if (NetCtrlScript == null) {
			int playerID = int.Parse(Network.player.ToString());
			Transform tran = GameTypeCtrl.Instance.NetCtrlObj.transform;
			GameObject obj = (GameObject)Network.Instantiate(GameTypeCtrl.Instance.NetCtrlObj, tran.position, tran.rotation, playerID);
			NetCtrlScript = obj.GetComponent<NetCtrl>();
			AddNpcObjList(obj);
		}
		SetServerSendState(0);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		ScreenLog.Log("OnPlayerDisconnected -> stop send msg! playerIP " + player.ipAddress);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
		SetServerSendState(0);
	}

	void CheckServerPortPlayerNum(int key)
	{
		if (Time.realtimeSinceStartup - TimeCheckServerNetState < 0.3f && key == 0) {
			return;
		}
		TimeCheckServerNetState = Time.realtimeSinceStartup;

		int playerNum = Network.connections.Length;
		if (playerNum > 0) {
			IsClientExit = false;
			return;
		}

		if (IsClientExit) {
			return;
		}
		IsClientExit = true;

		if (XkGameCtrl.IsLoadingLevel) {
			if (!IsLoadingLevelCheck) {
				IsLoadingLevelCheck = true;
				TimeValLoadingLevel = Time.realtimeSinceStartup;
			}
			else {
				if (Time.realtimeSinceStartup - TimeValLoadingLevel > 20f) {
					IsLoadingLevelCheck = false;
					XkGameCtrl.IsLoadingLevel = false;
				}
			}
			return;
		}

		if (SceneManager.GetActiveScene().buildIndex < (int)GameLevel.Scene_1
		    || SceneManager.GetActiveScene().buildIndex > (int)GameLevel.Scene_4) {
			return;
		}
		//XkGameCtrl.HiddenMissionCleanup();

		Debug.Log("CheckServerPortPlayerNum...");
		NetCtrl.SelectLinkCount = 0;
		if (!IsInvoking("DelayServerLoadingGameMovie")) {
			Invoke("DelayServerLoadingGameMovie", 1f);
		}
	}

	void DelayServerLoadingGameMovie()
	{
		XkGameCtrl.LoadingGameMovie();
	}

	public static void SetServerSendState(int val)
	{
		//Debug.Log("SetServerSendState -> val "+val);
		ServerSendState = val;
	}

	public void MakeClientDisconnect()
	{
		if (Network.peerType != NetworkPeerType.Client) {
			return;
		}

		if (IsInvoking("DelayMakeClientDisconnect")) {
			return;
		}

		if (NetCtrl.GetInstance() != null) {
			NetCtrl.GetInstance().CloseServerPortSendRPC();
		}
		Invoke("DelayMakeClientDisconnect", 1f);
	}
	
	void DelayMakeClientDisconnect()
	{
		if (bIsDisconnect) {
			return;
		}
		bIsDisconnect = true;
		ClearAllNetSpawnObj();
	}

	bool IsCheckServerPortPlayerNum = true;
	public void SetIsCheckServerPortPlayerNum()
	{
		IsCheckServerPortPlayerNum = true;
	}

	bool IsTryCloseServerPort;
	public void TryToCloseServerPort()
	{
		if (IsTryCloseServerPort) {
			return;
		}
		IsTryCloseServerPort = true;

		if (SceneManager.GetActiveScene().buildIndex == (int)(GameLevel.Scene_4)) {
		//if (SceneManager.GetActiveScene().buildIndex == XkGameCtrl.TestGameEndLv) { //test
			Debug.Log("TryToCloseServerPort...");
			SetIsCheckServerPortPlayerNum();
			return;
		}

		IsCheckServerPortPlayerNum = false;
		if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiServer) {
			NetCtrl.MaxPlayerCount = Network.connections.Length;
		}
		MakeServerDisconnect();
	}

	void CheckIsTryCloseServerPort()
	{
		if (!IsTryCloseServerPort) {
			return;
		}
		IsTryCloseServerPort = false;
		TryToCreateServer();
	}

	public void MakeServerDisconnect()
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		
		if (IsInvoking("DelayMakeServerDisconnect")) {
			return;
		}
		NetCtrl.GetInstance().CloseClientPortSendRPC();
		Invoke("DelayMakeServerDisconnect", 2f);
	}
	
	void DelayMakeServerDisconnect()
	{
		if (bIsDisconnect) {
			return;
		}
		bIsDisconnect = true;
		ClearAllNetSpawnObj();
		Invoke("CheckIsTryCloseServerPort", 2f);
	}

	float TimeDisconnectedVal;
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Debug.Log("OnDisconnectedFromServer -> info is "+info+", AppTypeStatic "+GameTypeCtrl.AppTypeStatic);
		if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiServer) {

		}
		else {
			IsOnDisconnectedFromServer = true;
			SetServerSendState(0);
			if (SceneManager.GetActiveScene().buildIndex == (int)(GameLevel.Scene_4)) {
			//if (SceneManager.GetActiveScene().buildIndex == XkGameCtrl.TestGameEndLv) { //test
				CheckClientPortNetState(1);
			}
		}
		TimeDisconnectedVal = Time.realtimeSinceStartup;
	}

	void TryToConnectServerPort()
	{
		if (GameModeCtrl.ModeVal != GameMode.LianJi) {
			return;
		}

		if (SceneManager.GetActiveScene().buildIndex < (int)(GameLevel.Scene_1)
		    || SceneManager.GetActiveScene().buildIndex >= (SceneManager.sceneCountInBuildSettings - 1)) {
			return;
		}

		if (Time.realtimeSinceStartup - TimeDisconnectedVal >= 10f) {
			CheckClientPortNetState(1);
			return;
		}

		if (Time.frameCount % 5 == 0) {
			TryToLinkServer();
		}
	}

	void CheckClientPortNetState(int key)
	{
		if (Time.realtimeSinceStartup - TimeCheckClientNetState < 1f && key == 0) {
			return;
		}
		TimeCheckClientNetState = Time.realtimeSinceStartup;

		if (!IsOnDisconnectedFromServer) {
			return;
		}

		if (XkGameCtrl.IsLoadingLevel) {
			return;
		}

		if (SceneManager.GetActiveScene().buildIndex < (int)GameLevel.Scene_1
		    || SceneManager.GetActiveScene().buildIndex > (int)GameLevel.Scene_4) {
			return;
		}
		IsOnDisconnectedFromServer = false;
		Debug.Log("CheckClientPortNetState......IsLoadingLevel "+XkGameCtrl.IsLoadingLevel+", key "+key);
		Invoke("DelayClientLoadingGameMovie", 5f);
	}

	void DelayClientLoadingGameMovie()
	{
		XkGameCtrl.LoadingGameMovie(1);
	}

	public static bool IsFindMasterServer;
	public static void ChangeMasterServerHostName(PlayerEnum playerSt)
	{
		string hostName = "";
		switch (playerSt) {
		case PlayerEnum.PlayerOne:
			hostName = HostNameP1;
			break;
		case PlayerEnum.PlayerTwo:
			hostName = HostNameP2;
			break;
		}

		if (hostName == "") {
			return;
		}

		IsFindMasterServer = false;
		if (MasterServer.PollHostList().Length > 0) {
			HostData[] hostData = MasterServer.PollHostList();
			
			for (int i = 0; i < hostData.Length; i++) {
				if (hostData[i].gameName == HostNameServer) {
					Debug.Log("Find masterServer!");
					IsFindMasterServer = true;
					break;
				}
			}

			if (!IsFindMasterServer) {
				for (int i = 0; i < hostData.Length; i++) {
					if (hostData[i].gameName == hostName) {
						MasterServer.RegisterHost(GameTypeName, HostNameServer, Network.player.ipAddress);
						break;
					}
				}
			}
			else {
				//Make the port change to client port.
				GameTypeCtrl.SetAppTypeVal(AppGameType.LianJiFeiJi);
				_Instance.MakeServerDisconnect();
				_Instance.LoadindeGameScene_1();
			}
		}
	}

	public void LoadindeGameScene_1()
	{
		if (SceneManager.GetActiveScene().buildIndex == (int)GameLevel.Movie) {
			Invoke("DelayLoadingGameScene_1", 6f);
		}
	}

	void DelayLoadingGameScene_1()
	{
		Debug.Log("NetworkServerNet -> DelayLoadingGameScene...");
		if (SceneManager.GetActiveScene().buildIndex == (int)GameLevel.Movie) {
			XkGameCtrl.LoadingGameScene_1();
		}
	}
}