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
	Vector3 PlayerGunPos;
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
		//UpdatePlayerMainCamera();
		UpdatePlayerGunRot();
	}

	void UpdatePlayerMainCamera()
	{
		Vector3 forwardVal = PlayerMainCamTr.forward;
		forwardVal.y = 0f;
		if (forwardVal != Vector3.zero) {
			PlayerMainCamTr.forward = forwardVal;
		}
	}

	void UpdatePlayerGunRot()
	{
		if (Camera.main == null) {
			return;
		}

		Vector3 mousePosInput = Input.mousePosition;
		if (pcvr.bIsHardWare) {
			mousePosInput = pcvr.CrossPositionOne;
		}
		CurPX = mousePosInput.x;
		CurPY = mousePosInput.y;
		CurPX = CurPX < 0f ? 0f : CurPX;
		CurPX = CurPX > MaxPX ? MaxPX : CurPX;

		CurPY = CurPY < 0f ? 0f : CurPY;
		CurPY = CurPY > MaxPY ? MaxPY : CurPY;
		mousePosInput.x = CurPX;
		mousePosInput.y = CurPY;

		PlayerGunPos = PlayerGunTr.position;
		Vector3 mousePos = mousePosInput + Vector3.forward * OffsetForward;
		Vector3 posTmp = Camera.main.ScreenToWorldPoint(mousePos);
		Vector3 gunForward = Vector3.Normalize(posTmp - PlayerGunPos);
		if (gunForward != Vector3.zero) {
			PlayerGunTr.forward = gunForward;
		}
	}

	public void SetActivePlayerGun()
	{

	}
}