using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Text;

public class GameMovieCtrl : MonoBehaviour {
	public MovieTexture Movie;
	public GameObject MovieBJObj;
	public Rect[] RectArray;
	public Rect RectMv;
	public Texture[] TextureMv;
	public Texture[] TextureMv_Ch; //中文版UI.
	public Texture[] TextureMv_En; //英文版UI.
	/**
	 * TextureMvEnd[0] ->“坦克大战”.
	 * TextureMvEnd[1] ->“直升机大战”.
	 * TextureMvEnd[2] ->“联合战斗”.
	 */
	public Texture[] TextureMvEnd;
	public Texture TextureMvLG;
	public static bool IsTestLJGame; //测试联机小窗口游戏.
	AudioSource AudioSourceObj;
	bool IsStopMovie = true;
	/**
	 * 控制游戏为3屏或1屏输出.
	 * 单机坦克为1屏输出.
	 */
	public static bool IsThreeScreenGame = false;
	public static bool IsActivePlayer;
	public static bool IsOpenVR = true;
	float TimeVal;
	public static bool IsTestXiaoScreen = false;
	enum QualityLevelEnum
	{
		Fastest,
		Fast,
		Simple,
		Good,
		Beautiful,
		Fantastic
	}
	static GameMovieCtrl _instance;
	public static GameMovieCtrl GetInstance()
	{
		return _instance;
	}

	void Awake()
	{
		if (!pcvr.bIsHardWare) {
			string strInfo = HandleJson.GetInstance().ReadFromFileXml("TestOpenVR.info", "IsOpenVR");
			if (strInfo == null || strInfo == "") {
				strInfo = "1";
				HandleJson.GetInstance().WriteToFileXml("TestOpenVR.info", "IsOpenVR", "1");
			}
			
			if (strInfo == "1") {
				IsOpenVR = true;
			}
			else {
				IsOpenVR = false;
			}
		}
		
		
		if (CountMovie == 0) {
			if (!IsOpenVR) {
				GlassesCamObj.SetActive(false);
			}
			else {
				MovieUICtrlObj.SetActive(false);
			}
		}
		else {
			GlassesCamObj.SetActive(false);
		}

		if (CountMovie == 0 && !IsTestXiaoScreen && IsOpenVR) {
			ChangeWindowPos();
		}
	}

	// Use this for initialization
	void Start()
	{
		MovieRender = GetComponent<Renderer>();
		if (XKGlobalData.GetInstance() != null) {
			AudioListener.volume = (float)XKGlobalData.GameAudioVolume / 10f;
		}

		_instance = this;
		XkGameCtrl.ResetIsLoadingLevel();
		pcvr.CloseGunZhenDongDengJi();
		PlayerIPInfo = Network.player.ipAddress;
//		TimeLast = Time.realtimeSinceStartup;
		GameTextType gameTextVal = XKGlobalData.GetGameTextMode();
		//gameTextVal = GameTextType.English; //test.
		switch (gameTextVal) {
		case GameTextType.Chinese:
			TextureMv = TextureMv_Ch;
			break;
			
		case GameTextType.English:
			TextureMv = TextureMv_En;
			break;
		}

		if (AudioListCtrl.GetInstance() != null) {
			AudioListCtrl.GetInstance().CloseGameAudioBJ();
		}
		Cursor.visible = pcvr.bIsHardWare;
		LoadingGameCtrl.ResetLoadingInfo();
		Time.timeScale = 1.0f;
		CheckClientPortMovieInfo(1);
		AudioManager.Instance.SetParentTran(null);
		GameOverCtrl.IsShowGameOver = false;

		if (!IsTestLJGame) {
			IsActivePlayer = true;
			if (IsTestXiaoScreen) {
				Screen.SetResolution((int)(XkGameCtrl.ScreenWidth / 4), (int)(XkGameCtrl.ScreenHeight / 4), false); //test
			}
		}

		QualitySettings.SetQualityLevel((int)QualityLevelEnum.Fast);
		AudioSourceObj = transform.GetComponent<AudioSource>();
		Invoke("DelayResetIsLoadingLevel", 5f);
		CountMovie++;
		if (CountMovie == 1 && IsOpenVR) {
			Invoke("DelayPlayMovie", 3f);
		}
		else {
			PlayMovie();
		}
	}

