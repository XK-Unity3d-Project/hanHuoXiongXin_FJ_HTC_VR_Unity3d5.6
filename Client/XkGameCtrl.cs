using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum NpcJiFenEnum
{
	Null = -1,
	ShiBing,
	CheLiang,
	ChuanBo,
	FeiJi,
}

public enum GameMode
{
	Null,
	LianJi,
	DanJiFeiJi,
	DanJiTanKe,
}

public enum GameJiTaiType
{
	Null,
	FeiJiJiTai,
	TanKeJiTai,
}

public class XkGameCtrl : MonoBehaviour {
	public GameObject GameWaterObj;
	/**
	 * CameraVRObj[0] -> cameraPlayer1.
	 * CameraVRObj[1] -> cameraPlayer2.
	 */
	public GameObject[] CameraVRObj;
	/**
	 * YouLiangDianMin -> 椋炴満涓昏?鍗曚汉娌归噺鐐疙
	 */
	[Range(1, 1000)] public int YouLiangDianMin = 50;
	/**
	 * YouLiangDianMinSR -> 椋炴満涓昏?鍙屼汉娌归噺鐐疙
	 */
	[Range(1, 1000)] public int YouLiangDianMinSR = 50;
	/**
	 * YouLiangDianMinLJ -> 椋炴満涓昏?鑱旀満鍗曚汉娌归噺鐐疙
	 */
	[Range(1, 1000)] public int YouLiangDianMinLJ = 50;
	/**
	 * YouLiangDianMinLJSR -> 椋炴満涓昏?鑱旀満鍙屼汉娌归噺鐐疙
	 */
	[Range(1, 1000)] public int YouLiangDianMinLJSR = 50;
	/**
	 * YouLiangDianMin -> 鍧﹀厠涓昏?鍗曚汉娌归噺鐐疙
	 */
	[Range(1, 1000)] public int TKYouLiangDianMin = 50;
	/**
	 * YouLiangDianMinSR -> 鍧﹀厠涓昏?鍙屼汉娌归噺鐐疙
	 */
	[Range(1, 1000)] public int SRTKYouLiangDianMin = 50;
	/**
	 * YouLiangDianMinLJ -> 鍧﹀厠涓昏?鑱旀満鍗曚汉娌归噺鐐疙
	 */
	[Range(1, 1000)] public int LJTKYouLiangDianMin = 50;
	/**
	 * YouLiangDianMinLJSR -> 鍧﹀厠涓昏?鑱旀満鍙屼汉娌归噺鐐疙
	 */
	[Range(1, 1000)] public int LJSRTKYouLiangDianMin = 50;
	int YouLiangDianVal;
	[Range(1, 99999)] public int DaoDanNum = 50;
	public GameObject FeiJiPlayer;
	public AiMark FeiJiPlayerMark;
	Transform FeiJiPlayerTran;
	int FeiJiMarkIndex = 1;
	AiPathCtrl FeiJiPlayerPath;
	public GameObject TanKePlayer;
	public AiMark TanKePlayerMark;
	Transform TanKePlayerTran;
	int TanKeMarkIndex = 1;
	AiPathCtrl TanKePlayerPath;
	public GameObject CartoonCamPlayer;
	public AiMark CartoonCamPlayerMark;
	public GameObject ServerCamera; //鏈嶅姟鍣ㄩ?鏈烘憚鍍忔満.
	public static GameObject ServerCameraObj;
	public GameObject ServerCameraTK; //鏈嶅姟鍣ㄥ潶鍏嬫憚鍍忔満.
	public static GameObject ServerCameraObjTK;
	Transform CartoonCamPlayerTran;
	int CartoonCamMarkIndex = 1;
	AiPathCtrl CartoonCamPlayerPath;
	public LayerMask PlayerLayer;
	public LayerMask LandLayer;
	public LayerMask NpcAmmoHitLayer;
	public LayerMask PlayerAmmoHitLayer;
	public static Transform MissionCleanup;
	public static Transform NpcAmmoArray;
	public static Transform PlayerAmmoArray;
	List<Transform> NpcTranList = new List<Transform>(20);
	static List<YouLiangDianMoveCtrl> YLDLvA = new List<YouLiangDianMoveCtrl>(20);
	static List<YouLiangDianMoveCtrl> YLDLvB = new List<YouLiangDianMoveCtrl>(20);
	static List<YouLiangDianMoveCtrl> YLDLvC = new List<YouLiangDianMoveCtrl>(20);
	public static float ScreenWidth = 1360f;
	public static float ScreenHeight = 768f;
	public static string TerrainLayer = "Terrain";
	public int[] ShiBingXunZhangJB = {10, 20};
	public int[] TanKeXunZhangJB = {10, 20};
	public int[] ChuanBoXunZhangJB = {10, 20};
	public int[] FeiJiXunZhangJB = {10, 20};
	public int[] XunZhangZP = {100, 200, 300};
	[Range(0f, 100f)]public float MinRandTimeServer = 5f;
	[Range(0f, 100f)]public float MaxRandTimeServer = 15f;
	public GameObject GameFpsPrefab;
	public GameObject AudioListPrefab;
	public GameMode TestGameModeVal = GameMode.Null;
	public static GameMode GameModeVal = GameMode.DanJiFeiJi;
	public static GameJiTaiType GameJiTaiSt = GameJiTaiType.FeiJiJiTai;
	public RenderTexture TestCameraRender;
	public bool IsCartoonShootTest;
//	public bool IsActiveAllPlayerTest = true;
	public PlayerEnum ActivePlayerTest = PlayerEnum.PlayerOne;
	public bool IsServerCameraTest;
	public static string TagNull = "Untagged";
	public static string TagMainCamera = "MainCamera";
	public static int ShiBingNumPOne;
	public static int CheLiangNumPOne;
	public static int ChuanBoNumPOne;
	public static int FeiJiNumPOne;
	public static int ShiBingNumPTwo;
	public static int CheLiangNumPTwo;
	public static int ChuanBoNumPTwo;
	public static int FeiJiNumPTwo;
	int AllPlayerKillNpc;
	int GaoBaoDanBuJiNum = 50;
	int DaoDanBuJiNum = 3;
	public static int DaoDanNumPOne;
	public static int DaoDanNumPTwo;
	public static int GaoBaoDanNumPOne;
	public static int GaoBaoDanNumPTwo;
	public static bool IsMoveOnPlayerDeath = true;
	public static float YouLiangBuJiNum = 30f;
	public static float PlayerYouLiangMax = 60f;
	public static float PlayerYouLiangCur = 60f;
	public static bool IsActivePlayerOne;
	public static bool IsActivePlayerTwo;
	public static bool IsLoadingLevel;
//	public static Transform PlayerTranCurrent;
	float TimeCheckNpcAmmo;
	float TimeCheckNpcTranList;
	public static float TriggerBoxSize_Z = 1.5f;
	static List<XKTriggerRemoveNpc> CartoonTriggerSpawnList;
	public static int CountNpcAmmo;
	static List<GameObject> NpcAmmoList;
	public static bool IsDonotCheckError = false;
	public static bool IsShowDebugInfoBox = false;
	static bool IsActiveFinishTask;
	public static bool IsPlayGamePOne;
	public static bool IsPlayGamePTwo;
	public static int YouLiangDianAddPOne;
	public static int YouLiangDianAddPTwo;
	public static GameObject ServerCameraPar;
	int YouLiangJingGaoVal = 10;
	public static bool IsAddPlayerYouLiang;
	public static bool IsGameOnQuit;
	public static bool IsDrawGizmosObj = true;
	public static int AmmoNumMaxNpc = 30;
	public bool IsOpenVR;
	public static int TestGameEndLv = (int)GameLevel.Scene_2;
	public static bool IsTiaoGuoStartCartoon = true;
	static XkGameCtrl _Instance;
	public static XkGameCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		pcvr.SetGunZhenDongDengJi(XKGlobalData.GunZhenDongP1, PlayerEnum.PlayerOne);
		pcvr.SetGunZhenDongDengJi(XKGlobalData.GunZhenDongP2, PlayerEnum.PlayerTwo);
		XKSpawnNpcPoint.ClearFiJiNpcPointList();
		XKPlayerHeTiData.IsActiveHeTiPlayer = false;
		XKTriggerClosePlayerUI.IsActiveHeTiCloseUI = false;
		XKTriggerClosePlayerUI.IsClosePlayerUI = false;
		XKTriggerKaQiuShaFire.IsFireKaQiuSha = false;
		XKTriggerOpenPlayerUI.IsActiveOpenPlayerUI = false;
		XKGlobalData.GetInstance().StopModeBeiJingAudio();
		SetActiveGameWaterObj(false);
		switch (XKGlobalData.GameDiff) {
		case "0":
			GameDiffVal = 0.8f;
			break;
		case "1":
			GameDiffVal = 1f;
			break;
		case "2":
			GameDiffVal = 1.2f;
			break;
		}
		CountNpcAmmo = 0;
		ShiBingNumPOne = 0;
		CheLiangNumPOne = 0;
		ChuanBoNumPOne = 0;
		FeiJiNumPOne = 0;
		ShiBingNumPTwo = 0;
		CheLiangNumPTwo = 0;
		ChuanBoNumPTwo = 0;
		GaoBaoDanNumPOne = 0;
		GaoBaoDanNumPTwo = 0;
		FeiJiNumPTwo = 0;
		YouLiangDianAddPOne = 0;
		YouLiangDianAddPTwo = 0;
		IsActiveFinishTask = false;
		IsAddPlayerYouLiang = false;
		ScreenDanHeiCtrl.IsStartGame = false;

