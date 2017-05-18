using UnityEngine;
using System.Collections;

public class InputEventCtrl : MonoBehaviour {
	public static bool IsClickFireBtOneDown;
	public static bool IsClickFireBtTwoDown;
	public static bool IsClickDaoDanBtOneDown;
	public static bool IsClickDaoDanBtTwoDown;
	static private InputEventCtrl Instance = null;
	static public InputEventCtrl GetInstance()
	{
		if(Instance == null)
		{
			GameObject obj = new GameObject("_InputEventCtrl");
			Instance = obj.AddComponent<InputEventCtrl>();
			pcvr.GetInstance();
			XKGlobalData.GetInstance();
			SetPanelCtrl.GetInstance();
		}
		return Instance;
	}

	#region Click Button Envent
	public delegate void EventHandel(ButtonState val);
	public event EventHandel ClickStartBtOneEvent;
	public void ClickStartBtOne(ButtonState val)
	{
		if(ClickStartBtOneEvent != null)
		{
			ClickStartBtOneEvent( val );
			//pcvr.StartLightStateP1 = LedState.Mie;
			/*if (val == ButtonState.UP) {
				XKGlobalData.GetInstance().PlayStartBtAudio();
			}*/
		}
	}
	
	public event EventHandel ClickStartBtTwoEvent;
	public void ClickStartBtTwo(ButtonState val)
	{
		if(ClickStartBtTwoEvent != null)
		{
			ClickStartBtTwoEvent( val );
			//pcvr.StartLightStateP2 = LedState.Mie;
			/*if (val == ButtonState.UP) {
				XKGlobalData.GetInstance().PlayStartBtAudio();
			}*/
		}
	}

	public event EventHandel ClickSetEnterBtEvent;
	public void ClickSetEnterBt(ButtonState val)
	{
		if(ClickSetEnterBtEvent != null)
		{
			ClickSetEnterBtEvent( val );
		}

		if (val == ButtonState.DOWN) {
			XKGlobalData.PlayAudioSetEnter();
		}
	}
	
	public event EventHandel ClickSetMoveBtEvent;
	public void ClickSetMoveBt(ButtonState val)
	{
		if(ClickSetMoveBtEvent != null)
		{
			ClickSetMoveBtEvent( val );
		}

		if (val == ButtonState.DOWN) {
			XKGlobalData.PlayAudioSetMove();
		}
	}
	
	public event EventHandel ClickFireBtOneEvent;
	public void ClickFireBtOne(ButtonState val)
	{
		if(ClickFireBtOneEvent != null)
		{
			ClickFireBtOneEvent( val );
		}

		if(val == ButtonState.UP && SetPanelUiRoot.GetInstance() == null)
		{
			switch (GameTypeCtrl.PlayerPCState) {
			case PlayerEnum.PlayerOne:
				ClickStartBtOne( ButtonState.UP );
				break;
			case PlayerEnum.PlayerTwo:
				ClickStartBtTwo( ButtonState.UP );
				break;
			}
		}
	}

	public event EventHandel ClickFireBtTwoEvent;
	public void ClickFireBtTwo(ButtonState val)
	{
		if(ClickFireBtTwoEvent != null)
		{
			ClickFireBtTwoEvent( val );
		}
		
		if(val == ButtonState.UP && SetPanelUiRoot.GetInstance() == null)
		{
			switch (GameTypeCtrl.PlayerPCState) {
			case PlayerEnum.PlayerOne:
				ClickStartBtOne( ButtonState.UP );
				break;
			case PlayerEnum.PlayerTwo:
				ClickStartBtTwo( ButtonState.UP );
				break;
			}
		}
	}

	public event EventHandel ClickDaoDanBtOneEvent;
	public void ClickDaoDanBtOne(ButtonState val)
	{
		if(ClickDaoDanBtOneEvent != null)
		{
			ClickDaoDanBtOneEvent( val );
		}
	}
	
	public event EventHandel ClickDaoDanBtTwoEvent;
	public void ClickDaoDanBtTwo(ButtonState val)
	{
		if(ClickDaoDanBtTwoEvent != null)
		{
			ClickDaoDanBtTwoEvent( val );
		}
	}

