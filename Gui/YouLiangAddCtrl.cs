using UnityEngine;
using System.Collections;

public class YouLiangAddCtrl : MonoBehaviour {
	public GameObject YouLiangGuang;
	public UISprite YouLiangSprite;
	[Range(1f, 100f)] public float YouLiangDianAddYL = 40f;
	static YouLiangAddCtrl _Instance;
	public static YouLiangAddCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		YouLiangGuang.SetActive(false);
		SetYouLiangSpriteAmount(0f);
	}

	public void SetYouLiangSpriteAmount(float val)
	{
		YouLiangSprite.fillAmount = val;
	}

	public void AddPlayerYouLiangDian()
	{
		XkGameCtrl.GetInstance().AddPlayerYouLiang(YouLiangDianAddYL); //Add Player YouLiang
	}

	public void HiddenYouLiangAdd()
	{
		gameObject.SetActive(false);
	}

	public void ShowYouLiangGuangObj()
	{
		if (YouLiangGuang.activeSelf) {
			YouLiangGuang.SetActive(false);
		}
		YouLiangGuang.SetActive(true);
		if (IsInvoking("HiddenYouLiangGuangObj")) {
			CancelInvoke("HiddenYouLiangGuangObj");
		}
		Invoke("HiddenYouLiangGuangObj", 0.1f);
	}

	void HiddenYouLiangGuangObj()
	{
		YouLiangGuang.SetActive(false);
	}
}