	static int CountMovie;
	public GameObject GlassesCamObj;
	public GameObject MovieUICtrlObj;
	void DelayPlayMovie()
	{
		GlassesCamObj.SetActive(false);
		MovieUICtrlObj.SetActive(true);
		PlayMovie();
	}

	void CheckClientPortMovieInfo(int key = 0)
	{
		if (Time.frameCount % 30 != 0 && key == 0) {
			return;
		}
		RectMv.width = Screen.width;
		RectMv.height = Screen.height * 0.93f;
		
		if (Camera.main != null) {
			Vector3 posTmp = Camera.main.WorldToScreenPoint(InsertCoinTr[0].position);
			float testPX = posTmp.x - (RectArray[0].width / 2f);
			float testPY = posTmp.y + (RectArray[0].height * 0.7f);
			testPY = Screen.height - testPY;
			RectArray[0].x = testPX;
			RectArray[0].y = testPY;
			
			posTmp = Camera.main.WorldToScreenPoint(InsertCoinTr[1].position);
			testPX = posTmp.x - (RectArray[1].width / 2f);
			RectArray[1].x = testPX;
			RectArray[1].y = testPY;
			
			posTmp = Camera.main.WorldToScreenPoint(StartBtTr[0].position);
			testPX = posTmp.x - (RectArray[2].width / 2f);
			testPY = posTmp.y + (RectArray[2].height * 0.7f);
			testPY = Screen.height - testPY;
			RectArray[2].x = testPX;
			RectArray[2].y = testPY;
			
			posTmp = Camera.main.WorldToScreenPoint(StartBtTr[1].position);
			testPX = posTmp.x - (RectArray[3].width / 2f);
			RectArray[3].x = testPX;
			RectArray[3].y = testPY;
		}
	}

	void DelayResetIsLoadingLevel()
	{
		XkGameCtrl.ResetIsLoadingLevel();
		if (NetworkServerNet.GetInstance() != null) {
			NetworkServerNet.GetInstance().TryToCreateServer();
		}
	}

	Renderer MovieRender;
	bool IsPlayMovie;
	void PlayMovie()
	{
		if (!IsStopMovie) {
			return;
		}

		IsPlayMovie = true;
		if (MovieRender != null) {
			MovieRender.enabled = true;
			MovieRender.material.mainTexture = Movie;
		}
		IsStopMovie = false;
		Movie.loop = false;
		Movie.Play();
		TimeStartMV = Time.realtimeSinceStartup;
		
		if (AudioSourceObj != null) {
			AudioSourceObj.clip = Movie.audioClip;
			AudioSourceObj.enabled = true;
			AudioSourceObj.Play();
		}
	}

	public void StopPlayMovie()
	{
		if (IsStopMovie) {
			return;
		}
		IsStopMovie = true;
		Movie.Stop();
		if (AudioSourceObj != null) {
			AudioSourceObj.Stop();
			AudioSourceObj.enabled = false;
		}

		if (Network.peerType == NetworkPeerType.Server) {
			return;
		}
		gameObject.SetActive(false);
		//MovieBJObj.SetActive(false);
	}

	void ShowGameMvLG()
	{
		if (TimeMovieVal < 80f || IsPlayMvLogo) {
			return;
		}

		float swTmp = (float)Screen.width / 3f;
		if (!IsThreeScreenGame) {
			GUI.DrawTexture(new Rect(0f,0f, Screen.width, Screen.height), TextureMvLG);
		}
		else {
			GUI.DrawTexture(new Rect(0f, 0f, swTmp, Screen.height), TextureMvLG);
			GUI.DrawTexture(new Rect(swTmp, 0f, swTmp, Screen.height), TextureMvLG);
			GUI.DrawTexture(new Rect(swTmp * 2f, 0f, swTmp, Screen.height), TextureMvLG);
		}
	}

