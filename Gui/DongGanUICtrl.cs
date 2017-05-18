using UnityEngine;
using System.Collections;

public class DongGanUICtrl : MonoBehaviour {
	public Texture[] DongGanUI;
	public Texture[] DongGanUI_Ch;
	public Texture[] DongGanUI_En;
	UITexture DongGanTexture;
	public static int DongGanCount;
	public static DongGanUICtrl Instance;
	// Use this for initialization
	void Start()
	{
		Instance = this;
		GameTextType gameTextVal = XKGlobalData.GetGameTextMode();
		//gameTextVal = GameTextType.English; //test.
		switch (gameTextVal) {
		case GameTextType.Chinese:
			DongGanUI = DongGanUI_Ch;
			break;
			
		case GameTextType.English:
			DongGanUI = DongGanUI_En;
			break;
		}

		DongGanCount = 1;
		DongGanTexture = GetComponent<UITexture>();
		DongGanTexture.mainTexture = DongGanUI[0];
		gameObject.SetActive(false);
	}

	public static void ShowDongGanInfo()
	{
		if (!ScreenDanHeiCtrl.IsStartGame) {
			return;
		}

		if (Instance == null) {
			return;
		}

		DongGanCount++;
		if (DongGanCount > 1) {
			DongGanCount = 0;
		}
		Instance.ShowDongGanUI(DongGanCount);
	}

	public void ShowDongGanUI(int index)
	{
		DongGanTexture.mainTexture = DongGanUI[index];
		gameObject.SetActive(true);

		if (index == 1) {
			Invoke("HiddenDongGanUI", 3f);
		}
	}

	void HiddenDongGanUI()
	{
		gameObject.SetActive(false);
	}
}