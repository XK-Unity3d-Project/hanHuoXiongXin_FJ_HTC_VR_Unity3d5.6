using UnityEngine;
using System.Collections;

public class GameModeCtrl : MonoBehaviour {
	public static GameMode ModeVal = GameMode.Null;
	public GameObject ModeObj;
	public GameObject StartBtObj;
	public GameObject LoadingObj;
	public GameObject WaitingObj;
	public static bool IsSelectDanJiMode;
	bool IsShowGameMode;
	bool IsLoadingGame;
	static GameModeCtrl _Instance;
	public static GameModeCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		IsActiveClientPort = false;
		if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiServer) {
			ModeVal = GameMode.LianJi;
		}
		else {
			ModeVal = GameMode.Null;
		}

		IsSelectDanJiMode = false;
		SetActiveLoading(false);
		HiddenGameMode();
		SetActiveStartBt(false);
		SetActiveWaitingObj(false);
		InputEventCtrl.GetInstance().ClickFireBtOneEvent += ClickFireBtOneEvent;
		InputEventCtrl.GetInstance().ClickFireBtTwoEvent += ClickFireBtTwoEvent;
	}

	void SetActiveWaitingObj(bool isActive)
	{
		if (WaitingObj.activeSelf == isActive) {
			return;
		}
		WaitingObj.SetActive(isActive);
	}

	public void ShowGameMode()
	{
		if (IsShowGameMode) {
			return;
		}
		IsShowGameMode = true;
		Debug.Log("show game mode selectPanel...");
		XKGlobalData.GetInstance().PlayModeBeiJingAudio();
//		InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickFireBtOneEvent;
//		InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickFireBtTwoEvent;
//		Invoke("DelayShowGameMode", 0.3f);

		CheckGameBtEvent();
	}
	
	void DelayShowGameMode()
	{
		ModeObj.SetActive(true);
	}

	void HiddenGameMode()
	{
		if (!ModeObj.activeSelf) {
			return;
		}
		ModeObj.SetActive(false);
	}

	public void SetActiveStartBt(bool IsActive)
	{
		if (StartBtObj.activeSelf == IsActive) {
			return;
		}

		if (IsActive && ModeVal == GameMode.Null) {
			return;
		}

		if (!IsActive) {
			if (ModeVal == GameMode.LianJi) {
				SetActiveWaitingObj(true);
			}
		}
		else {
			SetActiveWaitingObj(false);
		}
		StartBtObj.SetActive(IsActive);
	}
	
	public void ServerPortHiddenGameMode()
	{
//		if (!ModeObj.activeSelf) {
//			return;
//		}
		HiddenGameMode();
		LoadingObj.SetActive(true);
		ZhunXingCtrl.GetInstanceOne().SetActiveZhunXingObj(false);
		ZhunXingCtrl.GetInstanceTwo().SetActiveZhunXingObj(false);
	}

	public void SetActiveLoading(bool isActive)
	{
		if (LoadingObj.activeSelf == isActive) {
			return;
		}
		LoadingObj.SetActive(isActive);

		if (isActive) {
			//DanKengCtrl.GetInstance().ShowDanKengObj();
			ZhunXingCtrl.GetInstanceOne().SetActiveZhunXingObj(false);
			ZhunXingCtrl.GetInstanceTwo().SetActiveZhunXingObj(false);
			if (!IsLoadingGame) {
				IsLoadingGame = true;
				Invoke("DelayLoadingGame", 5f);
			}
		}
	}

	void DelayLoadingGame()
	{
		XkGameCtrl.LoadingGameScene_1();
	}

	public void ServerCallClientLoadingGame()
	{
		if (!ModeObj.activeSelf) {
			return;
		}
		HiddenGameMode();
		SetActiveLoading(true);
	}
	
	void ClickFireBtOneEvent(ButtonState state)
	{
		if (!XkGameCtrl.IsActivePlayerOne) {
			return;
		}
		CheckGameBtEvent();
	}

	void ClickFireBtTwoEvent(ButtonState state)
	{
		if (!XkGameCtrl.IsActivePlayerTwo) {
			return;
		}
		CheckGameBtEvent();
	}

	void CheckGameBtEvent()
	{
//		if (!ModeObj.activeSelf) {
//			return;
//		}

		ModeVal = GameMode.LianJi;
//		if (ModeVal == GameMode.Null) {
//			return;
//		}
		
//		if (!StartBtObj.activeSelf) {
//			return;
//		}

		XKGlobalData.GetInstance().PlayModeQueRenAudio();
		if (ModeVal != GameMode.LianJi) {
			IsSelectDanJiMode = true;
			HiddenGameMode();
			if (NetCtrl.GetInstance() != null) {
				NetCtrl.GetInstance().SendSubLinkCount();
			}
			GameTypeCtrl.SetAppTypeVal(AppGameType.LianJiFeiJi);
			NetworkServerNet.GetInstance().MakeServerDisconnect(); //Close ServerNet.
			NetworkServerNet.GetInstance().MakeClientDisconnect(); //Close ClientNet.
			SetActiveLoading(true);
		}
		else {
			IsSelectDanJiMode = false;
//			if (NetworkServerNet.GetInstance() != null) {
//				NetworkServerNet.GetInstance().TryToLinkServer();
//			}

			if (NetCtrl.GetInstance() != null) {
				NetCtrl.GetInstance().SendAddLinkCount(); //ServerPort.
			}
			else {
				//ClientPort.
				IsActiveClientPort = true;
				HiddenGameMode();
				GameTypeCtrl.SetAppTypeVal(AppGameType.LianJiFeiJi);
				SetActiveLoading(true);
			}
		}
		SetActiveStartBt(false);
	}
	public static bool IsActiveClientPort = false;
}