	float TimeMovieVal;
//	float TimeLast;
	public static string PlayerIPInfo = "192.168.0.2";
	bool IsFixServerPortIP;
	bool IsRestartGame;
	float TimeStartMV;
	int CountMV;
	bool IsPlayMvLogo;
	float TimeLogo;
	void ShowGameMovieLogo()
	{
		int indexMVLG = 0;
		float swTmp = (float)Screen.width / 3f;
		AppGameType gameType = GameTypeCtrl.AppTypeStatic;
		switch (gameType) {
		case AppGameType.LianJiFeiJi:
		case AppGameType.LianJiTanKe:
		case AppGameType.LianJiServer:
			indexMVLG = 2;
			break;
		case AppGameType.DanJiFeiJi:
			indexMVLG = 1;
			break;
		case AppGameType.DanJiTanKe:
			indexMVLG = 0;
			break;
		}
		if (!IsThreeScreenGame) {
			GUI.DrawTexture(new Rect(0f,0f, Screen.width, Screen.height), TextureMvEnd[indexMVLG]);
		}
		else {
			GUI.DrawTexture(new Rect(0f, 0f, swTmp, Screen.height), TextureMvEnd[indexMVLG]);
			GUI.DrawTexture(new Rect(swTmp, 0f, swTmp, Screen.height), TextureMvEnd[indexMVLG]);
			GUI.DrawTexture(new Rect(swTmp * 2f, 0f, swTmp, Screen.height), TextureMvEnd[indexMVLG]);
		}
	}