		if (GameFpsPrefab != null) {
			Instantiate(GameFpsPrefab);
		}

		if (GameMovieCtrl.IsActivePlayer) {
			IsOpenVR = GameMovieCtrl.IsOpenVR;
		}

		for (int i = 0; i < CameraVRObj.Length; i++) {
			if (CameraVRObj[i] != null) {
				CameraVRObj[i].SetActive(IsOpenVR);
			}
		}

		PlayerAmmoCtrl.PlayerAmmoHitLayer = PlayerAmmoHitLayer;
		NpcAmmoList = new List<GameObject>();
		CartoonTriggerSpawnList = new List<XKTriggerRemoveNpc>();
		if (Network.peerType == NetworkPeerType.Disconnected && !NetworkServerNet.IsFindMasterServer) {
			if (!GameMovieCtrl.IsActivePlayer) {
				if (IsServerCameraTest) {
					TestGameModeVal = GameMode.LianJi;
				}
				GameModeVal = TestGameModeVal != GameMode.Null ? TestGameModeVal : GameModeVal; //TestGame
			}
			else {
				if (GameTypeCtrl.AppTypeStatic == AppGameType.DanJiFeiJi
				    || GameTypeCtrl.AppTypeStatic == AppGameType.LianJiFeiJi) {
					if (!GameModeCtrl.IsActiveClientPort) {
						GameModeVal = GameMode.DanJiFeiJi;
					}
					else {
						GameModeVal = GameMode.LianJi;
					}
				}
				else if (GameTypeCtrl.AppTypeStatic == AppGameType.DanJiTanKe
				         || GameTypeCtrl.AppTypeStatic == AppGameType.LianJiTanKe) {
					GameModeVal = GameMode.DanJiTanKe;
				}
			}
		}
		else {
			GameModeVal = GameMode.LianJi;
			if (Network.peerType == NetworkPeerType.Server) {
				GameJiTaiSt = GameJiTaiType.Null;
			}
			else if (Network.peerType == NetworkPeerType.Client) {
				if (GameTypeCtrl.AppTypeStatic == AppGameType.DanJiFeiJi
				    || GameTypeCtrl.AppTypeStatic == AppGameType.LianJiFeiJi) {
					GameJiTaiSt = GameJiTaiType.FeiJiJiTai;
				}
				else if (GameTypeCtrl.AppTypeStatic == AppGameType.DanJiTanKe
				         || GameTypeCtrl.AppTypeStatic == AppGameType.LianJiTanKe) {
					GameJiTaiSt = GameJiTaiType.TanKeJiTai;
				}
			}
		}

		if (GameMovieCtrl.IsActivePlayer) {
			IsCartoonShootTest = false;
			IsServerCameraTest = false;
		}
		Cursor.visible = pcvr.bIsHardWare;

		if (IsServerCameraTest) {
			IsCartoonShootTest = false;
		}

		NpcAmmoCtrl.NpcAmmoHitLayer = NpcAmmoHitLayer;
		GameObject obj = null;
		XkPlayerCtrl playerScript = null;
		Transform pathTran = null;
		if (FeiJiPlayerMark != null) {
			FeiJiPlayerTran = FeiJiPlayerMark.transform;
			FeiJiPlayerPath = FeiJiPlayerTran.parent.GetComponent<AiPathCtrl>();
			pathTran = FeiJiPlayerPath.transform;

			for (int i = 0; i < pathTran.childCount; i++) {
				if (FeiJiPlayerTran == pathTran.GetChild(i)) {
					FeiJiMarkIndex = i + 1;
					break;
				}
			}
		}
		else {
			Debug.LogWarning("FeiJiPlayerMark was wrong!");
			obj.name = "null";
			return;
		}

		if (TanKePlayerMark != null) {
			TanKePlayerTran = TanKePlayerMark.transform;
			TanKePlayerPath = TanKePlayerTran.parent.GetComponent<AiPathCtrl>();
			pathTran = TanKePlayerPath.transform;
			
			for (int i = 0; i < pathTran.childCount; i++) {
				if (TanKePlayerTran == pathTran.GetChild(i)) {
					TanKeMarkIndex = i + 1;
					break;
				}
			}
		}
		else {
			Debug.LogWarning("TanKePlayerMark was wrong!");
			obj.name = "null";
			return;
		}
		
		if (CartoonCamPlayerMark != null) {
			CartoonCamPlayerTran = CartoonCamPlayerMark.transform;
			CartoonCamPlayerPath = CartoonCamPlayerTran.parent.GetComponent<AiPathCtrl>();
			pathTran = CartoonCamPlayerPath.transform;
			
			for (int i = 0; i < pathTran.childCount; i++) {
				if (CartoonCamPlayerTran == pathTran.GetChild(i)) {
					CartoonCamMarkIndex = i + 1;
					break;
				}
			}
		}
		else {
			Debug.LogWarning("CartoonCamPlayerMark was wrong!");
			obj.name = "null";
			return;
		}

		Vector3 posPlayerFJ = new Vector3(0f, -1700f, 0f);
		Vector3 posPlayerTK = new Vector3(0f, -1500f, 0f);
		switch (GameModeVal) {
		case GameMode.DanJiFeiJi:
			GameJiTaiSt = GameJiTaiType.FeiJiJiTai; //test
//			obj = (GameObject)Instantiate(FeiJiPlayer, posPlayerFJ, FeiJiPlayerTran.rotation);
			obj = (GameObject)Instantiate(FeiJiPlayer,
			                              FeiJiPlayerMark.transform.position,
			                              FeiJiPlayerMark.transform.rotation);
			playerScript = obj.GetComponent<XkPlayerCtrl>();
			playerScript.SetAiPathScript(FeiJiPlayerPath);
//			PlayerTranCurrent = obj.transform;
			break;

		case GameMode.DanJiTanKe:
			GameJiTaiSt = GameJiTaiType.TanKeJiTai; //test
			obj = (GameObject)Instantiate(TanKePlayer, posPlayerTK, TanKePlayerTran.rotation);
			playerScript = obj.GetComponent<XkPlayerCtrl>();
			playerScript.SetAiPathScript(TanKePlayerPath);
//			PlayerTranCurrent = obj.transform;
			break;

		case GameMode.LianJi:
			Debug.Log("peerType "+Network.peerType);
			if (Network.peerType == NetworkPeerType.Disconnected && !NetworkServerNet.IsFindMasterServer) {
				if (!GameModeCtrl.IsActiveClientPort) {
					obj = (GameObject)Instantiate(FeiJiPlayer,
					                              FeiJiPlayerMark.transform.position,
					                              FeiJiPlayerMark.transform.rotation);
					playerScript = obj.GetComponent<XkPlayerCtrl>();
					playerScript.SetAiPathScript(FeiJiPlayerPath);
				}

//				obj = (GameObject)Instantiate(TanKePlayer, posPlayerTK, TanKePlayerTran.rotation);
//				playerScript = obj.GetComponent<XkPlayerCtrl>();
//				playerScript.SetAiPathScript(TanKePlayerPath);
			}
			else {
				AmmoNumMaxNpc = 25;
				if (Network.peerType == NetworkPeerType.Server) {
					Invoke("DelaySpawnNetPlayer", 0.5f);
				}
			}
			break;
		}

		//CartoonCamPlayer
		if (GameModeVal != GameMode.LianJi
		    || (Network.peerType == NetworkPeerType.Disconnected && !NetworkServerNet.IsFindMasterServer)) {
			if (!IsTiaoGuoStartCartoon) {
				obj = (GameObject)Instantiate(CartoonCamPlayer, CartoonCamPlayerTran.position, CartoonCamPlayerTran.rotation);
				playerScript = obj.GetComponent<XkPlayerCtrl>();
				playerScript.SetAiPathScript(CartoonCamPlayerPath);
			}
		}
		else {
			if (Network.peerType == NetworkPeerType.Server && !IsTiaoGuoStartCartoon) {
				NetworkServerNet.GetInstance().SpawnNetPlayerObj(CartoonCamPlayer,
				                                                      CartoonCamPlayerPath,
				                                                      CartoonCamPlayerTran.position,
				                                                      CartoonCamPlayerTran.rotation);
			}
		}

		GameObject objMiss = new GameObject();
		objMiss.name = "MissionCleanup";
		objMiss.transform.parent = transform;
		MissionCleanup = objMiss.transform;

		objMiss = new GameObject();
		objMiss.name = "NpcAmmoArray";
		objMiss.transform.parent = MissionCleanup;
		NpcAmmoArray = objMiss.transform;

		objMiss = new GameObject();
		objMiss.name = "PlayerAmmoArray";
		objMiss.transform.parent = MissionCleanup;
		PlayerAmmoArray = objMiss.transform;
		XKNpcSpawnListCtrl.GetInstance();

		DaoDanNum = DaoDanNum > 99 ? 99 : DaoDanNum;
		DaoDanNumPOne = DaoDanNum;
		DaoDanNumPTwo = DaoDanNum;
		PlayerYouLiangCur = 0f;
		if (Network.peerType != NetworkPeerType.Server) {
			StartCoroutine(SubPlayerYouLiang());
		}
		Invoke("DelayResetIsLoadingLevel", 2f);
		Invoke("TestInitCameraRender", 0.5f);

