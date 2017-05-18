using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SetPanelCtrl : MonoBehaviour {
	static private SetPanelCtrl Instance = null;
	static public SetPanelCtrl GetInstance()
	{
		if(Instance == null)
		{
			GameObject obj = new GameObject("_SetPanelCtrl");
			Instance = obj.AddComponent<SetPanelCtrl>();
		}
		return Instance;
	}

	// Use this for initialization
	void Start()
	{
		//Debug.Log("SetPanelCtrl::init...");
		InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
	}

	void ClickSetEnterBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			return;
		}

		if (HardwareCheckCtrl.IsTestHardWare) {
			return;
		}

//		if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiServer) {
//			return;
//		}

		if (SceneManager.GetActiveScene().buildIndex != (int)GameLevel.Movie) {
			if (Network.peerType != NetworkPeerType.Disconnected) {
				return;
			}
		}

		if (SceneManager.GetActiveScene().buildIndex == (int)GameLevel.SetPanel) {
			return;
		}
		loadLevelSetPanel();
	}

	void loadLevelSetPanel()
	{
		if (XkGameCtrl.IsLoadingLevel) {
//			Debug.Log("*************Loading...");
			return;
		}

		if (!XkGameCtrl.IsGameOnQuit) {
			if (GameMovieCtrl.GetInstance() != null) {
				GameMovieCtrl.GetInstance().StopPlayMovie();
			}
			System.GC.Collect();
			SceneManager.LoadScene( (int)GameLevel.SetPanel );
		}
	}
}