using UnityEngine;
using System.Collections;
using System;

public class pcvr : MonoBehaviour {
	public static bool bIsHardWare = false;
	public static Vector3 CrossPositionOne;
	public static Vector3 CrossPositionTwo;
	public static bool IsJiaoYanHid;
	public static bool IsSlowLoopCom;
	public static bool IsTestHardWare;
	private static int HID_BUF_LEN = 23;
	
	private static int openPCVRFlag = 1;
	private System.IntPtr gTestHidPtr;
	float lastUpTime = 0.0f;
	
//	private float mTimeGetMSG = 0f;
//	private float mTimeOpenPCVR = 0f;
	
	public static float mGetSteer = 0f;
	public static Vector3 MousePositionP1;
	public static Vector3 MousePositionP2;
	public static bool IsClickFireBtDown;

	public static uint gOldCoinNum = 0;
	public int CoinNumCurrentP1 = 0;
	public int CoinNumCurrentP2 = 0;
	
	public static bool bPlayerHitTaBan_P1 = false;
	public static bool bPlayerHitTaBan_P2 = false;
	
	public static float VerticalVal;
	public static float TanBanDownCount_P1 = 0;
	public static float TanBanDownCount_P2 = 0;
	
	public static LedState StartLightStateP1 = LedState.Mie;
	public static LedState StartLightStateP2 = LedState.Mie;
	public static bool IsOpenStartLightP1 = false;
	public static bool IsOpenStartLightP2 = false;
	int subCoinNum = 0;
	
	static string FileName;
	static HandleJson HandleJsonObj;
	
	public static uint SteerValMax = 999999;
	public static uint SteerValCen = 1765;
	public static uint SteerValMin = 0;
	public static uint SteerValCur;
	public static uint SteerDisVal;
	
	public static uint TaBanValMax;
	public static uint TaBanValMin;
	public static uint TaBanValCur = 30000;
	public static uint TaBanDisVal;
	
	public static uint CrossPosXMaxP1;
	public static uint CrossPosXMinP1;
	public static uint CrossPosXCurP1;
	
	public static uint CrossPosYMaxP1;
	public static uint CrossPosYMinP1;
	public static uint CrossPosYCurP1;
	
	public static uint CrossPosDisXP1;
	public static uint CrossPosDisYP1;
	
	static uint CrossPxP1_1;
	static uint CrossPxP1_2;
	static uint CrossPxP1_3;
	static uint CrossPxP1_4;
	
	static uint CrossPyP1_1;
	static uint CrossPyP1_2;
	static uint CrossPyP1_3;
	static uint CrossPyP1_4;

	public static uint CrossPosXMaxP2;
	public static uint CrossPosXMinP2;
	public static uint CrossPosXCurP2;
	
	public static uint CrossPosYMaxP2;
	public static uint CrossPosYMinP2;
	public static uint CrossPosYCurP2;
	
	public static uint CrossPosDisXP2;
	public static uint CrossPosDisYP2;
	
	static uint CrossPxP2_1;
	static uint CrossPxP2_2;
	static uint CrossPxP2_3;
	static uint CrossPxP2_4;
	
	static uint CrossPyP2_1;
	static uint CrossPyP2_2;
	static uint CrossPyP2_3;
	static uint CrossPyP2_4;

	public static bool IsOpenShuiBeng = false;
	public static PcvrShuiBengState ShuiBengState = PcvrShuiBengState.Level_1;
	static uint TanBanCenterNum = 30000;
	static float SubTaBanCount = 2.0f;
	
	bool IsSubPlayerCoin = false;
	bool IsSubCoinP1 = false;
	bool IsSubCoinP2 = false;
	static bool IsFireBtDownP1 = false;
	static bool IsFireBtDownP2 = false;
	static bool IsDaoDanBtDownP1 = false;
	static bool IsDaoDanBtDownP2 = false;
	static public bool bPlayerStartKeyDownP1 = false;
	static public bool bPlayerStartKeyDownP2 = false;
	static public bool IsClickDongGanBtOne = false;
	static public bool IsClickDongGanBtTwo = false;
	private bool bSetEnterKeyDown = false;
	static public bool bSetMoveKeyDown = false;
//	static public bool bIsTouBiBtDown = false;
	static bool IsFanZhuangTaBan = false;
	public static uint CoinCurPcvr;
	//9500.0f -> maxSpeed(60km/h)
	public static float PcvrTanBanValTmp = 1.8f * 9500.0f;
	public static int QiangZhenDongP1;
	public static int QiangZhenDongP2;
	static pcvr Instance;
	public static pcvr GetInstance()
	{
		if (Instance == null) {
			GameObject obj = new GameObject("_PCVR");
			DontDestroyOnLoad(obj);
			Instance = obj.AddComponent<pcvr>();
			if (bIsHardWare) {
				obj.AddComponent<MyCOMDevice>();
			}
			
			NetworkServerNet.GetInstance();
		}
		return Instance;
	}
	
	// Use this for initialization
	void Awake()
	{
		if (Application.loadedLevel == (int)GameLevel.Movie) {
			AudioManager.Instance.SetParentTran(transform);
		}
	}

	void Start()
	{
		InitJiaoYanMiMa();
		lastUpTime = Time.realtimeSinceStartup;
		InitHandleJsonInfo();
		InitSteerInfo();
		InitTaBanInfo();
		InitCrossPosInfoPOne();
		InitCrossPosInfoPTwo();
	}
	
	// Update is called once per frame
	void Update()
	{
		if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiServer) {
			return;
		}

		CheckCrossPositionPOne();
		CheckCrossPositionPTwo();
		if (!bIsHardWare) {
			return;
		}
		
		float dTime = Time.realtimeSinceStartup - lastUpTime;
		if (IsJiaoYanHid) {
			if (dTime < 0.1f) {
				return;
			}
		}
		else {
			if (dTime < 0.03f) {
				return;
			}
		}
		lastUpTime = Time.realtimeSinceStartup;
		CheckIsPlayerActivePcvr();
		
		getMessage();
		sendMessage();
	}