		if (Network.peerType == NetworkPeerType.Server || IsServerCameraTest) {
			ServerCameraObj = (GameObject)Instantiate(ServerCamera);
			ServerCameraObj.SetActive(false);
			ServerCameraObj.transform.parent = MissionCleanup;

			ServerCameraObjTK = (GameObject)Instantiate(ServerCameraTK);
			ServerCameraObjTK.SetActive(false);
			ServerCameraObjTK.transform.parent = MissionCleanup;
		}

		if (!GameMovieCtrl.IsActivePlayer) {
			switch (ActivePlayerTest) {
			case PlayerEnum.Null:
				SetActivePlayerOne(true);
				SetActivePlayerTwo(true);
				break;
				
			case PlayerEnum.PlayerOne:
				SetActivePlayerOne(true);
				break;
				
			case PlayerEnum.PlayerTwo:
				SetActivePlayerTwo(true);
				break;
			}
		}
		IsPlayGamePOne = IsActivePlayerOne;
		IsPlayGamePTwo = IsActivePlayerTwo;

		if (MinRandTimeServer >= MaxRandTimeServer) {
			Debug.LogWarning("MinRandTimeServer was wrong!");
		}

		Invoke("OnStartMovePlayer", 1f);
	}

	void OnStartMovePlayer()
	{
		GameMode modeVal = XkGameCtrl.GameModeVal;
		Debug.Log("OnStartMovePlayer -> GameMode "+modeVal);
		
		bool isClearCartoonNpc = true;
//		if (!XKTriggerOpenPlayerUI.IsActiveOpenPlayerUI) {
//			if (XKPlayerCamera.GetInstanceCartoon() != null) {
//				XKPlayerCamera.GetInstanceCartoon().SetActiveCamera(false);
//			}
//		}
		
		switch (modeVal) {
		case GameMode.DanJiFeiJi:
			XkPlayerCtrl.GetInstanceFeiJi().MakePlayerFlyToPathMark();
			break;
			
//		case GameMode.DanJiTanKe:
//			XkPlayerCtrl.GetInstanceTanKe().MakePlayerFlyToPathMark();
//			break;
			
		case GameMode.LianJi:
			if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
				XkPlayerCtrl.GetInstanceFeiJi().MakePlayerFlyToPathMark();
			}
			
			if (XkPlayerCtrl.GetInstanceTanKe() != null) {
				XkPlayerCtrl.GetInstanceTanKe().MakePlayerFlyToPathMark();
			}
			
			if (Network.peerType != NetworkPeerType.Disconnected) {
				isClearCartoonNpc = false;
			}
			
			if (Network.peerType == NetworkPeerType.Client) {
				NetCtrl.GetInstance().SendSetScreenDanHeiIsStartGame();
			}
			break;
		}
//		DestroyObject(DanHeiTweenAlpha);
//		DanHeiTweenAlpha = ScreenDanHeiObj.AddComponent<TweenAlpha>();
//		DanHeiTweenAlpha.enabled = false;
//		DanHeiTweenAlpha.from = 1f;
//		DanHeiTweenAlpha.to = 0f;
//		EventDelegate.Add(DanHeiTweenAlpha.onFinished, delegate{
//			Invoke("OnSreenAlphaToMin", 0.2f);
//		});
//		DanHeiTweenAlpha.enabled = true;
//		DanHeiTweenAlpha.PlayForward();
		
		if (isClearCartoonNpc) {
			XkGameCtrl.ClearCartoonSpawnNpc();
		}
		
//		if (Network.peerType != NetworkPeerType.Server) {
//			IsStartGame = true;
//		}
		Time.timeScale = 1.0f;
		switch (modeVal) {
		case GameMode.DanJiFeiJi:
			XkPlayerCtrl.GetInstanceFeiJi().RestartMovePlayer();
			break;
			
//		case GameMode.DanJiTanKe:
//			XkPlayerCtrl.GetInstanceTanKe().RestartMovePlayer();
//			break;
			
		case GameMode.LianJi:
			if (Network.peerType != NetworkPeerType.Server) {
				if (Network.peerType != NetworkPeerType.Client) {
					if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
						XkPlayerCtrl.GetInstanceFeiJi().RestartMovePlayer();
					}
					
//					if (XkPlayerCtrl.GetInstanceTanKe() != null) {
//						XkPlayerCtrl.GetInstanceTanKe().RestartMovePlayer();
//					}
//					XKCameraMapCtrl.GetInstance().SetCameraMapState(); //test
				}
				else {
					//SendServerMovePlayer
//					NetCtrl.GetInstance().SetScreenDanHieStartMovePlayer();
				}
			}
			else {
				//AddStartMovePlayerCount();
				if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
					XkPlayerCtrl.GetInstanceFeiJi().RestartMovePlayer();
				}
			}
			break;
		}
		ScreenDanHeiCtrl.IsStartGame = true;

		if (GameModeCtrl.IsActiveClientPort) {
			if (NetworkServerNet.GetInstance() != null) {
				NetworkServerNet.GetInstance().TryToLinkServer();
			}
		}
	}

	public void ChangeAudioListParent()
	{
//		if (Network.peerType == NetworkPeerType.Server) {
//			return;
//		}

		if (SceneManager.GetActiveScene().buildIndex < (int)GameLevel.Scene_1 || SceneManager.GetActiveScene().buildIndex > (int)GameLevel.Scene_4) {
			return;
		}

		Debug.Log("ChangeAudioListParent -> GameJiTaiSt "+GameJiTaiSt);
		switch (GameJiTaiSt) {
		case GameJiTaiType.Null:
		case GameJiTaiType.FeiJiJiTai:
			if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
				AudioManager.Instance.SetParentTran(XkPlayerCtrl.GetInstanceFeiJi().transform);
			}
			break;

		case GameJiTaiType.TanKeJiTai:
			if (XkPlayerCtrl.GetInstanceTanKe() != null) {
				AudioManager.Instance.SetParentTran(XkPlayerCtrl.GetInstanceTanKe().transform);
			}
			break;
		}
	}

	void DelaySpawnNetPlayer()
	{
		NetworkServerNet.GetInstance().SpawnNetPlayerObj(FeiJiPlayer,
		                                                 FeiJiPlayerPath,
		                                                 FeiJiPlayerMark.transform.position,
		                                                 FeiJiPlayerMark.transform.rotation);
//		Vector3 posPlayerFJ = new Vector3(0f, -1700f, 0f);
//		Vector3 posPlayerTK = new Vector3(0f, -1500f, 0f);
//		if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiFeiJi) {
//			NetworkServerNet.GetInstance().SpawnNetPlayerObj(FeiJiPlayer,
//			                                                 	FeiJiPlayerPath,
//				                                                FeiJiPlayerMark.transform.position,
//				                                                FeiJiPlayerMark.transform.rotation);
//		}
//		else if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiTanKe) {
//			NetworkServerNet.GetInstance().SpawnNetPlayerObj(TanKePlayer,
//			                                                      TanKePlayerPath,
//			                                                      posPlayerTK,
//			                                                      TanKePlayer.transform.rotation);
//		}
	}

	void Update()
	{
		if (!pcvr.bIsHardWare) {
			if (IsCartoonShootTest) {
				if (Input.GetKeyUp(KeyCode.N)) {
					if (!XkGameCtrl.IsGameOnQuit && (SceneManager.GetActiveScene().buildIndex+1) < SceneManager.sceneCountInBuildSettings) {
						System.GC.Collect();
						SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex+1));
					}
				}
			}

//			if (GameModeVal == GameMode.LianJi
//			    && Network.peerType == NetworkPeerType.Disconnected
//			    && ScreenDanHeiCtrl.IsStartGame) {
//				if (Input.GetKeyUp(KeyCode.P)) {
//					TestChangePlayerCamera(); //test
//				}
				
//				if (Input.GetKeyUp(KeyCode.O)
//				    && XKCameraMapCtrl.GetInstance() != null) {
//					XKCameraMapCtrl.GetInstance().SetCameraMapState(); //test
//				}
//			}
			
			if (Input.GetKeyUp(KeyCode.S)) {
				IsShowDebugInfoBox = !IsShowDebugInfoBox; //test
			}

