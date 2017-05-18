using UnityEngine;
using System.Collections;

public class YouLiangCtrl : MonoBehaviour {
	public GameObject YouLiangFlashObj;
	public UISprite YouLiangJDSprite;
	public static bool IsActiveYouLiangFlash;
	public static bool IsChangeYouLiangFillAmout;
	float StartYouLiang;
	float EndYouLiang;
	static YouLiangCtrl Instance;
	public static YouLiangCtrl GetInstance()
	{
		return Instance;
	}

	// Use this for initialization
	void Start()
	{
		Instance = this;
		SetActiveYouLiangFlash(false);
	}
	
	void Update()
	{
		if (IsChangeYouLiangFillAmout) {
			ChangePlayerYouLiangFillAmout();
			return;
		}

		float valYL = XkGameCtrl.PlayerYouLiangCur / XkGameCtrl.PlayerYouLiangMax;
		if (valYL > 1f) {
			valYL = 1f;
		}
		else if (valYL < 0f) {
			valYL = 0f;
		}
		YouLiangJDSprite.fillAmount = valYL;
	}

	public void InitChangePlayerYouLiangFillAmout(float startVal, float endVal)
	{
		if (IsChangeYouLiangFillAmout) {
			return;
		}
		IsChangeYouLiangFillAmout = true;
		StartYouLiang = startVal;
		EndYouLiang = endVal;
	}

	void ResetChangePlayerYouLiangFillAmout()
	{
		if (!IsChangeYouLiangFillAmout) {
			return;
		}
		IsChangeYouLiangFillAmout = false;
	}

	void ChangePlayerYouLiangFillAmout()
	{
		if (!IsChangeYouLiangFillAmout) {
			return;
		}

		StartYouLiang += 2f;
		if (EndYouLiang < StartYouLiang) {
			StartYouLiang = EndYouLiang;
		}

		float valYL = StartYouLiang / XkGameCtrl.PlayerYouLiangMax;
		if (valYL > 1f) {
			valYL = 1f;
		}
		else if (valYL < 0f) {
			valYL = 0f;
		}
		YouLiangJDSprite.fillAmount = valYL;

		if (EndYouLiang <= StartYouLiang) {
			ResetChangePlayerYouLiangFillAmout();
		}
	}

	public void SetActiveYouLiangFlash(bool isActive)
	{
		IsActiveYouLiangFlash = isActive;
		YouLiangFlashObj.SetActive(isActive);
		if (!isActive) {
			XKGlobalData.GetInstance().StopAudioRanLiaoJingGao();
		}
	}

	public void HiddenYouLiang()
	{
		gameObject.SetActive(false);
		XKGlobalData.GetInstance().StopAudioRanLiaoJingGao();
	}
}