	float TimeDelayHiddenMvLogo = -10f;
	void OnGUI()
	{
		if (MovieRender != null) {
			return;
		}
		
		if (!IsPlayMovie) {
			return;
		}

		if (IsPlayMvLogo) {
			ShowGameMovieLogo();
			if (Time.realtimeSinceStartup - TimeLogo >= 3f) {
				IsPlayMvLogo = false;
				TimeStartMV = Time.realtimeSinceStartup;
				TimeDelayHiddenMvLogo = Time.realtimeSinceStartup;
				Movie.Play();
				AudioSourceObj.Play();
				return;
			}
			return;
		}

		if (Time.realtimeSinceStartup - TimeDelayHiddenMvLogo < 0.2f) {
			ShowGameMovieLogo();
			return;
		}

		if (IsStopMovie) {
			return;
		}
		CheckClientPortMovieInfo();
		GUI.DrawTexture(RectMv, Movie, ScaleMode.StretchToFill);

		TimeVal += Time.deltaTime;
		int timeTmp = (int)TimeVal;
		if (!XKGlobalData.IsFreeMode) {
			if (timeTmp % 2 == 0) {
				if (XKGlobalData.CoinPlayerOne < XKGlobalData.GameNeedCoin) {
					GUI.DrawTexture(RectArray[0], TextureMv[0]);
				}
				else {
					GUI.DrawTexture(RectArray[2], TextureMv[1]);
				}
				
				if (XKGlobalData.CoinPlayerTwo < XKGlobalData.GameNeedCoin) {
					GUI.DrawTexture(RectArray[1], TextureMv[0]);
				}
				else {
					GUI.DrawTexture(RectArray[3], TextureMv[1]);
				}
			}
			else {
				if (XKGlobalData.CoinPlayerOne >= XKGlobalData.GameNeedCoin) {
					GUI.DrawTexture(RectArray[2], TextureMv[2]);
				}
				
				if (XKGlobalData.CoinPlayerTwo >= XKGlobalData.GameNeedCoin) {
					GUI.DrawTexture(RectArray[3], TextureMv[2]);
				}
			}
		}
		else {
			if (timeTmp % 2 == 0) {
				GUI.DrawTexture(RectArray[2], TextureMv[1]);
				GUI.DrawTexture(RectArray[3], TextureMv[1]);
			}
			else {
				GUI.DrawTexture(RectArray[2], TextureMv[2]);
				GUI.DrawTexture(RectArray[3], TextureMv[2]);
			}
		}
		
		XkGameCtrl.TestNetInfo();
		if (Camera.main != null && IsThreeScreenGame) {
			Vector3 posTmp = Camera.main.WorldToScreenPoint(InsertCoinTr[0].position);
			float testPX = posTmp.x - (RectArray[0].width / 2f);
			float testPY = posTmp.y + (RectArray[0].height * 0.7f);
			testPY = Screen.height - testPY;
			RectArray[0].x = testPX;
			RectArray[0].y = testPY;

			posTmp = Camera.main.WorldToScreenPoint(InsertCoinTr[1].position);
			testPX = posTmp.x - (RectArray[1].width / 2f);
			RectArray[1].x = testPX;
			RectArray[1].y = testPY;

			posTmp = Camera.main.WorldToScreenPoint(StartBtTr[0].position);
			testPX = posTmp.x - (RectArray[2].width / 2f);
			testPY = posTmp.y + (RectArray[2].height * 0.7f);
			testPY = Screen.height - testPY;
			RectArray[2].x = testPX;
			RectArray[2].y = testPY;
			
			posTmp = Camera.main.WorldToScreenPoint(StartBtTr[1].position);
			testPX = posTmp.x - (RectArray[3].width / 2f);
			RectArray[3].x = testPX;
			RectArray[3].y = testPY;
		}

		TimeMovieVal = Time.realtimeSinceStartup - TimeStartMV;
		if (TimeMovieVal >= Movie.duration + 3f) {
			TimeLogo = Time.realtimeSinceStartup;
			IsPlayMvLogo = true;
			Movie.Stop();
			AudioSourceObj.Stop();

			TimeStartMV = Time.realtimeSinceStartup;
			CountMV++;
		}
		
		ShowGameMvLG();
		if (!pcvr.bIsHardWare) {
			string mvInfo = "mvTime "+Movie.duration+", CountMV "+CountMV+", timeVal "+TimeMovieVal.ToString("f2");
			GUI.Label(new Rect(10f, 0f, Screen.width * 0.5f, 30f), mvInfo);
		}
		XKGameFPSCtrl.DrawGameFPS();
	}
	public Transform[] InsertCoinTr;
	public Transform[] StartBtTr;

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
	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
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
	const int HWND_TOP = 0;
	const int HWND_BOTTOM = 1;
	int SM_CXSCREEN = 0;
	int SM_CYSCREEN = 1;
	static bool IsChangePos = false;

	void ChangeWindowPos()  
	{
		if (IsChangePos) {
			return;
		}
		IsChangePos = true;

#if UNITY_EDITOR
		return;
#endif

		if (!IsTestXiaoScreen) {
			//Screen.SetResolution((int)XkGameCtrl.ScreenWidth, (int)XkGameCtrl.ScreenHeight, false);
			MakeGameFullScreenView();
		}
	}
	
	[DllImport("kernel32.dll")]
	static extern uint GetCurrentThreadId();

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
	
	public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
	
	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);
	
	private const string UnityWindowClassName = "UnityWndClass";
	
	private IntPtr windowHandle = IntPtr.Zero;

	void MakeGameFullScreenView()
	{
		if (IsTestXiaoScreen) {
			return;
		}

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
		
		if (windowHandle != IntPtr.Zero) {
//			int iScreenW = (int)XkGameCtrl.ScreenWidth;
//			int iScreenH = (int)XkGameCtrl.ScreenHeight;
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
}