//			if (Input.GetKeyUp(KeyCode.N)) {
//				IsSubPlayerYouLiangTest = !IsSubPlayerYouLiangTest;
//				if (IsSubPlayerYouLiangTest) {
//					PlayerYouLiangCur = 1f;
//				}
//				else {
//					PlayerYouLiangCur = PlayerYouLiangMax;
//				}
//			}
		}

		CheckNpcAmmoList();
		CheckNpcTranFromList();
	}

	void DelayResetIsLoadingLevel()
	{
		XkGameCtrl.ResetIsLoadingLevel();
	}

	public static void ResetIsLoadingLevel()
	{
		IsLoadingLevel = false;
	}

	void TestInitCameraRender()
	{
		if (GameModeVal != GameMode.LianJi) {
			return;
		}

		if(XKPlayerCamera.GetInstanceCartoon() != null) {
			XKPlayerCamera.GetInstanceCartoon().SetActiveCamera(false);
		}

		if (XKPlayerCamera.GetInstanceTanKe() != null) {
			XKPlayerCamera.GetInstanceTanKe().SetActiveCamera(false);
		}

		if (XKPlayerCamera.GetInstanceFeiJi() != null) {
			XKPlayerCamera.GetInstanceFeiJi().SetActiveCamera(false);
		}
		
		if (XKPlayerCamera.GetInstanceTanKe() != null) {
			XKPlayerCamera.GetInstanceTanKe().SetActiveCamera(true);
		}
		
		if (XKPlayerCamera.GetInstanceFeiJi() != null) {
			XKPlayerCamera.GetInstanceFeiJi().SetActiveCamera(true);
			XKPlayerCamera.GetInstanceFeiJi().GetComponent<Camera>().targetTexture = null;
		}

		if (XKPlayerCamera.GetInstanceTanKe() != null && Network.peerType == NetworkPeerType.Disconnected) {
			XKPlayerCamera.GetInstanceTanKe().GetComponent<Camera>().targetTexture = TestCameraRender;
		}

		if(XKPlayerCamera.GetInstanceCartoon() != null) {
			XKPlayerCamera.GetInstanceCartoon().SetActiveCamera(true);
		}

		if (Network.peerType == NetworkPeerType.Disconnected) {
			if (XKPlayerCamera.GetInstanceFeiJi() != null && XKPlayerCamera.GetInstanceTanKe() != null) {
				XKPlayerCamera.GetInstanceFeiJi().gameObject.tag = TagMainCamera;
				XKPlayerCamera.GetInstanceTanKe().gameObject.tag = TagNull;
			}
			else if (XKPlayerCamera.GetInstanceFeiJi() != null) {
				XKPlayerCamera.GetInstanceFeiJi().gameObject.tag = TagMainCamera;
			}
			else if (XKPlayerCamera.GetInstanceTanKe() != null) {
				XKPlayerCamera.GetInstanceTanKe().gameObject.tag = TagMainCamera;
			}
		}
		else {
			if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiTanKe) {
				if (XKPlayerCamera.GetInstanceTanKe() != null) {
					XKPlayerCamera.GetInstanceTanKe().gameObject.tag = TagMainCamera;
				}
			}
			else {
				if (XKPlayerCamera.GetInstanceFeiJi() != null) {
					XKPlayerCamera.GetInstanceFeiJi().gameObject.tag = TagMainCamera;
				}
			}
		}
	}

	public void ChangePlayerCameraTag()
	{
		if (GameModeVal != GameMode.LianJi) {
			return;
		}

		if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiTanKe) {
			if (XKPlayerCamera.GetInstanceTanKe() != null) {
				XKPlayerCamera.GetInstanceTanKe().gameObject.tag = TagMainCamera;
				XKPlayerCamera.GetInstanceTanKe().SetActiveCamera(true);
				XKPlayerCamera.GetInstanceTanKe().ActivePlayerCamera();
			}

			if (XKPlayerCamera.GetInstanceFeiJi() != null) {
				XKPlayerCamera.GetInstanceFeiJi().gameObject.tag = TagNull;
			}
		}
		else {
			if (XKPlayerCamera.GetInstanceTanKe() != null) {
				XKPlayerCamera.GetInstanceTanKe().gameObject.tag = TagNull;
			}

			if (XKPlayerCamera.GetInstanceFeiJi() != null) {
				XKPlayerCamera.GetInstanceFeiJi().gameObject.tag = TagMainCamera;
			}
		}
	}
	
	void TestChangePlayerCamera()
	{
		if (GameModeVal != GameMode.LianJi) {
			return;
		}

		//AudioListener listener = null;
		switch (GameJiTaiSt) {
		case GameJiTaiType.FeiJiJiTai:
			GameJiTaiSt = GameJiTaiType.TanKeJiTai;
			XKPlayerCamera.GetInstanceTanKe().SetActiveCamera(false);
			XKPlayerCamera.GetInstanceFeiJi().SetActiveCamera(false);
			XKPlayerCamera.GetInstanceFeiJi().SetActiveCamera(true);
//			if (!XKCameraMapCtrl.GetInstance().GetActiveCameraMap()) {
//				XKPlayerCamera.GetInstanceFeiJi().SetEnableCamera(false);
//			}
			XKPlayerCamera.GetInstanceTanKe().SetActiveCamera(true);
			XKPlayerCamera.GetInstanceFeiJi().gameObject.tag = TagNull;
			XKPlayerCamera.GetInstanceTanKe().gameObject.tag = TagMainCamera;

			XKPlayerCamera.GetInstanceFeiJi().GetComponent<Camera>().targetTexture = TestCameraRender;
			XKPlayerCamera.GetInstanceTanKe().GetComponent<Camera>().targetTexture = null;
//			PlayerTranCurrent = XkPlayerCtrl.GetInstanceTanKe().transform;
			Camera.SetupCurrent(XKPlayerCamera.GetInstanceFeiJi().GetComponent<Camera>());

			/*listener = XKPlayerCamera.GetInstanceFeiJi().GetComponent<AudioListener>();
			listener.enabled = false;
			listener = XKPlayerCamera.GetInstanceTanKe().GetComponent<AudioListener>();
			if (ScreenDanHeiCtrl.IsStartGame) {
				listener.enabled = true;
			}
			else {
				listener.enabled = false;
			}*/
			break;

		case GameJiTaiType.TanKeJiTai:
			GameJiTaiSt = GameJiTaiType.FeiJiJiTai;
			XKPlayerCamera.GetInstanceTanKe().SetActiveCamera(false);
			XKPlayerCamera.GetInstanceFeiJi().SetActiveCamera(false);
			XKPlayerCamera.GetInstanceTanKe().SetActiveCamera(true);
//			if (!XKCameraMapCtrl.GetInstance().GetActiveCameraMap()) {
//				XKPlayerCamera.GetInstanceTanKe().SetEnableCamera(false);
//			}
			XKPlayerCamera.GetInstanceFeiJi().SetActiveCamera(true);
			XKPlayerCamera.GetInstanceFeiJi().gameObject.tag = TagMainCamera;
			XKPlayerCamera.GetInstanceTanKe().gameObject.tag = TagNull;

			XKPlayerCamera.GetInstanceFeiJi().GetComponent<Camera>().targetTexture = null;
			XKPlayerCamera.GetInstanceTanKe().GetComponent<Camera>().targetTexture = TestCameraRender;
//			PlayerTranCurrent = XkPlayerCtrl.GetInstanceFeiJi().transform;
			Camera.SetupCurrent(XKPlayerCamera.GetInstanceTanKe().GetComponent<Camera>());

			/*listener = XKPlayerCamera.GetInstanceFeiJi().GetComponent<AudioListener>();
			if (ScreenDanHeiCtrl.IsStartGame) {
				listener.enabled = true;
			}
			else {
				listener.enabled = false;
			}
			listener = XKPlayerCamera.GetInstanceTanKe().GetComponent<AudioListener>();
			listener.enabled = false;*/
			break;
		}
		ChangeAudioListParent();
	}

	public List<Transform> GetNpcTranList()
	{
		return NpcTranList;
	}

	public void AddNpcTranToList(Transform tran)
	{
		if (tran == null || NpcTranList.Contains(tran)) {
			return;
		}
		NpcTranList.Add(tran);
	}

	public void RemoveNpcTranFromList(Transform tran)
	{
		if (tran == null || !NpcTranList.Contains(tran)) {
			return;
		}
		NpcTranList.Remove(tran);
	}

	public void CheckNpcTranFromList()
	{
		float dTime = Time.realtimeSinceStartup - TimeCheckNpcTranList;
		if (dTime < 0.1f) {
			return;
		}
		TimeCheckNpcTranList = Time.realtimeSinceStartup;

		int max = NpcTranList.Count;
		int[] countArray = new int[max];
		int indexCount = 0;
		for (int i = 0; i < max; i++) {
			if (NpcTranList[i] == null) {
				countArray[indexCount] = i;
				indexCount++;
			}
		}
		
		for (int i = 0; i < max; i++) {
			if (countArray[i] == 0 && i > 0) {
				break;
			}

			if (countArray[i] >= NpcTranList.Count) {
				break;
			}

			if (NpcTranList[countArray[i]] == null) {
				NpcTranList.RemoveAt(countArray[i]);
			}
		}
	}

	public void AddPlayerKillNpc(PlayerEnum playerSt, NpcJiFenEnum npcSt)
	{
		AllPlayerKillNpc++;
		switch (playerSt) {
		case PlayerEnum.Null:
			if (XkGameCtrl.IsActivePlayerOne) {
				AddKillNpcToPlayerOne(npcSt);
			}

			if (XkGameCtrl.IsActivePlayerTwo) {
				AddKillNpcToPlayerTwo(npcSt);
			}
			break;

		case PlayerEnum.PlayerOne:
			AddKillNpcToPlayerOne(npcSt);
			break;

		case PlayerEnum.PlayerTwo:
			AddKillNpcToPlayerTwo(npcSt);
			break;
		}
	}

	void AddKillNpcToPlayerOne(NpcJiFenEnum npcSt)
	{
		switch (npcSt) {
		case NpcJiFenEnum.ShiBing:
			ShiBingNumPOne++;
			break;

		case NpcJiFenEnum.CheLiang:
			CheLiangNumPOne++;
			break;

		case NpcJiFenEnum.ChuanBo:
			ChuanBoNumPOne++;
			break;

		case NpcJiFenEnum.FeiJi:
			FeiJiNumPOne++;
			break;
		}
	}

	void AddKillNpcToPlayerTwo(NpcJiFenEnum npcSt)
	{
		switch (npcSt) {
		case NpcJiFenEnum.ShiBing:
			ShiBingNumPTwo++;
			break;
			
		case NpcJiFenEnum.CheLiang:
			CheLiangNumPTwo++;
			break;
			
		case NpcJiFenEnum.ChuanBo:
			ChuanBoNumPTwo++;
			break;
			
		case NpcJiFenEnum.FeiJi:
			FeiJiNumPTwo++;
			break;
		}
	}

	public void AddDaoDanNum(PlayerEnum playerSt)
	{
		if (IsOpenVR) {
			return;
		}

		switch(playerSt) {
		case PlayerEnum.PlayerOne:
			DaoDanNumPOne += DaoDanBuJiNum;
			if (DaoDanNumPOne > 99) {
				DaoDanNumPOne = 99;
			}

			if (DanYaoInfoCtrl.GetInstanceOne() != null) {
				DanYaoInfoCtrl.GetInstanceOne().ShowDaoDanSprite();
			}
			break;
			
		case PlayerEnum.PlayerTwo:
			DaoDanNumPTwo += DaoDanBuJiNum;
			if (DaoDanNumPTwo > 99) {
				DaoDanNumPTwo = 99;
			}

			if (DanYaoInfoCtrl.GetInstanceTwo() != null) {
				DanYaoInfoCtrl.GetInstanceTwo().ShowDaoDanSprite();
			}
			break;
		}
	}
	
	public void AddGaoBaoDanNum(PlayerEnum playerSt)
	{
		switch(playerSt) {
		case PlayerEnum.PlayerOne:
			if (!IsActivePlayerOne) {
				return;
			}

			GaoBaoDanNumPOne += GaoBaoDanBuJiNum;
			if (GaoBaoDanNumPOne > 99) {
				GaoBaoDanNumPOne = 99;
			}

			if (DanYaoInfoCtrl.GetInstanceOne() != null) {
				DanYaoInfoCtrl.GetInstanceOne().ShowHuoLiJQSprite();
			}
			XKPlayerAutoFire.AmmoStatePOne = PlayerAmmoType.GaoBaoAmmo;
			if (ZhunXingCtrl.GetInstanceOne() != null) {
				ZhunXingCtrl.GetInstanceOne().SetZhunXingSprite();
			}
			break;
			
		case PlayerEnum.PlayerTwo:
			if (!IsActivePlayerTwo) {
				return;
			}

			GaoBaoDanNumPTwo += GaoBaoDanBuJiNum;
			if (GaoBaoDanNumPTwo > 99) {
				GaoBaoDanNumPTwo = 99;
			}
			if (DanYaoInfoCtrl.GetInstanceTwo() != null) {
				DanYaoInfoCtrl.GetInstanceTwo().ShowHuoLiJQSprite();
			}
			XKPlayerAutoFire.AmmoStatePTwo = PlayerAmmoType.GaoBaoAmmo;
			if (ZhunXingCtrl.GetInstanceTwo() != null) {
				ZhunXingCtrl.GetInstanceTwo().SetZhunXingSprite();
			}
			break;
		}
	}
	
	public int GetGaoBaoDanNumPOne()
	{
		return GaoBaoDanNumPOne;
	}

	public int GetGaoBaoDanNumPTwo()
	{
		return GaoBaoDanNumPTwo;
	}
	
	public void SubGaoBaoDanNumPOne()
	{
		//Debug.Log("SubGaoBaoDanNumPOne -> GaoBaoDanNumPOne "+GaoBaoDanNumPOne);
		GaoBaoDanNumPOne--;
		if (GaoBaoDanNumPOne <= 0) {
			XKPlayerAutoFire.AmmoStatePOne = PlayerAmmoType.PuTongAmmo;
			if (ZhunXingCtrl.GetInstanceOne() != null) {
				ZhunXingCtrl.GetInstanceOne().SetZhunXingSprite();
			}
		}
	}
	
	public void SubGaoBaoDanNumPTwo()
	{
		GaoBaoDanNumPTwo--;
		if (GaoBaoDanNumPTwo <= 0) {
			XKPlayerAutoFire.AmmoStatePTwo = PlayerAmmoType.PuTongAmmo;
			if (ZhunXingCtrl.GetInstanceTwo() != null) {
				ZhunXingCtrl.GetInstanceTwo().SetZhunXingSprite();
			}
		}
	}
	
	public int GetDaoDanNumPOne()
	{
		return DaoDanNumPOne;
	}
	
	public int GetDaoDanNumPTwo()
	{
		return DaoDanNumPTwo;
	}

	public void SubDaoDanNumPOne()
	{
		if (IsOpenVR) {
			return;
		}
		DaoDanNumPOne--;
	}
	
	public void SubDaoDanNumPTwo()
	{
		if (IsOpenVR) {
			return;
		}
		DaoDanNumPTwo--;
	}

	static bool IsSubPlayerYouLiangTest = true;
	IEnumerator SubPlayerYouLiang()
	{
		do {
			yield return new WaitForSeconds(1.0f);
			if (JiFenJieMianCtrl.GetInstance() != null && JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
				yield break;
			}

			if (PlayerYouLiangCur <= 0f
			    || !ScreenDanHeiCtrl.IsStartGame
			    || !ZhunXingTeXiaoCtrl.IsOverTeXiaoZhunXing) {
				continue;
			}
			
			if (XKTriggerClosePlayerUI.IsClosePlayerUI
			    || YouLiangCtrl.IsChangeYouLiangFillAmout
			    || IsAddPlayerYouLiang
			    || IsCartoonShootTest
			    || IsServerCameraTest) {
				continue;
			}

			if (Network.peerType == NetworkPeerType.Server) {
				continue;
			}

			if (XKTriggerGameOver.IsActiveGameOver) {
				yield break;
			}
			
			//System.GC.Collect();
			if (PlayerYouLiangCur > 0) {
				PlayerYouLiangCur -= 1f;
				PlayerYouLiangCur += 1f; //test
				if (!pcvr.bIsHardWare && !IsSubPlayerYouLiangTest) {
					PlayerYouLiangCur += 1f; //test
				}
			}
//			if (PlayerYouLiangCur > 15f) {
//				PlayerYouLiangCur -= 14f; //test
//			}

			if (!DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi()) {
				if (PlayerYouLiangCur <= YouLiangJingGaoVal && !YouLiangCtrl.IsActiveYouLiangFlash) {
					YouLiangCtrl.GetInstance().SetActiveYouLiangFlash(true);
					XKGlobalData.GetInstance().PlayAudioRanLiaoJingGao();
				}
			}

			if (PlayerYouLiangCur <= 0f) {
				PlayerYouLiangCur = 0f;
				if (JiFenJieMianCtrl.GetInstance() != null && !JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
					DaoJiShiCtrl.GetInstance().StartPlayDaoJiShi();
				}
				SetActivePlayerOne(false);
				SetActivePlayerTwo(false);
			}
		} while(true);
	}

	public void AddPlayerYouLiang(float val)
	{
//		PlayerYouLiangCur += YouLiangBuJiNum;
		if (DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi()) {
			return;
		}
		IsAddPlayerYouLiang = false;

		float startVal = PlayerYouLiangCur;
		PlayerYouLiangCur += val;
		if (PlayerYouLiangCur > PlayerYouLiangMax) {
			PlayerYouLiangCur = PlayerYouLiangMax;
		}
		YouLiangCtrl.GetInstance().InitChangePlayerYouLiangFillAmout(startVal, PlayerYouLiangCur);

		if (PlayerYouLiangCur > YouLiangJingGaoVal) {
			YouLiangCtrl.GetInstance().SetActiveYouLiangFlash(false);
		}
	}

	public static float GameDiffVal = 1f;
	public void AddYouLiangDian(int val, PlayerEnum playerSt)
	{
		if (IsCartoonShootTest) {
			return;
		}

		if (JiFenJieMianCtrl.GetInstance() != null && JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
			return;
		}

		switch (playerSt) {
		case PlayerEnum.PlayerOne:
			YouLiangDianAddPOne += val;
			break;

		case PlayerEnum.PlayerTwo:
			YouLiangDianAddPTwo += val;
			break;

		default:
			YouLiangDianAddPOne += val;
			YouLiangDianAddPTwo += val;
			break;
		}

		YouLiangDianVal += val;
		int youLiangDianTmp = YouLiangDianMin;
		if (IsActivePlayerOne && IsActivePlayerTwo) {
			if (GameModeVal == GameMode.LianJi) {
				if (GameJiTaiSt == GameJiTaiType.FeiJiJiTai) {
					youLiangDianTmp = YouLiangDianMinLJSR;
				}
				else {
					youLiangDianTmp = LJSRTKYouLiangDianMin;
				}
			}
			else {
				if (GameJiTaiSt == GameJiTaiType.FeiJiJiTai) {
					youLiangDianTmp = YouLiangDianMinSR;
				}
				else {
					youLiangDianTmp = SRTKYouLiangDianMin;
				}
			}
		}
		else {
			if (GameModeVal == GameMode.LianJi) {
				if (GameJiTaiSt == GameJiTaiType.FeiJiJiTai) {
					youLiangDianTmp = YouLiangDianMinLJ;
				}
				else {
					youLiangDianTmp = LJTKYouLiangDianMin;
				}
			}
			else {
				if (GameJiTaiSt != GameJiTaiType.FeiJiJiTai) {
					youLiangDianTmp = TKYouLiangDianMin;
				}
			}
		}
		youLiangDianTmp = (int)(youLiangDianTmp * GameDiffVal);

		if (YouLiangDianVal >= youLiangDianTmp
		    && DaoJiShiCtrl.GetInstance() != null
		    && !DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi()) {
			IsAddPlayerYouLiang = true;
			YouLiangMarkCtrl.GetInstance().MoveYouLiangMark();
			YouLiangDianVal -= youLiangDianTmp;
		}

		float valTmp = (float)YouLiangDianVal / youLiangDianTmp;
		if (YouLiangAddCtrl.GetInstance() != null) {
			YouLiangAddCtrl.GetInstance().SetYouLiangSpriteAmount(valTmp);
		}
	}

	void ResetPlayerYouLiangVal()
	{
		YouLiangDianVal = 0;
		if (YouLiangAddCtrl.GetInstance() != null) {
			YouLiangAddCtrl.GetInstance().SetYouLiangSpriteAmount(0f);
		}
	}

	public static void SetActivePlayerOne(bool isActive)
	{
		IsActivePlayerOne = isActive;
		if (isActive) {
			IsPlayGamePOne = true;
			pcvr.StartLightStateP1 = LedState.Mie;
		}

		if (XKPlayerGunLaser.GetInstanceOne() != null) {
			XKPlayerGunLaser.GetInstanceOne().SetActivePlayerLaser(isActive);
		}

		if (XkGameCtrl.GetInstance() == null ||
			(XkGameCtrl.GetInstance() != null && !XkGameCtrl.GetInstance().IsCartoonShootTest) ) {
			if (isActive && SceneManager.GetActiveScene().buildIndex == (int)GameLevel.Movie) {
				StopMovie();
			}
		}

		if (SceneManager.GetActiveScene().buildIndex >= (int)GameLevel.Scene_1 && SceneManager.GetActiveScene().buildIndex <= (int)GameLevel.Scene_4) {
			if (isActive) {
				if (!XKTriggerClosePlayerUI.IsClosePlayerUI) {
					DaoDanNumPOne = _Instance.DaoDanNum;
				}

				CheckMovePlayer();
				if (DanYaoInfoCtrl.GetInstanceOne() != null) {
					DanYaoInfoCtrl.GetInstanceOne().ShowPlayerDanYaoInfo();
				}

				if (XkGameCtrl.GetInstance() != null) {
					XkGameCtrl.GetInstance().ResetPlayerYouLiangVal();
				}
			}
			else {
				if (DanYaoInfoCtrl.GetInstanceOne() != null) {
					DanYaoInfoCtrl.GetInstanceOne().HiddenPlayerDanYaoInfo();
				}
			}
		}

		if (ZhunXingCtrl.GetInstanceOne() != null) {
			if (SceneManager.GetActiveScene().buildIndex == (int)GameLevel.Movie) {
				if (GameTypeCtrl.AppTypeStatic == AppGameType.DanJiTanKe
				    || GameTypeCtrl.AppTypeStatic == AppGameType.DanJiFeiJi) {
					isActive = false;
				}
			}
			ZhunXingCtrl.GetInstanceOne().SetActiveZhunXingObj(isActive);
		}
		SetPlayerFireMaxAmmoCount();
	}

	public static void SetActivePlayerTwo(bool isActive)
	{
		IsActivePlayerTwo = isActive;
		if (isActive) {
			IsPlayGamePTwo = true;
			pcvr.StartLightStateP2 = LedState.Mie;
		}
		
		if (XKPlayerGunLaser.GetInstanceTwo() != null) {
			XKPlayerGunLaser.GetInstanceTwo().SetActivePlayerLaser(isActive);
		}
		
		if (XkGameCtrl.GetInstance() == null ||
		    (XkGameCtrl.GetInstance() != null && !XkGameCtrl.GetInstance().IsCartoonShootTest) ) {
			if (isActive && SceneManager.GetActiveScene().buildIndex == (int)GameLevel.Movie) {
				StopMovie();
			}
		}
		
		if (SceneManager.GetActiveScene().buildIndex >= (int)GameLevel.Scene_1 && SceneManager.GetActiveScene().buildIndex <= (int)GameLevel.Scene_4) {
			if (isActive) {
				if (!XKTriggerClosePlayerUI.IsClosePlayerUI) {
					DaoDanNumPTwo = _Instance.DaoDanNum;
				}

				CheckMovePlayer();
				if (DanYaoInfoCtrl.GetInstanceTwo() != null) {
					DanYaoInfoCtrl.GetInstanceTwo().ShowPlayerDanYaoInfo();
				}
				
				if (XkGameCtrl.GetInstance() != null) {
					XkGameCtrl.GetInstance().ResetPlayerYouLiangVal();
				}
			}
			else {
				if (DanYaoInfoCtrl.GetInstanceTwo() != null) {
					DanYaoInfoCtrl.GetInstanceTwo().HiddenPlayerDanYaoInfo();
				}
			}
		}

		if (ZhunXingCtrl.GetInstanceTwo() != null) {
			if (SceneManager.GetActiveScene().buildIndex == (int)GameLevel.Movie) {
				if (GameTypeCtrl.AppTypeStatic == AppGameType.DanJiTanKe
				    || GameTypeCtrl.AppTypeStatic == AppGameType.DanJiFeiJi) {
					isActive = false;
				}
			}
			ZhunXingCtrl.GetInstanceTwo().SetActiveZhunXingObj(isActive);
		}
		SetPlayerFireMaxAmmoCount();
	}
	
	static void SetPlayerFireMaxAmmoCount()
	{		
		if (!IsActivePlayerOne || !IsActivePlayerTwo) {
			XKPlayerAutoFire.MaxAmmoCount = 15;
		}
		else {
			XKPlayerAutoFire.MaxAmmoCount = 30;
		}
	}

	static void StopMovie()
	{
		if (SceneManager.GetActiveScene().buildIndex != (int)GameLevel.Movie) {
			return;
		}
		GameMovieCtrl.GetInstance().StopPlayMovie();
		switch (GameTypeCtrl.AppTypeStatic) {
		case AppGameType.DanJiFeiJi:
		case AppGameType.DanJiTanKe:
			GameModeCtrl.GetInstance().SetActiveLoading(true);
			break;

		case AppGameType.LianJiFeiJi:
		case AppGameType.LianJiTanKe:
		case AppGameType.LianJiServer:
			GameModeCtrl.GetInstance().ShowGameMode();
			break;
		}
	}