	public event EventHandel ClickStopDongGanBtOneEvent;
	public void ClickStopDongGanBtOne(ButtonState val)
	{
		if(ClickStopDongGanBtOneEvent != null)
		{
			ClickStopDongGanBtOneEvent( val );
		}

		if (val == ButtonState.DOWN) {
			DongGanUICtrl.ShowDongGanInfo();
		}
	}
	#endregion

	void Update()
	{
		if (pcvr.bIsHardWare) {
			return;
		}

		if (Input.GetKeyUp(KeyCode.T) || Input.GetKeyUp(KeyCode.I)) {
			int coinVal = 0;
			switch (GameTypeCtrl.PlayerPCState) {
			case PlayerEnum.PlayerOne:
				coinVal = XKGlobalData.CoinPlayerOne + 1;
				XKGlobalData.SetCoinPlayerOne(coinVal);
				break;
			case PlayerEnum.PlayerTwo:
				coinVal = XKGlobalData.CoinPlayerTwo + 1;
				XKGlobalData.SetCoinPlayerTwo(coinVal);
				break;
			}
		}

		if(Input.GetKeyUp(KeyCode.G) || Input.GetKeyUp(KeyCode.K))
		{
			switch (GameTypeCtrl.PlayerPCState) {
			case PlayerEnum.PlayerOne:
				ClickStartBtOne( ButtonState.UP );
				break;
			case PlayerEnum.PlayerTwo:
				ClickStartBtTwo( ButtonState.UP );
				break;
			default:
				if (Input.GetKeyUp(KeyCode.G)) {
					ClickStartBtOne( ButtonState.UP );
				}
				if (Input.GetKeyUp(KeyCode.K)) {
					ClickStartBtTwo( ButtonState.UP );
				}
				break;
			}
		}
		
		if(Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.K))
		{
			switch (GameTypeCtrl.PlayerPCState) {
			case PlayerEnum.PlayerOne:
				ClickStartBtOne( ButtonState.DOWN );
				break;
			case PlayerEnum.PlayerTwo:
				ClickStartBtTwo( ButtonState.DOWN );
				break;
			default:
				if (Input.GetKeyDown(KeyCode.G)) {
					ClickStartBtOne( ButtonState.DOWN );
				}
				if (Input.GetKeyDown(KeyCode.K)) {
					ClickStartBtTwo( ButtonState.DOWN );
				}
				break;
			}
		}
		
		//setPanel enter button
		if(Input.GetKeyUp(KeyCode.F4))
		{
			ClickSetEnterBt( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.F4))
		{
			ClickSetEnterBt( ButtonState.DOWN );
		}
		
		//setPanel move button
		if(Input.GetKeyUp(KeyCode.F5))
		{
			ClickSetMoveBt( ButtonState.UP );
			//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.F5))
		{
			ClickSetMoveBt( ButtonState.DOWN );
			//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.DOWN );
		}

		//Fire button
		if(Input.GetKeyUp(KeyCode.Mouse0))
		{
			IsClickFireBtOneDown = false;
			ClickFireBtOne( ButtonState.UP );

			IsClickFireBtTwoDown = false;
			ClickFireBtTwo( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.Mouse0))
		{
			IsClickFireBtOneDown = true;
			ClickFireBtOne( ButtonState.DOWN );

			IsClickFireBtTwoDown = true;
			ClickFireBtTwo( ButtonState.DOWN );
		}

		if(Input.GetKeyUp(KeyCode.Mouse1))
		{
			IsClickDaoDanBtOneDown = false;
			ClickDaoDanBtOne( ButtonState.UP );

			IsClickDaoDanBtTwoDown = false;
			ClickDaoDanBtTwo( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.Mouse1))
		{
			IsClickDaoDanBtOneDown = true;
			ClickDaoDanBtOne( ButtonState.DOWN );

			IsClickDaoDanBtTwoDown = true;
			ClickDaoDanBtTwo( ButtonState.DOWN );
		}

		if(Input.GetKeyDown(KeyCode.C))
		{
			ClickStopDongGanBtOne(ButtonState.DOWN);
		}
	}
}

public enum ButtonState : int
{
	UP = 1,
	DOWN = -1
}