//	static byte ReadHead_1 = 0x01;
//	static byte ReadHead_2 = 0x55;
	static byte WriteHead_1 = 0x02;
	static byte WriteHead_2 = 0x55;
	static byte WriteEnd_1 = 0x0d;
	static byte WriteEnd_2 = 0x0a;
	byte EndRead_1 = 0x41;
	byte EndRead_2 = 0x42;
	/**
****************.显示器.****************
QiNangArray[0]			QiNangArray[1]
QiNangArray[3]			QiNangArray[2]
	 */
	public static byte[] QiNangArray = {0, 0, 0, 0};
	public static void CloseAllQiNangArray()
	{
		QiNangArray[0] = 0;
		QiNangArray[1] = 0;
		QiNangArray[2] = 0;
		QiNangArray[3] = 0;
	}
	
	static bool IsOpenQiNangQian;
	static bool IsOpenQiNangHou;
	static bool IsOpenQiNangZuo;
	static bool IsOpenQiNangYou;
	/**
	 * qn1		qn2
	 * qn4		qn3
	 */
	public static void OpenQiNangQian()
	{
		if (IsOpenQiNangQian) {
			return;
		}
		IsOpenQiNangQian = true;
		QiNangArray[0] = 1;
		QiNangArray[1] = 1;
	}
	
	public static void CloseQiNangQian()
	{
		if (!IsOpenQiNangQian) {
			return;
		}
		IsOpenQiNangQian = false;
		QiNangArray[0] = 0;
		QiNangArray[1] = 0;
	}
	
	public static void OpenQiNangHou()
	{
		if (IsOpenQiNangHou) {
			return;
		}
		IsOpenQiNangHou = true;
		QiNangArray[2] = 1;
		QiNangArray[3] = 1;
	}
	
	public static void CloseQiNangHou()
	{
		if (!IsOpenQiNangHou) {
			return;
		}
		IsOpenQiNangHou = false;
		QiNangArray[2] = 0;
		QiNangArray[3] = 0;
	}
	
	public static void OpenQiNangZuo()
	{
		if (IsOpenQiNangZuo) {
			return;
		}
		IsOpenQiNangZuo = true;
		QiNangArray[0] = 1;
		QiNangArray[3] = 1;
	}
	
	public static void CloseQiNangZuo()
	{
		if (!IsOpenQiNangZuo) {
			return;
		}
		IsOpenQiNangZuo = false;
		QiNangArray[0] = 0;
		QiNangArray[3] = 0;
	}
	
	public static void OpenQiNangYou()
	{
		if (IsOpenQiNangYou) {
			return;
		}
		IsOpenQiNangYou = true;
		QiNangArray[1] = 1;
		QiNangArray[2] = 1;
	}
	
	public static void CloseQiNangYou()
	{
		if (!IsOpenQiNangYou) {
			return;
		}
		IsOpenQiNangYou = false;
		QiNangArray[1] = 0;
		QiNangArray[2] = 0;
	}
	
	public static bool IsPlayerHitShake;
	public void OnPlayerHitShake()
	{
		if (IsPlayerHitShake) {
			return;
		}
		IsPlayerHitShake = true;
		//Debug.Log("OnPlayerHitShake...");
		StartCoroutine(PcvrQiNangHitShake());
	}
	
	void ClosePlayerHitShake()
	{
		if (!IsPlayerHitShake) {
			return;
		}
		IsPlayerHitShake = false;
		//Debug.Log("ClosePlayerHitShake...");
	}
	
	IEnumerator PcvrQiNangHitShake()
	{
		bool isHitShake = true;
		int count = 0;
		do {
			if (count % 2 == 0) {
				OpenQiNangZuo();
				CloseQiNangYou();
			}
			else {
				OpenQiNangYou();
				CloseQiNangZuo();
			}
			yield return new WaitForSeconds(0.25f);
			
			if (count >= 4) {
				isHitShake = false;
				ClosePlayerHitShake();
				yield break;
			}
			count++;
		} while (isHitShake);
	}
	
	void sendMessage()
	{
		if (!MyCOMDevice.IsFindDeviceDt) {
			return;
		}
		
		byte []buffer;
		buffer = new byte[HID_BUF_LEN];
		buffer[0] = WriteHead_1;
		buffer[1] = WriteHead_2;
		buffer[HID_BUF_LEN - 2] = WriteEnd_1;
		buffer[HID_BUF_LEN - 1] = WriteEnd_2;
		for (int i = 4; i < HID_BUF_LEN - 2; i++) {
			buffer[i] = (byte)(UnityEngine.Random.Range(0, 10000) % 256);
		}
		
		if (IsSubPlayerCoin) {
			buffer[2] = 0xaa;
			buffer[3] = (byte)subCoinNum;
		}
		
		switch (StartLightStateP1) {
		case LedState.Liang:
			buffer[4] |= 0x01;
			break;
			
		case LedState.Shan:
			buffer[4] |= 0x01;
			break;
			
		case LedState.Mie:
			buffer[4] &= 0xfe;
			break;
		}
		
		switch (StartLightStateP2) {
		case LedState.Liang:
			buffer[4] |= 0x02;
			break;
			
		case LedState.Shan:
			buffer[4] |= 0x02;
			break;
			
		case LedState.Mie:
			buffer[4] &= 0xfd;
			break;
		}
		
		if (DongGanState == 1 || HardwareCheckCtrl.IsTestHardWare) {
			buffer[5] = (byte)(QiNangArray[0]
			                   + (QiNangArray[1] << 1)
			                   + (QiNangArray[2] << 2)
			                   + (QiNangArray[3] << 3)
			                   + (QiNangArray[0] << 4)
			                   + (QiNangArray[1] << 5)
			                   + (QiNangArray[2] << 6)
			                   + (QiNangArray[3] << 7));
		}
		else {
			buffer[5] = 0x00;
		}
		
//		if (IsOpenShuiBeng) {
//			switch (ShuiBengState) {
//			case PcvrShuiBengState.Level_1:
//				buffer[5] |= 0x10;
//				break;
//				
//			case PcvrShuiBengState.Level_2:
//				buffer[5] |= 0x10;
//				break;
//			}
//		}
//		else {
//			buffer[5] &= 0x0f;
//		}
		
		if (IsJiaoYanHid) {
			for (int i = 0; i < 4; i++) {
				buffer[i + 10] = JiaoYanMiMa[i];
			}
			
			for (int i = 0; i < 4; i++) {
				buffer[i + 14] = JiaoYanDt[i];
			}
		}
		else {
			RandomJiaoYanMiMaVal();
			for (int i = 0; i < 4; i++) {
				buffer[i + 10] = JiaoYanMiMaRand[i];
			}
			
			//0x41 0x42 0x43 0x44
			for (int i = 15; i < 18; i++) {
				buffer[i] = (byte)UnityEngine.Random.Range(0x00, 0x40);
			}
			buffer[14] = 0x00;
			
			for (int i = 15; i < 18; i++) {
				buffer[14] ^= buffer[i];
			}
		}

		buffer[8] = (byte)QiangZhenDongP1;
		buffer[9] = (byte)QiangZhenDongP2;
		
		buffer[6] = 0x00;
		for (int i = 2; i <= 11; i++) {
			if (i == 6) {
				continue;
			}
			buffer[6] ^= buffer[i];
		}

		buffer[19] = 0x00;
		for (int i = 0; i < HID_BUF_LEN; i++) {
			if (i == 19) {
				continue;
			}
			buffer[19] ^= buffer[i];
		}
		MyCOMDevice.ComThreadClass.WriteByteMsg = buffer;
	}
	
	void getMessage()
	{
		if (!MyCOMDevice.ComThreadClass.IsReadComMsg) {
			return;
		}
		
		if (MyCOMDevice.ComThreadClass.IsReadMsgComTimeOut) {
			return;
		}
		
		if (MyCOMDevice.ComThreadClass.ReadByteMsg.Length < (MyCOMDevice.ComThreadClass.BufLenRead - MyCOMDevice.ComThreadClass.BufLenReadEnd)) {
			//Debug.Log("ReadBufLen was wrong! len is "+MyCOMDevice.ComThreadClass.ReadByteMsg.Length);
			return;
		}
		
		if (IsJiOuJiaoYanFailed) {
			return;
		}
		
		if ((MyCOMDevice.ComThreadClass.ReadByteMsg[22]&0x01) == 0x01) {
			JiOuJiaoYanCount++;
			if (JiOuJiaoYanCount >= JiOuJiaoYanMax && !IsJiOuJiaoYanFailed) {
				IsJiOuJiaoYanFailed = true;
				//JiOuJiaoYanFailed
			}
		}
		//		//IsJiOuJiaoYanFailed = true; //test
		//		
		byte tmpVal = 0x00;
		string testA = "";
		for (int i = 2; i < (MyCOMDevice.ComThreadClass.BufLenRead - 4); i++) {
			if (i == 18 || i == 21) {
				continue;
			}
			testA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
			tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
		}
		tmpVal ^= EndRead_1;
		tmpVal ^= EndRead_2;
		testA += EndRead_1 + " ";
		testA += EndRead_2 + " ";
		
		if (tmpVal != MyCOMDevice.ComThreadClass.ReadByteMsg[21]) {
			if (MyCOMDevice.ComThreadClass.IsStopComTX) {
				return;
			}
			MyCOMDevice.ComThreadClass.IsStopComTX = true;
			//			ScreenLog.Log("testA: "+testA);
			//			ScreenLog.LogError("tmpVal: "+tmpVal.ToString("X2")+", byte[21] "+MyCOMDevice.ComThreadClass.ReadByteMsg[21].ToString("X2"));
			//			ScreenLog.Log("byte21 was wrong!");
			return;
		}
		
		if (IsJiaoYanHid) {
			tmpVal = 0x00;
			//string testStrA = MyCOMDevice.ComThreadClass.ReadByteMsg[30].ToString("X2") + " ";
			//			string testStrB = "";
			//			string testStrA = "";
			//			for (int i = 0; i < MyCOMDevice.ComThreadClass.ReadByteMsg.Length; i++) {
			//				testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
			//			}
			//			ScreenLog.Log("readStr: "+testStrA);
			//
			//			for (int i = 0; i < JiaoYanDt.Length; i++) {
			//				testStrB += JiaoYanDt[i].ToString("X2") + " ";
			//			}
			//			ScreenLog.Log("GameSendDt: "+testStrB);
			//
			//			string testStrC = "";
			//			for (int i = 0; i < JiaoYanDt.Length; i++) {
			//				testStrC += JiaoYanMiMa[i].ToString("X2") + " ";
			//			}
			//			ScreenLog.Log("GameSendMiMa: "+testStrC);
			
			for (int i = 11; i < 14; i++) {
				tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
				//testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
			}
			
			if (tmpVal == MyCOMDevice.ComThreadClass.ReadByteMsg[10]) {
				bool isJiaoYanDtSucceed = false;
				tmpVal = 0x00;
				for (int i = 15; i < 18; i++) {
					tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
				}
				
				//校验2...
				if ( tmpVal == MyCOMDevice.ComThreadClass.ReadByteMsg[14]
				    && (JiaoYanDt[1]&0xef) == MyCOMDevice.ComThreadClass.ReadByteMsg[15]
				    && (JiaoYanDt[2]&0xfe) == MyCOMDevice.ComThreadClass.ReadByteMsg[16]
				    && (JiaoYanDt[3]|0x28) == MyCOMDevice.ComThreadClass.ReadByteMsg[17] ) {
					isJiaoYanDtSucceed = true;
				}
				
				JiaoYanCheckCount++;
				if (isJiaoYanDtSucceed) {
					//JiaMiJiaoYanSucceed
					OnEndJiaoYanIO(JIAOYANENUM.SUCCEED);
					//ScreenLog.Log("JMJYCG...");
				}
				else {
					if (JiaoYanCheckCount > 0) {
						OnEndJiaoYanIO(JIAOYANENUM.FAILED);
					}
					//					testStrA = "";
					//					for (int i = 0; i < 23; i++) {
					//						testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
					//					}
					//
					//					testStrB = "";
					//					for (int i = 14; i < 18; i++) {
					//						testStrB += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
					//					}
					//					
					//					string testStrD = "";
					//					for (int i = 0; i < 4; i++) {
					//						testStrD += JiaoYanDt[i].ToString("X2") + " ";
					//					}
					//					ScreenLog.Log("ReadByte[0 - 22] "+testStrA);
					//					ScreenLog.Log("ReadByte[14 - 17] "+testStrB);
					//					ScreenLog.Log("SendByte[14 - 17] "+testStrD);
					//					ScreenLog.LogError("校验数据错误!");
				}
			}
			//			else {
			//				testStrB = "byte[10] "+MyCOMDevice.ComThreadClass.ReadByteMsg[10].ToString("X2")+" "
			//					+", tmpVal "+tmpVal.ToString("X2");
			//				ScreenLog.Log("ReadByte[10 - 13] "+testStrA);
			//				ScreenLog.Log(testStrB);
			//				ScreenLog.LogError("ReadByte[10] was wrong!");
			//			}
		}
		
		int len = MyCOMDevice.ComThreadClass.ReadByteMsg.Length;
		uint[] readMsg = new uint[len];
		for (int i = 0; i < len; i++) {
			readMsg[i] = MyCOMDevice.ComThreadClass.ReadByteMsg[i];
		}
		keyProcess(readMsg);
	}
	
	/**
	 * DongGanState == 1 -> 打开动感.
	 * DongGanState == 0 -> 关闭动感.
	 */
	public static byte DongGanState = 1;
	void keyProcess(uint []buffer)
	{
		MousePositionP1.x = ((buffer[2] & 0x0f) << 8) + buffer[3];
		MousePositionP1.y = ((buffer[4] & 0x0f) << 8) + buffer[5];
		MousePositionP2.x = ((buffer[6] & 0x0f) << 8) + buffer[7];
		MousePositionP2.y = ((buffer[8] & 0x0f) << 8) + buffer[9];                                                           
		
		//game coinInfo
		CoinCurPcvr = buffer[18];
		uint coinP1 = CoinCurPcvr & 0x0f;
		uint coinP2 = (CoinCurPcvr & 0xf0) >> 4;
		//coinP2 = coinP1; //test
		bool isCoinPCBOneTest = false; //test
		if (IsSubPlayerCoin) {
			if (isCoinPCBOneTest) {
				if (CoinCurPcvr == 0) {
					IsSubPlayerCoin = false;
				}
			}
			else {
				if (coinP1 == 0 && IsSubCoinP1) {
					IsSubCoinP1 = false;
					IsSubPlayerCoin = false;
				}
				
				if (coinP2 == 0 && IsSubCoinP2) {
					IsSubCoinP2 = false;
					IsSubPlayerCoin = false;
				}
			}
		}
		else {
			if (isCoinPCBOneTest) {
				if (CoinCurPcvr > 0 && CoinCurPcvr < 256) {
					CoinNumCurrentP1 += (int)CoinCurPcvr;
					//XKGlobalData.SetCoinPlayerOne(CoinNumCurrentP1);
					
					switch (GameTypeCtrl.PlayerPCState) {
					case PlayerEnum.PlayerOne:
						XKGlobalData.SetCoinPlayerOne(CoinNumCurrentP1);
						break;
					case PlayerEnum.PlayerTwo:
						XKGlobalData.SetCoinPlayerTwo(CoinNumCurrentP1);
						break;
					}
					SubPcvrCoin((int)CoinCurPcvr);
				}
			}
			else {
				if (coinP1 > 0 && coinP1 < 256) {
					IsSubCoinP1 = true;
					CoinNumCurrentP1 += (int)coinP1;
					//XKGlobalData.SetCoinPlayerOne(CoinNumCurrentP1);
					
					switch (GameTypeCtrl.PlayerPCState) {
					case PlayerEnum.PlayerOne:
						XKGlobalData.SetCoinPlayerOne(CoinNumCurrentP1 + CoinNumCurrentP2);
						break;
					case PlayerEnum.PlayerTwo:
						XKGlobalData.SetCoinPlayerTwo(CoinNumCurrentP1 + CoinNumCurrentP2);
						break;
					}
					SubPcvrCoin((int)(CoinCurPcvr & 0x0f));
				}
				
				if (coinP2 > 0 && coinP2 < 256) {
					IsSubCoinP2 = true;
					CoinNumCurrentP2 += (int)coinP2;
					//XKGlobalData.SetCoinPlayerTwo(CoinNumCurrentP2);

					switch (GameTypeCtrl.PlayerPCState) {
					case PlayerEnum.PlayerOne:
						XKGlobalData.SetCoinPlayerOne(CoinNumCurrentP1 + CoinNumCurrentP2);
						break;
					case PlayerEnum.PlayerTwo:
						XKGlobalData.SetCoinPlayerTwo(CoinNumCurrentP1 + CoinNumCurrentP2);
						break;
					}
					SubPcvrCoin((int)(CoinCurPcvr & 0xf0));
				}
			}
		}
		
		//game startBt, hitNpcBt or jiaoZhunBt
		if( !bPlayerStartKeyDownP1 && (buffer[19]&0x04) == 0x04 )
		{
			//ScreenLog.Log("gameP1 startBt down!");
			bPlayerStartKeyDownP1 = true;
			//InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.DOWN );
			
			switch (GameTypeCtrl.PlayerPCState) {
			case PlayerEnum.PlayerOne:
				InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.DOWN );
				break;
			case PlayerEnum.PlayerTwo:
				InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.DOWN );
				break;
			default:
				InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.DOWN );
				break;
			}
		}
		else if ( bPlayerStartKeyDownP1 && (buffer[19]&0x04) == 0x00 )
		{
			//ScreenLog.Log("gameP1 startBt up!");
			bPlayerStartKeyDownP1 = false;
			//InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.UP );
			
			switch (GameTypeCtrl.PlayerPCState) {
			case PlayerEnum.PlayerOne:
				InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.UP );
				break;
			case PlayerEnum.PlayerTwo:
				InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.UP );
				break;
			default:
				InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.UP );
				break;
			}
		}
		
		//game startBt, hitNpcBt or jiaoZhunBt
		if( !bPlayerStartKeyDownP2 && (buffer[19]&0x08) == 0x08 )
		{
			//ScreenLog.Log("gameP2 startBt down!");
			bPlayerStartKeyDownP2 = true;
			//InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.DOWN );
			
			switch (GameTypeCtrl.PlayerPCState) {
			case PlayerEnum.PlayerOne:
				InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.DOWN );
				break;
			case PlayerEnum.PlayerTwo:
				InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.DOWN );
				break;
			default:
				InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.DOWN );
				break;
			}
		}
		else if ( bPlayerStartKeyDownP2 && (buffer[19]&0x08) == 0x00 )
		{
			//ScreenLog.Log("gameP2 startBt up!");
			bPlayerStartKeyDownP2 = false;
			//InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.UP );
			
			switch (GameTypeCtrl.PlayerPCState) {
			case PlayerEnum.PlayerOne:
				InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.UP );
				break;
			case PlayerEnum.PlayerTwo:
				InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.UP );
				break;
			default:
				InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.UP );
				break;
			}
		}
		
		//DongGanBt
		if( !IsClickDongGanBtOne && (buffer[19]&0x10) == 0x10 )
		{
			IsClickDongGanBtOne = true;
			InputEventCtrl.GetInstance().ClickStopDongGanBtOne( ButtonState.DOWN );
		}
		else if ( IsClickDongGanBtOne && (buffer[19]&0x10) == 0x00 )
		{
			IsClickDongGanBtOne = false;
			InputEventCtrl.GetInstance().ClickStopDongGanBtOne( ButtonState.UP );
		}

		if( !IsFireBtDownP1 && (buffer[19]&0x40) == 0x40 )
		{
			IsFireBtDownP1 = true;
			InputEventCtrl.IsClickFireBtOneDown = true;
			//ScreenLog.Log("game fireBtP1 down!");
			InputEventCtrl.GetInstance().ClickFireBtOne( ButtonState.DOWN );
		}
		else if( IsFireBtDownP1 && (buffer[19]&0x40) == 0x00 )
		{
			IsFireBtDownP1 = false;
			InputEventCtrl.IsClickFireBtOneDown = false;
			//ScreenLog.Log("game fireBtP1 up!");
			InputEventCtrl.GetInstance().ClickFireBtOne( ButtonState.UP );
		}

		if( !IsDaoDanBtDownP1 && (buffer[19]&0x80) == 0x80 )
		{
			IsDaoDanBtDownP1 = true;
			InputEventCtrl.GetInstance().ClickDaoDanBtOne( ButtonState.DOWN );
		}
		else if( IsDaoDanBtDownP1 && (buffer[19]&0x80) == 0x00 )
		{
			IsDaoDanBtDownP1 = false;
			InputEventCtrl.GetInstance().ClickDaoDanBtOne( ButtonState.UP );
		}

		if( !IsFireBtDownP2 && (buffer[20]&0x01) == 0x01 )
		{
			IsFireBtDownP2 = true;
			InputEventCtrl.IsClickFireBtTwoDown = true;
			//ScreenLog.Log("game fireBtP2 down!");
			InputEventCtrl.GetInstance().ClickFireBtTwo( ButtonState.DOWN );
		}
		else if( IsFireBtDownP2 && (buffer[20]&0x01) == 0x00 )
		{
			IsFireBtDownP2 = false;
			InputEventCtrl.IsClickFireBtTwoDown = false;
			//ScreenLog.Log("game fireBtP2 up!");
			InputEventCtrl.GetInstance().ClickFireBtTwo( ButtonState.UP );
		}

		if( !IsDaoDanBtDownP2 && (buffer[20]&0x02) == 0x02 )
		{
			IsDaoDanBtDownP2 = true;
			InputEventCtrl.GetInstance().ClickDaoDanBtTwo( ButtonState.DOWN );
		}
		else if( IsDaoDanBtDownP2 && (buffer[20]&0x02) == 0x00 )
		{
			IsDaoDanBtDownP2 = false;
			InputEventCtrl.GetInstance().ClickDaoDanBtTwo( ButtonState.UP );
		}
		
		//setPanel selectBt
		if( !bSetEnterKeyDown && (buffer[19]&0x01) == 0x01 )
		{
			bSetEnterKeyDown = true;
			//ScreenLog.Log("game setEnterBt down!");
			InputEventCtrl.GetInstance().ClickSetEnterBt( ButtonState.DOWN );
		}
		else if ( bSetEnterKeyDown && (buffer[19]&0x01) == 0x00 )
		{
			bSetEnterKeyDown = false;
			//ScreenLog.Log("game setEnterBt up!");
			InputEventCtrl.GetInstance().ClickSetEnterBt( ButtonState.UP );
		}
		
		//setPanel moveBt
		if ( !bSetMoveKeyDown && (buffer[19]&0x02) == 0x02 )
		{
			bSetMoveKeyDown = true;
			//ScreenLog.Log("game setMoveBt down!");
			//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.DOWN );
			InputEventCtrl.GetInstance().ClickSetMoveBt( ButtonState.DOWN );
		}
		else if( bSetMoveKeyDown && (buffer[19]&0x02) == 0x00 )
		{
			bSetMoveKeyDown = false;
			//ScreenLog.Log("game setMoveBt up!");
			//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.UP );
			InputEventCtrl.GetInstance().ClickSetMoveBt( ButtonState.UP );
		}
	}
	