//	static int LevelCountValTest = 0;
	public static void LoadingGameScene_1()
	{
		//Debug.Log("LoadingGameScene_1...");
		GameMovieCtrl.GetInstance().StopPlayMovie();
		XkGameCtrl.IsLoadingLevel = true;
		if (!XkGameCtrl.IsGameOnQuit) {
			System.GC.Collect();
			SceneManager.LoadScene((int)GameLevel.Scene_1);
			//Application.LoadLevel((int)GameLevel.Scene_4);//test
//			LevelCountValTest++;
//			Application.LoadLevel(LevelCountValTest);
//			if (LevelCountValTest >= (int)GameLevel.Scene_3) {
//				LevelCountValTest = 0;
//			}
		}
	}

	public static void LoadingGameMovie(int key = 0)
	{
		Debug.Log("LoadingGameMovie...");
		if (XkGameCtrl.IsLoadingLevel) {
			return;
		}

		if (NetworkServerNet.GetInstance() != null && NetCtrl.GetInstance() != null && key == 0) {
			NetworkServerNet.GetInstance().MakeClientDisconnect(); //Close ClientNet
			NetworkServerNet.GetInstance().MakeServerDisconnect(); //Close ServerNet
		}
		
		ResetGameInfo();
		SetActivePlayerOne(false);
		SetActivePlayerTwo(false);
		IsLoadingLevel = true;
		if (!XkGameCtrl.IsGameOnQuit) {
			System.GC.Collect();
			SceneManager.LoadScene((int)GameLevel.Movie);
		}
	}

	public static void CheckMovePlayer()
	{
		if (!ScreenDanHeiCtrl.IsStartGame) {
			return;
		}

		switch (XkGameCtrl.GameModeVal) {
		case GameMode.DanJiFeiJi:
			if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
				if (!IsActivePlayerOne && !IsActivePlayerTwo) {
					XkPlayerCtrl.GetInstanceFeiJi().StopMovePlayer();
				}
				else {
//					AddPlayerYouLiangToMax();
					if (DaoJiShiCtrl.GetInstance() != null) {
						DaoJiShiCtrl.GetInstance().StopDaoJiShi();
					}
					XkPlayerCtrl.GetInstanceFeiJi().RestartMovePlayer();
				}
			}
			break;

		case GameMode.DanJiTanKe:
			if (XkPlayerCtrl.GetInstanceTanKe() != null) {
				if (!IsActivePlayerOne && !IsActivePlayerTwo) {
					XkPlayerCtrl.GetInstanceTanKe().StopMovePlayer();
				}
				else {
//					AddPlayerYouLiangToMax();
					if (DaoJiShiCtrl.GetInstance() != null) {
						DaoJiShiCtrl.GetInstance().StopDaoJiShi();
					}
					XkPlayerCtrl.GetInstanceTanKe().RestartMovePlayer();
				}
			}
			break;

		case GameMode.LianJi:
			if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
				if (!IsActivePlayerOne && !IsActivePlayerTwo) {
					XkPlayerCtrl.GetInstanceFeiJi().StopMovePlayer();
				}
				else {
//					AddPlayerYouLiangToMax();
					if (DaoJiShiCtrl.GetInstance() != null) {
						DaoJiShiCtrl.GetInstance().StopDaoJiShi();
					}
					XkPlayerCtrl.GetInstanceFeiJi().RestartMovePlayer();
				}
			}

			if (XkPlayerCtrl.GetInstanceTanKe() != null) {
				if (!IsActivePlayerOne && !IsActivePlayerTwo) {
					XkPlayerCtrl.GetInstanceTanKe().StopMovePlayer();
				}
				else {
//					AddPlayerYouLiangToMax();
					if (DaoJiShiCtrl.GetInstance() != null) {
						DaoJiShiCtrl.GetInstance().StopDaoJiShi();
					}
					XkPlayerCtrl.GetInstanceTanKe().RestartMovePlayer();
				}
			}
			break;
		}
	}

	public static void AddPlayerYouLiangToMax()
	{
		if (SceneManager.GetActiveScene().buildIndex == (int)GameLevel.Scene_1) {
			PlayerYouLiangMax = 120f;
		}
		else {
			PlayerYouLiangMax = 60f;
		}
		//Debug.Log("AddPlayerYouLiangToMax -> PlayerYouLiangMax "+PlayerYouLiangMax);
		PlayerYouLiangCur = PlayerYouLiangMax;
//		PlayerYouLiangCur = 10f; //test
		if (YouLiangCtrl.GetInstance() != null) {
			YouLiangCtrl.GetInstance().SetActiveYouLiangFlash(false);
		}
	}

	public static void OnPlayerFinishTask()
	{
		if (IsActiveFinishTask) {
			return;
		}
		IsActiveFinishTask = true;

		if (JiFenJieMianCtrl.GetInstance() != null) {
			JiFenJieMianCtrl.GetInstance().ShowFinishTaskInfo();
		}

		if (_Instance.IsOpenVR) {
			XKFinishTaskVRCtrl.GetInstance().ShowFinishTask();
		}
		else {
			LoadingGameMovie();
		}
	}
	
	public int GetFeiJiMarkIndex()
	{
		return FeiJiMarkIndex;
	}

	public int GetTanKeMarkIndex()
	{
		return TanKeMarkIndex;
	}

	public int GetCartoonCamMarkIndex()
	{
		return CartoonCamMarkIndex;
	}

	public static void AddCartoonTriggerSpawnList(XKTriggerRemoveNpc script)
	{
		if (script == null) {
			return;
		}

		if (CartoonTriggerSpawnList.Contains(script)) {
			return;
		}
		CartoonTriggerSpawnList.Add(script);
	}

	public static void ClearCartoonSpawnNpc()
	{
		Debug.Log("ClearCartoonSpawnNpc...");
		int max = CartoonTriggerSpawnList.Count;
		for (int i = 0; i < max; i++) {
			CartoonTriggerSpawnList[i].RemoveSpawnPointNpc();
		}
		CartoonTriggerSpawnList.Clear();
	}
	
	public static void AddNpcAmmoList(GameObject obj)
	{
		if (NpcAmmoList.Contains(obj)) {
			return;
		}
		CountNpcAmmo++;
		NpcAmmoList.Add(obj);
	}
	
	public static void RemoveNpcAmmoList(GameObject obj)
	{
		if (!NpcAmmoList.Contains(obj)) {
			return;
		}
		CountNpcAmmo--;
		NpcAmmoList.Remove(obj);
	}

	void CheckNpcAmmoList()
	{
		float dTime = Time.realtimeSinceStartup - TimeCheckNpcAmmo;
		if (dTime < 0.1f) {
			return;
		}
		TimeCheckNpcAmmo = Time.realtimeSinceStartup;
		
		int maxAmmo = AmmoNumMaxNpc;
		if (NpcAmmoList.Count <= maxAmmo) {
			return;
		}
		
		int num = NpcAmmoList.Count - maxAmmo;
		GameObject[] ammoArray = new GameObject[num];
		for (int i = 0; i < num; i++) {
			ammoArray[i] = NpcAmmoList[i];
		}
		
		NpcAmmoCtrl script = null;
		for (int i = 0; i < num; i++) {
			if (ammoArray[i] == null) {
				continue;
			}
			
			script = ammoArray[i].GetComponent<NpcAmmoCtrl>();
			if (script == null) {
				NpcAmmoList.Remove(ammoArray[i]);
				continue;
			}
			script.GameNeedRemoveAmmo();
		}
	}
	
	public void CheckIsCartoonShootTest()
	{
		if (!IsCartoonShootTest) {
			return;
		}
		
		XKTriggerEndCartoon.GetInstance().CloseStartCartoon(); //test
	}
	
	public void ChangeBoxColliderSize(Transform tran)
	{
		Vector3 scaleVal = tran.localScale;
		scaleVal.z = 1f;
		tran.localScale = scaleVal;

		BoxCollider box = tran.GetComponent<BoxCollider>();
		Vector3 sizeBox = box.size;
		sizeBox.z = TriggerBoxSize_Z;
		box.size = sizeBox;
	}

	public static void SetServerCameraTran(Transform tran)
	{
		if (ServerCameraPar != null) {
			ServerCameraPar.SetActive(false);
		}
		ServerCameraPar = tran.gameObject;

		if (!tran.gameObject.activeSelf) {
			tran.gameObject.SetActive(true);
		}

		if (tran.GetComponent<Camera>() != null && tran.GetComponent<Camera>().enabled) {
			tran.GetComponent<Camera>().enabled = false;
		}

		Transform serverCamTran = ServerCameraObj.transform;
		serverCamTran.parent = tran;
		serverCamTran.localPosition = Vector3.zero;
		serverCamTran.localEulerAngles = Vector3.zero;
		if (!ServerCameraObj.activeSelf) {
			ServerCameraObj.SetActive(true);
		}
	}

	public static void CheckObjDestroyThisTimed(GameObject obj)
	{
		if (GameMovieCtrl.IsActivePlayer) {
			return;
		}

		if (obj == null) {
			return;
		}

		DestroyThisTimed script = obj.GetComponent<DestroyThisTimed>();
		if (script == null) {
			script = obj.AddComponent<DestroyThisTimed>();
			script.TimeRemove = 1f;
			Debug.LogWarning("obj is not find DestroyThisTimed! name is "+obj.name);
		}
	}

	public static void ResetGameInfo()
	{
		DaoDanNumPOne = 0;
		DaoDanNumPTwo = 0;
		GaoBaoDanNumPOne = 0;
		GaoBaoDanNumPTwo = 0;
	}
	
	public static void SetParentTran(Transform tran, Transform parTran)
	{
		tran.parent = parTran;
		tran.localPosition = Vector3.zero;
		tran.localEulerAngles = Vector3.zero;
	}

	public static void HiddenMissionCleanup()
	{
		if (MissionCleanup == null || !MissionCleanup.gameObject.activeSelf) {
			return;
		}

//		if (Network.peerType == NetworkPeerType.Client) {
//			return;
//		}
		if (GameModeVal == GameMode.LianJi) {
			return;
		}
		MissionCleanup.gameObject.SetActive(false);
	}

	public static bool GetMissionCleanupIsActive()
	{
		return MissionCleanup.gameObject.activeSelf;
	}
	
	public static void SetActiveGameWaterObj(bool isActive)
	{
		if (_Instance == null) {
			return;
		}
		
		if (_Instance.GameWaterObj == null) {
			return;
		}
		
		if (isActive == _Instance.GameWaterObj.activeSelf) {
			return;
		}
		_Instance.GameWaterObj.SetActive(isActive);
	}
	
	public static void ActiveServerCameraTran()
	{
		Debug.Log("ActiveServerCameraTran...");
		ServerPortCameraCtrl.RandOpenServerPortCamera();
	}
	
	public static void AddYLDLv(YouLiangDianMoveCtrl script)
	{
		YouLiangDengJi levelValTmp = script.LevelVal;
		switch (levelValTmp) {
		case YouLiangDengJi.Level_1:
			if (YLDLvA.Contains(script)) {
				return;
			}
			YLDLvA.Add(script);
			break;
			
		case YouLiangDengJi.Level_2:
			if (YLDLvB.Contains(script)) {
				return;
			}
			YLDLvB.Add(script);
			break;
			
		case YouLiangDengJi.Level_3:
			if (YLDLvC.Contains(script)) {
				return;
			}
			YLDLvC.Add(script);
			break;
		}
	}

	public static YouLiangDianMoveCtrl GetYLDMoveScript(YouLiangDengJi levelValTmp)
	{
		int maxNum = 0;
		YouLiangDianMoveCtrl yldScript = null;
		switch (levelValTmp) {
		case YouLiangDengJi.Level_1:
			maxNum = YLDLvA.Count;
			for (int i = 0; i < maxNum; i++) {
				if (YLDLvA[i] != null && !YLDLvA[i].gameObject.activeSelf) {
					yldScript = YLDLvA[i];
					break;
				}
			}
			break;
			
		case YouLiangDengJi.Level_2:
			maxNum = YLDLvB.Count;
			for (int i = 0; i < maxNum; i++) {
				if (YLDLvB[i] != null && !YLDLvB[i].gameObject.activeSelf) {
					yldScript = YLDLvB[i];
					break;
				}
			}
			break;
			
		case YouLiangDengJi.Level_3:
			maxNum = YLDLvC.Count;
			for (int i = 0; i < maxNum; i++) {
				if (YLDLvC[i] != null && !YLDLvC[i].gameObject.activeSelf) {
					yldScript = YLDLvC[i];
					break;
				}
			}
			break;
		}
		
		if (yldScript == null) {
			yldScript = YouLiangDianUICtrl.GetInstance().SpawnYouLiangDianUI(levelValTmp);
			AddYLDLv(yldScript);
		}
		return yldScript;
	}

	public static void ClearNpcSpawnAllAmmo(GameObject npcObj)
	{
		bool isClearNpcAmmoTest = false;
		if (!isClearNpcAmmoTest) {
			return;
		}

		if (npcObj == null) {
			return;
		}
		
		XKCannonCtrl[] npcCannonScript = npcObj.GetComponentsInChildren<XKCannonCtrl>();
		int max = npcCannonScript.Length;
		if (max > 0) {
			for (int i = 0; i < max; i++) {
				if (npcCannonScript[i] != null) {
					npcCannonScript[i].ClearNpcAmmoList();
				}
			}
		}

		XkNpcZaiTiCtrl[] zaiTiScriptArray = npcObj.GetComponentsInChildren<XkNpcZaiTiCtrl>();
		max = zaiTiScriptArray.Length;
		if (max > 0) {
			for (int i = 0; i < max; i++) {
				if (zaiTiScriptArray[i] != null
				    && zaiTiScriptArray[i].IsTeShuFireNpc
				    && zaiTiScriptArray[i].TimeFireAmmo.Length > 0) {
					zaiTiScriptArray[i].ClearNpcAmmoList();
				}
			}
		}
		
		XKNpcAnimatorCtrl[] npcAniScript = npcObj.GetComponentsInChildren<XKNpcAnimatorCtrl>();
		max = npcAniScript.Length;
		if (max > 0) {
			for (int i = 0; i < max; i++) {
				if (npcAniScript[i] != null) {
					npcAniScript[i].ClearNpcAmmoList();
				}
			}
		}
	}

	public static void TestNetInfo()
	{
		bool isTestInfo = false;
		if (isTestInfo) {
			return;
		}
		float hVal = 25f;
		float wVal = Screen.width;
		int LenTest = MasterServer.PollHostList().Length;
		GUI.Box(new Rect(0f, 0f, wVal, hVal), "isServer "+Network.isServer+", isClient "+Network.isClient
		        +", PlayerSt "+GameTypeCtrl.PlayerPCState+", HostServerIP "+NetworkServerNet.HostServerIP);
		GUI.Box(new Rect(0f, hVal, wVal, hVal), "appType "+GameTypeCtrl.AppTypeStatic
		        +", NetPassword "+Network.incomingPassword+", LenTest "+LenTest+", ModeVal "+GameModeCtrl.ModeVal
		        +", TypeServer "+GameTypeCtrl.IsServer);

		int indexVal = 2;
		if (MasterServer.PollHostList().Length != 0) {
			HostData[] hostData = MasterServer.PollHostList();
			if (hostData.Length > 0) {
				for (int i = 0; i < hostData.Length; i++) {
					GUI.Box(new Rect(0f, hVal * (indexVal + i), wVal, hVal), "HostGameName: " + hostData[i].gameName
					        +", ip: "+hostData[i].comment);
					if (GameTypeCtrl.PlayerPCState == PlayerEnum.PlayerOne
					    && hostData[i].gameName == NetworkServerNet.HostNameP2
					    && NetworkServerNet.HostServerIP != hostData[i].comment) {
						NetworkServerNet.HostServerIP = hostData[i].comment;
					}
				}
			}
		}
	}

	void OnGUI()
	{
		TestNetInfo();
		if (IsCartoonShootTest || !IsShowDebugInfoBox) {
			return;
		}

		float hight = 20f;
		float width = 400;
		string infoA = "P1: ShiBing "+ShiBingNumPOne+" CheLiang "+CheLiangNumPOne+" ChuanBo "+ChuanBoNumPOne+" FeiJi "+FeiJiNumPOne;
		GUI.Box(new Rect(0f, 0f, width, hight), infoA);

		string infoB = "P2: ShiBing "+ShiBingNumPTwo+" CheLiang "+CheLiangNumPTwo+" ChuanBo "+ChuanBoNumPTwo+" FeiJi "+FeiJiNumPTwo;
		GUI.Box(new Rect(0f, hight, width, hight), infoB);

		string infoD = "P1: YouLiangAdd "+YouLiangDianAddPOne+" P2: YouLiangAdd "+YouLiangDianAddPTwo;
		GUI.Box(new Rect(0f, hight*2f, width, hight), infoD);

		string infoE = "PlayerYouLiang "+PlayerYouLiangCur;
		GUI.Box(new Rect(0f, hight*3f, width, hight), infoE);
		
		string infoF = "CountNpcAmmo "+CountNpcAmmo
				+", CountPlayerAmmo "+XKPlayerAutoFire.PlayerAmmoNumTest
				+", NpcNum "+NpcTranList.Count;
		GUI.Box(new Rect(0f, hight*4f, width, hight), infoF);

		XkPlayerCtrl playerScript = null;
		switch (GameModeVal) {
		case GameMode.DanJiFeiJi:
			playerScript = XkPlayerCtrl.GetInstanceFeiJi();
			break;
			
		case GameMode.DanJiTanKe:
			playerScript = XkPlayerCtrl.GetInstanceTanKe();
			break;
			
		case GameMode.LianJi:
			if (GameJiTaiSt == GameJiTaiType.FeiJiJiTai) {
				playerScript = XkPlayerCtrl.GetInstanceFeiJi();
			}
			else if (GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
				playerScript = XkPlayerCtrl.GetInstanceTanKe();
			}
			break;
		}

		if (playerScript != null && playerScript.GetAiPathScript() != null
		    && playerScript.GetAimMvToMarkTran() != null) {
			string infoH = "Path "+playerScript.GetAiPathScript().name + ", Mark "+playerScript.GetAimMvToMarkTran().name;
			XKPlayerCamera cameraScript = playerScript.GetPlayerCameraScript();
			if (cameraScript != null) {
				if (cameraScript.GetAimTram() != null) {
					infoH += ", CameraAim is "+cameraScript.GetAimTram().name;
				}
				else {
					infoH += ", CameraAim is null";
				}
			}
			GUI.Box(new Rect(0f, hight*5f, width, hight), infoH);
		}

		string infoI = "TestDTimeVal "+XkPlayerCtrl.TestDTimeVal.ToString("f3");
		if (GameModeVal == GameMode.DanJiFeiJi || GameModeVal == GameMode.DanJiTanKe) {
			infoI += ", TestSpeed " + XkPlayerCtrl.TestSpeed.ToString("f3");
		}
		else {
			
			if (XkGameCtrl.GameModeVal == GameMode.LianJi) {
				if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.FeiJiJiTai) {
					infoI += ", TestSpeed " + XkPlayerCtrl.TestSpeed.ToString("f3");
				}
				else if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
					infoI += ", TestSpeed " + XkPlayerCtrl.TestSpeed.ToString("f3");
				}
			}
		}
		GUI.Box(new Rect(0f, hight*6f, width, hight), infoI);

		string infoJ = "QiNangTK: Q "+XKPlayerDongGanCtrl.QiNangStateTK[0]
		+", H "+XKPlayerDongGanCtrl.QiNangStateTK[1]
		+", Z "+XKPlayerDongGanCtrl.QiNangStateTK[2]
		+", Y "+XKPlayerDongGanCtrl.QiNangStateTK[3];
		GUI.Box(new Rect(0f, hight*7f, width, hight), infoJ);

		string infoK = "QiNangFJ: Q "+XKPlayerDongGanCtrl.QiNangStateFJ[0]
		+", H "+XKPlayerDongGanCtrl.QiNangStateFJ[1]
		+", Z "+XKPlayerDongGanCtrl.QiNangStateFJ[2]
		+", Y "+XKPlayerDongGanCtrl.QiNangStateFJ[3];
		GUI.Box(new Rect(0f, hight*8f, width, hight), infoK);
		
		string infoL = "IsStartGame "+ScreenDanHeiCtrl.IsStartGame;
		if (XkPlayerCtrl.PlayerTranFeiJi == null) {
			infoL += ", PlayerFJ == null";
		}
		else {
			infoL += ", PlayerFJ != null";
		}

		if (XkPlayerCtrl.PlayerTranTanKe == null) {
			infoL += ", PlayerTK == null";
		}
		else {
			infoL += ", PlayerTK != null";
		}
		GUI.Box(new Rect(0f, hight*9f, width, hight), infoL);
	}
}