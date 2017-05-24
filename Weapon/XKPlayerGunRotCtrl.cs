using UnityEngine;
using System.Collections;

public class XKPlayerGunRotCtrl : MonoBehaviour
{
	public PlayerEnum PlayerSt = PlayerEnum.PlayerOne;
	public Transform PlayerGunTr;
	public Transform PlayerMainCamTr;
	float MaxPX = 0f;
	float MaxPY = 0f;
	float CurPX;
	float CurPY;
	float OffsetForward = 30f;
	static XKPlayerGunRotCtrl _InstanceOne;
	public static XKPlayerGunRotCtrl GetInstanceOne()
	{
		return _InstanceOne;
	}
	static XKPlayerGunRotCtrl _InstanceTwo;
	public static XKPlayerGunRotCtrl GetInstanceTwo()
	{
		return _InstanceTwo;
	}
	// Use this for initialization
	void Start()
	{
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			_InstanceOne = this;
			break;

		case PlayerEnum.PlayerTwo:
			_InstanceTwo = this;
			break;
		}
		MaxPX = Screen.width;
		MaxPY = Screen.height;
	}

	// Update is called once per frame
	void Update()
	{
		UpdatePlayerGunRot();
	}

	void UpdatePlayerGunRot()
	{
		if (Camera.main == null || PlayerGunTr == null) {
			return;
		}

		Vector3 mousePosInput = Input.mousePosition;
		if (pcvr.bIsHardWare) {
			switch (PlayerSt) {
			case PlayerEnum.PlayerOne:
				mousePosInput = pcvr.CrossPositionOne;
				break;
			case PlayerEnum.PlayerTwo:
				mousePosInput = pcvr.CrossPositionTwo;
				break;
			}
		}
		
		byte offsetVal = 1;
		float pX = mousePosInput.x;
		float pY = mousePosInput.y;
		pX = pX < 0f ? 0f : pX;
		pX = pX > MaxPX ? MaxPX : pX;
		pY = pY < 0f ? 0f : pY;
		pY = pY > MaxPY ? MaxPY : pY;
		float dpX = Mathf.Abs(pX - CurPX);
		float dpY = Mathf.Abs(pY - CurPY);
		if (dpX <= offsetVal && dpY <= offsetVal) {
			return;
		}
		CurPX = pX;
		CurPY = pY;

		mousePosInput.x = pX;
		mousePosInput.y = pY;

		Vector3 mousePos = mousePosInput + Vector3.forward * OffsetForward;
		Vector3 posTmp = Camera.main.ScreenToWorldPoint(mousePos);
		Vector3 gunForward = Vector3.Normalize(posTmp - PlayerGunTr.position);
		if (gunForward != Vector3.zero) {
			offsetVal += 1;
			if (dpX > offsetVal || dpY > offsetVal) {
				PlayerGunTr.forward = gunForward;
			}
			else {
				PlayerGunTr.forward = Vector3.Lerp(PlayerGunTr.forward, gunForward, Time.deltaTime * 5);
			}
			//Debug.Log("dpX "+dpX.ToString("f2")+", dpY "+dpY.ToString("f2"));
		}
	}
}