//	void ResetIsTouBiBtDown()
//	{
//		if(!bIsTouBiBtDown)
//		{
//			return;
//		}
//		bIsTouBiBtDown = false;
//	}
	
	static void SubTaBanCountInfo()
	{
		if(TanBanDownCount_P1 > 0)
		{
			TanBanDownCount_P1 -= SubTaBanCount;
			if(TanBanDownCount_P1 < 0)
			{
				TanBanDownCount_P1 = 0;
			}
		}
	}
	
	void closeDevice()
	{
		if (openPCVRFlag == 1)
		{
			openPCVRFlag = 2;
		}
	}
	
	void SubPcvrCoin(int subNum)
	{
		if (!bIsHardWare) {
			return;
		}
		IsSubPlayerCoin = true;
		subCoinNum = subNum;
	}
	
	public void SubPlayerCoin(int subNum, PlayerEnum playerIndex)
	{
		if (!bIsHardWare) {
			return;
		}

		switch (playerIndex) {
		case PlayerEnum.PlayerOne:
			if (CoinNumCurrentP1 < subNum) {
				return;
			}
			CoinNumCurrentP1 -= subNum;
			break;

		case PlayerEnum.PlayerTwo:
			if (CoinNumCurrentP2 < subNum) {
				return;
			}
			CoinNumCurrentP2 -= subNum;
			break;
		}
	}
	
	public static void InitHandleJsonInfo()
	{
		FileName = XKGlobalData.FileName;
		HandleJsonObj = XKGlobalData.HandleJsonObj;
	}
	
	public static void InitSteerInfo()
	{
		string strValMax = HandleJsonObj.ReadFromFileXml(FileName, "SteerValMax");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "1";
			HandleJsonObj.WriteToFileXml(FileName, "SteerValMax", strValMax);
		}
		SteerValMax = Convert.ToUInt32( strValMax );
		
		string strValMin = HandleJsonObj.ReadFromFileXml(FileName, "SteerValMin");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			HandleJsonObj.WriteToFileXml(FileName, "SteerValMin", strValMin);
		}
		SteerValMin = Convert.ToUInt32( strValMin );
		
		string strValCen = HandleJsonObj.ReadFromFileXml(FileName, "SteerValCen");
		if(strValCen == null || strValCen == "")
		{
			strValCen = "1";
			HandleJsonObj.WriteToFileXml(FileName, "SteerValCen", strValCen);
		}
		SteerValCen = Convert.ToUInt32( strValCen );
		
		SteerDisVal = (uint)Mathf.Abs((float)SteerValMax - SteerValMin);
		//Debug.Log("SteerDisVal " + SteerDisVal + ", SteerValMax " + SteerValMax + ", SteerValMin " + SteerValMin);
	}
	
	public static void SaveSteerVal(uint steerVal, PcvrValState key)
	{
		switch (key) {
		case PcvrValState.ValMin:
			SteerValMin = steerVal;
			HandleJsonObj.WriteToFileXml(FileName, "SteerValMin", steerVal.ToString());
			break;
			
		case PcvrValState.ValCenter:
			SteerValCen = steerVal;
			HandleJsonObj.WriteToFileXml(FileName, "SteerValCen", steerVal.ToString());
			break;
			
		case PcvrValState.ValMax:
			SteerValMax = steerVal;
			HandleJsonObj.WriteToFileXml(FileName, "SteerValMax", steerVal.ToString());
			break;
		}
		SteerDisVal = (uint)Mathf.Abs((float)SteerValMax - SteerValMin);
	}
	
	public static float GetPcvrSteerVal()
	{
		if (bIsHardWare && openPCVRFlag == 0) {
			mGetSteer = 0f;
			return mGetSteer;
		}
		
		if (SteerDisVal == 0) {
			mGetSteer = 0f;
			return mGetSteer;
		}
		
		float tmpVal = 0f;
		float steerCur = 0;
		if (bIsHardWare && !IsGetValByKey) {
			//steerCur = (float)SteerValCur;
			uint bikeDir = SteerValCur;
			uint bikeDirLen = SteerValMax - SteerValMin + 1;
			if(SteerValMax < SteerValMin)
			{
				if(bikeDir > SteerValCen)
				{
					bikeDirLen = SteerValMin - SteerValCen + 1;
				}
				else
				{
					bikeDirLen = SteerValCen - SteerValMax + 1;
				}
				
				//check bikeDir
				if(bikeDir < SteerValMax)
				{
					bikeDir = SteerValMax;
				}
				else if(bikeDir > SteerValMin)
				{
					bikeDir = SteerValMin;
				}
			}
			else
			{
				if(bikeDir > SteerValCen)
				{
					bikeDirLen = SteerValMax - SteerValCen + 1;
				}
				else
				{
					bikeDirLen = SteerValCen - SteerValMin + 1;
				}
				
				//check bikeDir
				if(bikeDir < SteerValMin)
				{
					bikeDir = SteerValMin;
				}
				else if(bikeDir > SteerValMax)
				{
					bikeDir = SteerValMax;
				}
			}
			////ScreenLog.Log("bikeDirLen = " + bikeDirLen);
			
			if(bikeDirLen == 0)
			{
				bikeDirLen = 1;
			}
			
			uint bikeDirCur = SteerValMax - bikeDir;
			float bikeDirPer = (float)bikeDirCur / bikeDirLen;
			if(SteerValMax > SteerValMin)
			{
				//ZhengJie FangXiangDianWeiQi
				if(bikeDir > SteerValCen)
				{
					bikeDirCur = bikeDir - SteerValCen;
					bikeDirPer = (float)bikeDirCur / bikeDirLen;
				}
				else
				{
					bikeDirCur = SteerValCen - bikeDir;
					bikeDirPer = - (float)bikeDirCur / bikeDirLen;
				}
			}
			else
			{
				//FanJie DianWeiQi
				if(bikeDir > SteerValCen)
				{
					bikeDirCur = bikeDir - SteerValCen;
					bikeDirPer = - (float)bikeDirCur / bikeDirLen;
				}
				else
				{
					bikeDirCur = SteerValCen - bikeDir;
					bikeDirPer = (float)bikeDirCur / bikeDirLen;
				}
			}
			mGetSteer = bikeDirPer;
		}
		else {
			steerCur = Input.GetAxis("Horizontal") + 1f;
			tmpVal = Mathf.Abs(steerCur - SteerValMin) / SteerDisVal;
			mGetSteer = (tmpVal - 0.5f) * 2f;
		}
		
		/*TestValStr = tmpVal.ToString() + " *** " + steerCur.ToString() + " * " + Input.GetAxis("Horizontal") + " ** " + mGetSteer;
		Debug.Log(TestValStr);*/
		return mGetSteer;
	}
	
	public static void InitTaBanInfo()
	{
		string strValMax = HandleJsonObj.ReadFromFileXml(FileName, "TaBanValMax");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "1";
			HandleJsonObj.WriteToFileXml(FileName, "TaBanValMax", strValMax);
		}
		TaBanValMax = Convert.ToUInt32( strValMax );
		
		string strValMin = HandleJsonObj.ReadFromFileXml(FileName, "TaBanValMin");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			HandleJsonObj.WriteToFileXml(FileName, "TaBanValMin", strValMin);
		}
		TaBanValMin = Convert.ToUInt32( strValMin );
		
		string strIsFanZhuangTaBan = HandleJsonObj.ReadFromFileXml(FileName, "IsFanZhuangTaBan");
		if(strIsFanZhuangTaBan == null || strIsFanZhuangTaBan == "")
		{
			strIsFanZhuangTaBan = "0";
			HandleJsonObj.WriteToFileXml(FileName, "IsFanZhuangTaBan", strIsFanZhuangTaBan);
		}
		IsFanZhuangTaBan = strIsFanZhuangTaBan == "0" ? false : true;
		TaBanDisVal = (uint)Mathf.Abs((float)TaBanValMax - TaBanValMin);
		//Debug.Log("TaBanDisVal " + TaBanDisVal + ", TaBanValMax " + TaBanValMax + ", TaBanValMin " + TaBanValMin);
	}
	
	public static void SaveTaBanVal(uint TaBanVal, PcvrValState key)
	{
		switch (key) {
		case PcvrValState.ValMin:
			TaBanValMin = TaBanVal;
			HandleJsonObj.WriteToFileXml(FileName, "TaBanValMin", TaBanVal.ToString());
			break;
			
		case PcvrValState.ValMax:
			if (!bIsHardWare) {
				SaveTaBanVal(1, PcvrValState.ValMin);
			}
			else {
				SaveTaBanVal(TanBanCenterNum, PcvrValState.ValMin);
				
				uint bikeTaBanNum = TaBanVal;
				if(bikeTaBanNum >= TanBanCenterNum)
				{
					IsFanZhuangTaBan = false;
					HandleJsonObj.WriteToFileXml(FileName, "IsFanZhuangTaBan", "0");
				}
				else if(bikeTaBanNum < TanBanCenterNum)
				{
					IsFanZhuangTaBan = true;
					HandleJsonObj.WriteToFileXml(FileName, "IsFanZhuangTaBan", "1");
				}
			}
			TaBanValMax = TaBanVal;
			HandleJsonObj.WriteToFileXml(FileName, "TaBanValMax", TaBanVal.ToString());
			TaBanDisVal = (uint)Mathf.Abs((float)TaBanValMax - TaBanValMin);
			break;
		}
	}
	
	public static bool IsGetValByKey = false;
	public static float GetPcvrTaBanVal()
	{
		if (bIsHardWare && openPCVRFlag == 0) {
			TanBanDownCount_P1 = 0f;
			return TanBanDownCount_P1;
		}
		
		if (TaBanDisVal == 0) {
			TanBanDownCount_P1 = 0f;
			return TanBanDownCount_P1;
		}
		
		float tmpVal = 0f;
		float taBanCur = 0;
		if (bIsHardWare && !IsGetValByKey) {
			uint bikeTaBanNum = TaBanValCur;
			//player click jiaoTaBanBt
			if(!IsFanZhuangTaBan)
			{
				if(bikeTaBanNum > TanBanCenterNum)
				{
					bPlayerHitTaBan_P1 = true;
					if(TaBanValMax < bikeTaBanNum)
					{
						TaBanValMax = bikeTaBanNum;
						HandleJsonObj.WriteToFileXml(FileName, "TaBanValMax", TaBanValMax.ToString());
					}
					TanBanDownCount_P1 = ((float)(bikeTaBanNum - TanBanCenterNum) / (TaBanValMax - TanBanCenterNum)) * PcvrTanBanValTmp;
				}
				else if(bikeTaBanNum < TanBanCenterNum)
				{
					SubTaBanCountInfo();
					bPlayerHitTaBan_P1 = false;
				}
				else
				{
					SubTaBanCountInfo();
				}
			}
			else
			{
				if(bikeTaBanNum < TanBanCenterNum)
				{
					bPlayerHitTaBan_P1 = true;
					if(TaBanValMax > bikeTaBanNum)
					{
						TaBanValMax = bikeTaBanNum;
						HandleJsonObj.WriteToFileXml(FileName, "TaBanValMax", TaBanValMax.ToString());
					}
					TanBanDownCount_P1 = ((float)(TanBanCenterNum - bikeTaBanNum) / (TanBanCenterNum - TaBanValMax)) * PcvrTanBanValTmp;
				}
				else if(bikeTaBanNum > TanBanCenterNum)
				{
					SubTaBanCountInfo();
					bPlayerHitTaBan_P1 = false;
				}
				else
				{
					SubTaBanCountInfo();
				}
			}
		}
		else {
			taBanCur = Input.GetAxis("Vertical") + 1f;
			if ( (TaBanValMin < TaBanValMax && taBanCur >= TaBanValMin)
			    || (TaBanValMin > TaBanValMax && taBanCur <= TaBanValMin) ) {
				//tmpVal = Mathf.Abs((float)taBanCur - TaBanValMin) / TaBanDisVal;
				tmpVal = Mathf.Abs(taBanCur - TaBanValMin) / TaBanDisVal;
				//TanBanDownCount_P1 = (tmpVal - 0.5f) * 2f;
				TanBanDownCount_P1 = tmpVal;
			}
		}
		return TanBanDownCount_P1;
	}
	
	void InitCrossPosInfoPOne()
	{
		string strValMax = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosXMaxP1");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP1", strValMax);
		}
		CrossPosXMaxP1 = Convert.ToUInt32( strValMax );
		
		string strValMin = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosXMinP1");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP1", strValMax);
		}
		CrossPosXMinP1 = Convert.ToUInt32( strValMin );
		
		strValMax = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosYMaxP1");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP1", strValMax);
		}
		CrossPosYMaxP1 = Convert.ToUInt32( strValMax );
		
		strValMin = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosYMinP1");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP1", strValMax);
		}
		CrossPosYMinP1 = Convert.ToUInt32( strValMin );
		
		CrossPosDisXP1 = (uint)Mathf.Abs((float)CrossPosXMaxP1 - CrossPosXMinP1) + 1;
		CrossPosDisYP1 = (uint)Mathf.Abs((float)CrossPosYMaxP1 - CrossPosYMinP1) + 1;
		/*Debug.Log("CrossPosDisXP1 " + CrossPosDisXP1 + ", CrossPosDisYP1 " + CrossPosDisYP1);
		Debug.Log("CrossPosXMaxP1 " + CrossPosXMaxP1 + ", CrossPosXMinP1 " + CrossPosXMinP1);
		Debug.Log("CrossPosYMaxP1 " + CrossPosYMaxP1 + ", CrossPosYMinP1 " + CrossPosYMinP1);*/
	}
	
	public static void SaveCrossPosInfoPOne(AdjustGunDrossState val)
	{
		Vector3 crossPos = Input.mousePosition;
		if (bIsHardWare) {
			crossPos = MousePositionP1;
		}
		
		if (crossPos.x  < 0f) {
			crossPos.x = 0f;
		}
		
		if (crossPos.y  < 0f) {
			crossPos.y = 0f;
		}
		
		uint pxCenterMin = 0;
		uint pyCenterMin = 0;
		uint pxCenterMax = 0;
		uint pyCenterMax = 0;
		
		uint px = (uint)crossPos.x;
		uint py = (uint)crossPos.y;
		//Debug.Log("px " + px + ", py " + py);
		
		switch (val) {
		case AdjustGunDrossState.GunCrossLU:
			CrossPxP1_1 = px;
			CrossPyP1_1 = py;
			break;
			
		case AdjustGunDrossState.GunCrossRU:
			CrossPxP1_2 = px;
			CrossPyP1_2 = py;
			break;
			
		case AdjustGunDrossState.GunCrossRD:
			CrossPxP1_3 = px;
			CrossPyP1_3 = py;
			break;
			
		case AdjustGunDrossState.GunCrossLD:
			CrossPxP1_4 = px;
			CrossPyP1_4 = py;
			
			pxCenterMin = (uint)(0.5f * (CrossPxP1_1 + CrossPxP1_4));
			pxCenterMax = (uint)(0.5f * (CrossPxP1_2 + CrossPxP1_3));
			pyCenterMin = (uint)(0.5f * (CrossPyP1_3 + CrossPyP1_4));
			pyCenterMax = (uint)(0.5f * (CrossPyP1_1 + CrossPyP1_2));
			
			if (pxCenterMin < pxCenterMax) {
				CrossPosXMinP1 = CrossPxP1_1 >= CrossPxP1_4 ? CrossPxP1_4 : CrossPxP1_1;
				px = CrossPosXMinP1;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP1", px.ToString());
				
				CrossPosXMaxP1 = CrossPxP1_2 >= CrossPxP1_3 ? CrossPxP1_2 : CrossPxP1_3;
				px = CrossPosXMaxP1;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP1", px.ToString());
			}
			else {
				CrossPosXMinP1 = CrossPxP1_1 <= CrossPxP1_4 ? CrossPxP1_4 : CrossPxP1_1;
				px = CrossPosXMinP1;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP1", px.ToString());
				
				CrossPosXMaxP1 = CrossPxP1_2 <= CrossPxP1_3 ? CrossPxP1_2 : CrossPxP1_3;
				px = CrossPosXMaxP1;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP1", px.ToString());
			}
			
			if (pyCenterMin < pyCenterMax) {
				CrossPosYMinP1 = CrossPyP1_3 >= CrossPyP1_4 ? CrossPyP1_4 : CrossPyP1_3;
				py = CrossPosYMinP1;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP1", py.ToString());
				
				CrossPosYMaxP1 = CrossPyP1_1 >= CrossPyP1_2 ? CrossPyP1_1 : CrossPyP1_2;
				py = CrossPosYMaxP1;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP1", py.ToString());
			}
			else {
				CrossPosYMinP1 = CrossPyP1_3 <= CrossPyP1_4 ? CrossPyP1_4 : CrossPyP1_3;
				py = CrossPosYMinP1;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP1", py.ToString());
				
				CrossPosYMaxP1 = CrossPyP1_1 <= CrossPyP1_2 ? CrossPyP1_1 : CrossPyP1_2;
				py = CrossPosYMaxP1;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP1", py.ToString());
			}
			break;
		}
		
		CrossPosDisXP1 = (uint)Mathf.Abs((float)CrossPosXMaxP1 - CrossPosXMinP1) + 1;
		CrossPosDisYP1 = (uint)Mathf.Abs((float)CrossPosYMaxP1 - CrossPosYMinP1) + 1;
		
		/*Debug.Log("CrossPosDisXP1 " + CrossPosDisXP1
		          + ", CrossPosDisYP1 " + CrossPosDisYP1
		          + ", CrossPosXMaxP1 " + CrossPosXMaxP1
		          + ", CrossPosXMinP1 " + CrossPosXMinP1
		          + ", CrossPosYMaxP1 " + CrossPosYMaxP1
		          + ", CrossPosYMinP1 " + CrossPosYMinP1);*/
	}
	
	public static void CheckCrossPositionPOne()
	{
		if (CrossPosDisXP1 <= 1 || CrossPosDisYP1 <= 1) {
			return;
		}
		
		Vector3 pos = Vector3.zero;
		Vector3 mousePosCur = Input.mousePosition;
		if (bIsHardWare) {
			mousePosCur = MousePositionP1;
		}
		
		if (CrossPosXMinP1 < CrossPosXMaxP1) {
			mousePosCur.x = mousePosCur.x < CrossPosXMinP1 ? CrossPosXMinP1 : mousePosCur.x;
			mousePosCur.x = mousePosCur.x < CrossPosXMaxP1 ? mousePosCur.x : CrossPosXMaxP1;
		}
		else {
			mousePosCur.x = mousePosCur.x > CrossPosXMinP1 ? CrossPosXMinP1 : mousePosCur.x;
			mousePosCur.x = mousePosCur.x > CrossPosXMaxP1 ? mousePosCur.x : CrossPosXMaxP1;
		}
		
		if (CrossPosYMinP1 < CrossPosYMaxP1) {
			mousePosCur.y = mousePosCur.y < CrossPosYMinP1 ? CrossPosYMinP1 : mousePosCur.y;
			mousePosCur.y = mousePosCur.y < CrossPosYMaxP1 ? mousePosCur.y : CrossPosYMaxP1;
		}
		else {
			mousePosCur.y = mousePosCur.y > CrossPosYMinP1 ? CrossPosYMinP1 : mousePosCur.y;
			mousePosCur.y = mousePosCur.y > CrossPosYMaxP1 ? mousePosCur.y : CrossPosYMaxP1;
		}
		
		pos.x = (int)(Mathf.Abs(mousePosCur.x - CrossPosXMinP1 + 1) * XkGameCtrl.ScreenWidth) / CrossPosDisXP1;
		pos.y = (int)(Mathf.Abs(mousePosCur.y - CrossPosYMinP1 + 1) * XkGameCtrl.ScreenHeight) / CrossPosDisYP1;
		
		CrossPositionOne = pos;
	}

	void InitCrossPosInfoPTwo()
	{
		string strValMax = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosXMaxP2");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP2", strValMax);
		}
		CrossPosXMaxP2 = Convert.ToUInt32( strValMax );
		
		string strValMin = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosXMinP2");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP2", strValMax);
		}
		CrossPosXMinP2 = Convert.ToUInt32( strValMin );
		
		strValMax = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosYMaxP2");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP2", strValMax);
		}
		CrossPosYMaxP2 = Convert.ToUInt32( strValMax );
		
		strValMin = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosYMinP2");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP2", strValMax);
		}
		CrossPosYMinP2 = Convert.ToUInt32( strValMin );
		
		CrossPosDisXP2 = (uint)Mathf.Abs((float)CrossPosXMaxP2 - CrossPosXMinP2) + 1;
		CrossPosDisYP2 = (uint)Mathf.Abs((float)CrossPosYMaxP2 - CrossPosYMinP2) + 1;
		/*Debug.Log("CrossPosDisXP2 " + CrossPosDisXP2 + ", CrossPosDisYP2 " + CrossPosDisYP2);
		Debug.Log("CrossPosXMaxP2 " + CrossPosXMaxP2 + ", CrossPosXMinP2 " + CrossPosXMinP2);
		Debug.Log("CrossPosYMaxP2 " + CrossPosYMaxP2 + ", CrossPosYMinP2 " + CrossPosYMinP2);*/
	}
	
	public static void SaveCrossPosInfoPTwo(AdjustGunDrossState val)
	{
		Vector3 crossPos = Input.mousePosition;
		if (bIsHardWare) {
			crossPos = MousePositionP2;
		}
		
		if (crossPos.x  < 0f) {
			crossPos.x = 0f;
		}
		
		if (crossPos.y  < 0f) {
			crossPos.y = 0f;
		}
		
		uint pxCenterMin = 0;
		uint pyCenterMin = 0;
		uint pxCenterMax = 0;
		uint pyCenterMax = 0;
		
		uint px = (uint)crossPos.x;
		uint py = (uint)crossPos.y;
		//Debug.Log("px " + px + ", py " + py);
		
		switch (val) {
		case AdjustGunDrossState.GunCrossLU:
			CrossPxP2_1 = px;
			CrossPyP2_1 = py;
			break;
			
		case AdjustGunDrossState.GunCrossRU:
			CrossPxP2_2 = px;
			CrossPyP2_2 = py;
			break;
			
		case AdjustGunDrossState.GunCrossRD:
			CrossPxP2_3 = px;
			CrossPyP2_3 = py;
			break;
			
		case AdjustGunDrossState.GunCrossLD:
			CrossPxP2_4 = px;
			CrossPyP2_4 = py;
			
			pxCenterMin = (uint)(0.5f * (CrossPxP2_1 + CrossPxP2_4));
			pxCenterMax = (uint)(0.5f * (CrossPxP2_2 + CrossPxP2_3));
			pyCenterMin = (uint)(0.5f * (CrossPyP2_3 + CrossPyP2_4));
			pyCenterMax = (uint)(0.5f * (CrossPyP2_1 + CrossPyP2_2));
			
			if (pxCenterMin < pxCenterMax) {
				CrossPosXMinP2 = CrossPxP2_1 >= CrossPxP2_4 ? CrossPxP2_4 : CrossPxP2_1;
				px = CrossPosXMinP2;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP2", px.ToString());
				
				CrossPosXMaxP2 = CrossPxP2_2 >= CrossPxP2_3 ? CrossPxP2_2 : CrossPxP2_3;
				px = CrossPosXMaxP2;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP2", px.ToString());
			}
			else {
				CrossPosXMinP2 = CrossPxP2_1 <= CrossPxP2_4 ? CrossPxP2_4 : CrossPxP2_1;
				px = CrossPosXMinP2;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP2", px.ToString());
				
				CrossPosXMaxP2 = CrossPxP2_2 <= CrossPxP2_3 ? CrossPxP2_2 : CrossPxP2_3;
				px = CrossPosXMaxP2;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP2", px.ToString());
			}
			
			if (pyCenterMin < pyCenterMax) {
				CrossPosYMinP2 = CrossPyP2_3 >= CrossPyP2_4 ? CrossPyP2_4 : CrossPyP2_3;
				py = CrossPosYMinP2;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP2", py.ToString());
				
				CrossPosYMaxP2 = CrossPyP2_1 >= CrossPyP2_2 ? CrossPyP2_1 : CrossPyP2_2;
				py = CrossPosYMaxP2;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP2", py.ToString());
			}
			else {
				CrossPosYMinP2 = CrossPyP2_3 <= CrossPyP2_4 ? CrossPyP2_4 : CrossPyP2_3;
				py = CrossPosYMinP2;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP2", py.ToString());
				
				CrossPosYMaxP2 = CrossPyP2_1 <= CrossPyP2_2 ? CrossPyP2_1 : CrossPyP2_2;
				py = CrossPosYMaxP2;
				HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP2", py.ToString());
			}
			break;
		}
		
		CrossPosDisXP2 = (uint)Mathf.Abs((float)CrossPosXMaxP2 - CrossPosXMinP2) + 1;
		CrossPosDisYP2 = (uint)Mathf.Abs((float)CrossPosYMaxP2 - CrossPosYMinP2) + 1;
		
		/*Debug.Log("CrossPosDisXP2 " + CrossPosDisXP2
		          + ", CrossPosDisYP2 " + CrossPosDisYP2
		          + ", CrossPosXMaxP2 " + CrossPosXMaxP2
		          + ", CrossPosXMinP2 " + CrossPosXMinP2
		          + ", CrossPosYMaxP2 " + CrossPosYMaxP2
		          + ", CrossPosYMinP2 " + CrossPosYMinP2);*/
	}
	
	public static void CheckCrossPositionPTwo()
	{
		if (CrossPosDisXP2 <= 1 || CrossPosDisYP2 <= 1) {
			return;
		}
		
		Vector3 pos = Vector3.zero;
		Vector3 mousePosCur = Input.mousePosition;
		if (bIsHardWare) {
			mousePosCur = MousePositionP2;
		}
		
		if (CrossPosXMinP2 < CrossPosXMaxP2) {
			mousePosCur.x = mousePosCur.x < CrossPosXMinP2 ? CrossPosXMinP2 : mousePosCur.x;
			mousePosCur.x = mousePosCur.x < CrossPosXMaxP2 ? mousePosCur.x : CrossPosXMaxP2;
		}
		else {
			mousePosCur.x = mousePosCur.x > CrossPosXMinP2 ? CrossPosXMinP2 : mousePosCur.x;
			mousePosCur.x = mousePosCur.x > CrossPosXMaxP2 ? mousePosCur.x : CrossPosXMaxP2;
		}
		
		if (CrossPosYMinP2 < CrossPosYMaxP2) {
			mousePosCur.y = mousePosCur.y < CrossPosYMinP2 ? CrossPosYMinP2 : mousePosCur.y;
			mousePosCur.y = mousePosCur.y < CrossPosYMaxP2 ? mousePosCur.y : CrossPosYMaxP2;
		}
		else {
			mousePosCur.y = mousePosCur.y > CrossPosYMinP2 ? CrossPosYMinP2 : mousePosCur.y;
			mousePosCur.y = mousePosCur.y > CrossPosYMaxP2 ? mousePosCur.y : CrossPosYMaxP2;
		}
		
		pos.x = (int)(Mathf.Abs(mousePosCur.x - CrossPosXMinP2 + 1) * XkGameCtrl.ScreenWidth) / CrossPosDisXP2;
		pos.y = (int)(Mathf.Abs(mousePosCur.y - CrossPosYMinP2 + 1) * XkGameCtrl.ScreenHeight) / CrossPosDisYP2;
		
		CrossPositionTwo = pos;
	}

	static void RandomJiaoYanDt()
	{	
		for (int i = 1; i < 4; i++) {
			JiaoYanDt[i] = (byte)UnityEngine.Random.Range(0x00, 0x7b);
		}
		JiaoYanDt[0] = 0x00;
		for (int i = 1; i < 4; i++) {
			JiaoYanDt[0] ^= JiaoYanDt[i];
		}
	}
	
	public void StartJiaoYanIO()
	{
		if (IsJiaoYanHid) {
			return;
		}
		
		if (!HardwareCheckCtrl.IsTestHardWare) {
			if (JiaoYanSucceedCount >= JiaoYanFailedMax) {
				return;
			}
			
			if (JiaoYanState == JIAOYANENUM.FAILED && JiaoYanFailedCount >= JiaoYanFailedMax) {
				return;
			}
		}
		else {
			HardwareCheckCtrl.Instance.SetJiaMiJYMsg("校验中...", JiaMiJiaoYanEnum.Null);
		}
		RandomJiaoYanDt();
		JiaoYanCheckCount = 0;
		IsJiaoYanHid = true;
		CancelInvoke("CloseJiaoYanIO");
		Invoke("CloseJiaoYanIO", 3f);
		//ScreenLog.Log("开始校验...");
	}
	
	void CloseJiaoYanIO()
	{
		if (!IsJiaoYanHid) {
			return;
		}
		IsJiaoYanHid = false;
		OnEndJiaoYanIO(JIAOYANENUM.FAILED);
	}
	
	void OnEndJiaoYanIO(JIAOYANENUM val)
	{
		IsJiaoYanHid = false;
		if (IsInvoking("CloseJiaoYanIO")) {
			CancelInvoke("CloseJiaoYanIO");
		}
		
		switch (val) {
		case JIAOYANENUM.FAILED:
			JiaoYanFailedCount++;
			if (HardwareCheckCtrl.IsTestHardWare) {
				HardwareCheckCtrl.Instance.JiaMiJiaoYanFailed();
			}
			break;
			
		case JIAOYANENUM.SUCCEED:
			JiaoYanSucceedCount++;
			JiaoYanFailedCount = 0;
			if (HardwareCheckCtrl.IsTestHardWare) {
				HardwareCheckCtrl.Instance.JiaMiJiaoYanSucceed();
			}
			break;
		}
		JiaoYanState = val;
		//Debug.Log("*****JiaoYanState "+JiaoYanState);
		
		if (JiaoYanFailedCount >= JiaoYanFailedMax || IsJiOuJiaoYanFailed) {
			//JiaoYanFailed
			if (IsJiOuJiaoYanFailed) {
				//JiOuJiaoYanFailed
				//Debug.Log("JOJYSB...");
			}
			else {
				//JiaMiXinPianJiaoYanFailed
				//Debug.Log("JMXPJYSB...");
				IsJiaMiJiaoYanFailed = true;
			}
		}
	}
	public static bool IsJiaMiJiaoYanFailed;
	
	enum JIAOYANENUM
	{
		NULL,
		SUCCEED,
		FAILED,
	}
	static int JiaoYanCheckCount;
	static JIAOYANENUM JiaoYanState = JIAOYANENUM.NULL;
	static byte JiaoYanFailedMax = 0x01;
	static byte JiaoYanSucceedCount;
	static byte JiaoYanFailedCount;
	static byte[] JiaoYanDt = new byte[4];
	static byte[] JiaoYanMiMa = new byte[4];
	static byte[] JiaoYanMiMaRand = new byte[4];
	byte JiOuJiaoYanCount;
	byte JiOuJiaoYanMax = 5;
	public static bool IsJiOuJiaoYanFailed;
	
	void InitJiaoYanMiMa()
	{
		JiaoYanMiMa[1] = 0x8e; //0x8e
		JiaoYanMiMa[2] = 0xc3; //0xc3
		JiaoYanMiMa[3] = 0xd7; //0xd7
		JiaoYanMiMa[0] = 0x00;
		for (int i = 1; i < 4; i++) {
			JiaoYanMiMa[0] ^= JiaoYanMiMa[i];
		}
	}
	
	void RandomJiaoYanMiMaVal()
	{
		for (int i = 0; i < 4; i++) {
			JiaoYanMiMaRand[i] = (byte)UnityEngine.Random.Range(0x00, (JiaoYanMiMa[i] - 1));
		}
		
		byte TmpVal = 0x00;
		for (int i = 1; i < 4; i++) {
			TmpVal ^= JiaoYanMiMaRand[i];
		}
		
		if (TmpVal == JiaoYanMiMaRand[0]) {
			JiaoYanMiMaRand[0] = JiaoYanMiMaRand[0] == 0x00 ?
				(byte)UnityEngine.Random.Range(0x01, 0xff) : (byte)(JiaoYanMiMaRand[0] + UnityEngine.Random.Range(0x01, 0xff));
		}
	}

	public static void SetGunZhenDongDengJi(int val, PlayerEnum playerVal)
	{
		switch (playerVal) {
		case PlayerEnum.PlayerOne:
			QiangZhenDongP1 = val;
			break;
		case PlayerEnum.PlayerTwo:
			QiangZhenDongP2 = val;
			break;
		}
	}

	public static void CloseGunZhenDongDengJi()
	{
		QiangZhenDongP1 = 0;
		QiangZhenDongP2 = 0;
	}
	
	public static bool IsPlayerActivePcvr = true;
	public static float TimeLastActivePcvr;
	void CheckIsPlayerActivePcvr()
	{
		if (!IsPlayerActivePcvr) {
			return;
		}
		
		if (Time.realtimeSinceStartup - TimeLastActivePcvr > 60f) {
			IsPlayerActivePcvr = false;
		}
	}
	
	public static void SetIsPlayerActivePcvr()
	{
		if (!bIsHardWare) {
			return;
		}
		IsPlayerActivePcvr = true;
		TimeLastActivePcvr = Time.realtimeSinceStartup;
	}
}

public enum PcvrValState
{
	ValMax,
	ValMin,
	ValCenter
}

public enum PcvrShuiBengState
{
	Level_1,
	Level_2
}

public enum LedState
{
	Liang,
	Shan,
	Mie
}

public enum AdjustGunDrossState
{
	GunCrossLU = 0,
	GunCrossRU,
	GunCrossRD,
	GunCrossLD,
	GunCrossOver
}