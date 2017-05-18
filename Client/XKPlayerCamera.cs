using UnityEngine;
using System.Collections;

public class XKPlayerCamera : MonoBehaviour {
	Transform CameraTran;
	Transform AimTran;
	Transform CameraParent;
	float SpeedIntoAim = 0.1f;
	float SpeedOutAim = 1f;
	bool IsOutAim;
//	float AimNpcSpeed = 0.1f;
//	float LeaveNpcSpeed = 0.1f;
	float GenZongTmpVal = 0.0001f;
	float GenZongCamRotVal = 0.2f;
	float GenZongCamPosVal = 0.005f;
	bool IsChangeSpeedOutAim;
	public static Transform FeiJiCameraTan;
	public static Transform TanKeCameraTan;
	GameObject CameraObj;
	XkPlayerCtrl PlayerScript;
	GameObject AimNpcObj;
	float TimeCheckAimNpcLast;
	Camera PlayerCamera;
	PlayerTypeEnum PlayerSt = PlayerTypeEnum.FeiJi;
	static XKPlayerCamera _InstanceFeiJi;
	public static XKPlayerCamera GetInstanceFeiJi()
	{
		return _InstanceFeiJi;
	}
	static XKPlayerCamera _InstanceTanKe;
	public static XKPlayerCamera GetInstanceTanKe()
	{
		return _InstanceTanKe;
	}
	static XKPlayerCamera _InstanceCartoon;
	public static XKPlayerCamera GetInstanceCartoon()
	{
		return _InstanceCartoon;
	}

	static Transform CameraVRTr;
	void Awake()
	{
		CameraVRTr = XkGameCtrl.GetInstance().CameraVRObj[0].transform;
		for (int i = 0; i < 2; i++) {
			if (PlayerGunCameraObj[i] != null) {
				PlayerGunCameraObj[i].SetActive(XkGameCtrl.GetInstance().IsOpenVR);
			}
		}
	}

	// Use this for initialization
	void Start()
	{	
		CameraObj = gameObject;
		PlayerCamera = GetComponent<Camera>();
		PlayerCamera.targetTexture = null;
		PlayerMainCameraTmp = new Camera[2];
		if (PlayerMainCamTmp[0] != null) {
			PlayerMainCameraTmp[0] = PlayerMainCamTmp[0].GetComponent<Camera>();
			if (XkGameCtrl.GetInstance().IsOpenVR) {
				PlayerMainCamTmp[0].SetActive(false);
			}
		}

		if (PlayerMainCamTmp[1] != null) {
			PlayerMainCameraTmp[1] = PlayerMainCamTmp[1].GetComponent<Camera>();
			if (XkGameCtrl.GetInstance().IsOpenVR) {
				PlayerMainCamTmp[1].SetActive(false);
			}
		}

		CameraTran = transform;
		XkPlayerCtrl script = GetComponentInParent<XkPlayerCtrl>();
		switch (script.PlayerSt) {
		case PlayerTypeEnum.FeiJi:
			_InstanceFeiJi = this;
			PlayerSt = PlayerTypeEnum.FeiJi;
			FeiJiCameraTan = transform;
			if (!XkGameCtrl.IsTiaoGuoStartCartoon) {
				CameraObj.SetActive(false);
			}
//			else {
//				CameraObj.SetActive(XkGameCtrl.GetInstance().IsOpenVR);
//			}
			break;

		case PlayerTypeEnum.TanKe:
			_InstanceTanKe = this;
			PlayerSt = PlayerTypeEnum.TanKe;
			TanKeCameraTan = transform;
			CameraObj.SetActive(false);
			break;

		case PlayerTypeEnum.CartoonCamera:
			_InstanceCartoon = this;
			PlayerSt = PlayerTypeEnum.CartoonCamera;
			if (XkGameCtrl.IsTiaoGuoStartCartoon) {
				CameraObj.SetActive(false);
			}
			break;
		}

		PlayerScript = GetComponentInParent<XkPlayerCtrl>();
		if (PlayerScript != null) {
			PlayerScript.SetPlayerCamera(this);
		}
		
		GameObject obj = new GameObject();
		obj.name = "CameraParent";
		CameraParent = obj.transform;
		CameraParent.parent = CameraTran.parent;
		CameraParent.localPosition = CameraTran.localPosition;
		CameraParent.rotation = CameraTran.rotation;
		CameraTran.parent = null;

		if (PlayerSt != PlayerTypeEnum.CartoonCamera) {
			if (!XkGameCtrl.IsTiaoGuoStartCartoon) {
				SetEnableCamera(false);
			}

			if (XkGameCtrl.IsActivePlayerOne) {
				IndexPlayerNum = -1;
				TestChangePlayerCamera();
			}
			else {
				IndexPlayerNum = 0;
				TestChangePlayerCamera();
			}
		}
		XKGlobalData.GetInstance().PlayGuanKaBeiJingAudio();
	}

//	public Transform DpnVrCamTr;
	public void SetActiveCamera(bool isActive)
	{
//		if (!XkGameCtrl.IsTiaoGuoStartCartoon) {
//			CameraObj.SetActive(isActive);
//		}

//		if (XkGameCtrl.GameModeVal == GameMode.LianJi && !XKCameraMapCtrl.GetInstance().GetActiveCameraMap()) {
//			GameJiTaiType jiTai = XkGameCtrl.GameJiTaiSt;
//			switch (jiTai) {
//			case GameJiTaiType.FeiJiJiTai:
//				if (PlayerSt == PlayerTypeEnum.TanKe) {
//					isActive = false;
//				}
//				break;
//
//			case GameJiTaiType.TanKeJiTai:
//				if (PlayerSt == PlayerTypeEnum.FeiJi) {
//					isActive = false;
//				}
//				break;
//			}
//		}

//		if (isActive && !ScreenDanHeiCtrl.IsStartGame && PlayerSt != PlayerTypeEnum.CartoonCamera) {
//			isActive = false;
//		}
//
//		if (XkGameCtrl.IsTiaoGuoStartCartoon) {
//			if (!XkGameCtrl.GetInstance().IsOpenVR) {
//				isActive = true;
//			}
//			else {
//				isActive = false;
//			}
//		}
//		Debug.Log("SetActiveCamera -> player "+PlayerSt+", isEnable "+isActive);
//		PlayerCamera.enabled = false;
//		if (PlayerMainCameraTmp[0] != null) {
//			PlayerMainCameraTmp[0].enabled = true;
//		}
	}

	public static int IndexPlayerNum = -1;
	public void SetActivePlayerGunCameraObj(int indexVal)
	{
		if (!XkGameCtrl.GetInstance().IsOpenVR) {
			return;
		}

		switch (indexVal) {
		case 0:
			PlayerGunCameraObj[0].SetActive(true);
			PlayerGunCameraObj[1].SetActive(false);
			break;
		case 1:
			PlayerGunCameraObj[0].SetActive(false);
			PlayerGunCameraObj[1].SetActive(true);
			break;
		}
	}

	void TestChangePlayerCamera()
	{
//		Debug.Log("TestChangePlayerCamera -> IndexPlayerNum "+IndexPlayerNum
//		          +", IsActivePlayerTwo "+XkGameCtrl.IsActivePlayerTwo);
		if (!XkGameCtrl.IsActivePlayerOne && IndexPlayerNum == 1) {
			return;
		}
		
		if (!XkGameCtrl.IsActivePlayerTwo && IndexPlayerNum == 0) {
			return;
		}

		if (IndexPlayerNum >= 1) {
			IndexPlayerNum = -1;
		}
		IndexPlayerNum++;

		SetActivePlayerGunCameraObj(IndexPlayerNum);
		switch (IndexPlayerNum) {
		case 0:
			if (!XkGameCtrl.GetInstance().IsOpenVR) {
				PlayerMainCameraTmp[0].enabled = true;
				PlayerMainCameraTmp[1].enabled = false;
			}
			else {
				PlayerMainCameraTmp[0].enabled = false;
				PlayerMainCameraTmp[1].enabled = false;
				
				CameraVRObj[0].SetActive(true);
				CameraVRTr.parent = CameraVRObj[0].transform;
				CameraVRTr.localPosition = Vector3.zero;
				CameraVRTr.localEulerAngles = Vector3.zero;
				CameraVRTr.localScale = Vector3.one;
				CameraVRObj[1].SetActive(false);
			}
			
			if (XKPlayerGunLaser.GetInstanceOne() != null) {
				XKPlayerGunLaser.GetInstanceOne().SetActivePlayerLaser(true);
			}
			
			if (XKPlayerGunLaser.GetInstanceTwo() != null) {
				XKPlayerGunLaser.GetInstanceTwo().SetActivePlayerLaser(false);
			}
			break;
		case 1:
			if (!XkGameCtrl.GetInstance().IsOpenVR) {
				PlayerMainCameraTmp[0].enabled = false;
				PlayerMainCameraTmp[1].enabled = true;
			}
			else {
				PlayerMainCameraTmp[0].enabled = false;
				PlayerMainCameraTmp[1].enabled = false;

				CameraVRObj[1].SetActive(true);
				CameraVRTr.parent = CameraVRObj[1].transform;
				CameraVRTr.localPosition = Vector3.zero;
				CameraVRTr.localEulerAngles = Vector3.zero;
				CameraVRTr.localScale = Vector3.one;
				CameraVRObj[0].SetActive(false);}
			
			if (XKPlayerGunLaser.GetInstanceOne() != null) {
				XKPlayerGunLaser.GetInstanceOne().SetActivePlayerLaser(false);
			}
			
			if (XKPlayerGunLaser.GetInstanceTwo() != null) {
				XKPlayerGunLaser.GetInstanceTwo().SetActivePlayerLaser(true);
			}
			break;
		}
	}

	public void SetEnableCamera(bool isEnable)
	{
		if (XkGameCtrl.GameModeVal == GameMode.LianJi && Network.peerType != NetworkPeerType.Disconnected) {
			GameJiTaiType jiTai = XkGameCtrl.GameJiTaiSt;
			switch (jiTai) {
			case GameJiTaiType.FeiJiJiTai:
				if (PlayerSt == PlayerTypeEnum.TanKe) {
					isEnable = false;
				}
				break;
				
			case GameJiTaiType.TanKeJiTai:
				if (PlayerSt == PlayerTypeEnum.FeiJi) {
					isEnable = false;
				}
				break;
			}
		}
		Debug.Log("SetEnableCamera -> player "+PlayerSt+", isEnable "+isEnable);
		PlayerCamera.enabled = isEnable;
	}

	public void ActivePlayerCamera()
	{
		bool isEnable = true;
		GameJiTaiType jiTai = XkGameCtrl.GameJiTaiSt;
		if (XkGameCtrl.GameModeVal == GameMode.LianJi && Network.peerType != NetworkPeerType.Disconnected) {
			switch (jiTai) {
			case GameJiTaiType.FeiJiJiTai:
				if (PlayerSt == PlayerTypeEnum.TanKe) {
					isEnable = false;
				}
				break;
				
			case GameJiTaiType.TanKeJiTai:
				if (PlayerSt == PlayerTypeEnum.FeiJi) {
					isEnable = false;
				}
				break;
			}
		}
		isEnable = false;
		Debug.Log("ActivePlayerCamera -> player "+PlayerSt+", isEnable "+isEnable+", jiTai "+jiTai);
		PlayerCamera.enabled = isEnable;
	}

	public bool GetActiveCamera()
	{
		return CameraObj.activeSelf;
	}
	
	/**
	 * CameraVRObj[0] -> cameraPlayer1.
	 * CameraVRObj[1] -> cameraPlayer2.
	 */
	public GameObject[] CameraVRObj;
	/**
	 * PlayerGunCameraObj[0] -> cameraPlayer1.
	 * PlayerGunCameraObj[1] -> cameraPlayer2.
	 */
	public GameObject[] PlayerGunCameraObj;
	/**
	 * PlayerMainCamTmp[0] -> cameraPlayer1.
	 * PlayerMainCamTmp[1] -> cameraPlayer2.
	 */
	public GameObject[] PlayerMainCamTmp;
	Camera[] PlayerMainCameraTmp;
	void Update()
	{
		if (!pcvr.bIsHardWare) {
			if (Input.GetKeyUp(KeyCode.C)) {
				TestChangePlayerCamera();
			}
		}

		SmothMoveCamera();
//		if (!XkGameCtrl.GetInstance().IsOpenVR) {
//			CheckMainCamera();
//		}
		//CheckStopCameraAimTranArray();
	}

	void FixedUpdate()
	{
//		if (Network.peerType != NetworkPeerType.Client) {
//			return;
//		}
		SmothMoveCamera();
	}

//	void LateUpdate()
//	{
//		SmothMoveCamera();
//	}

	void CheckMainCamera()
	{
		if (Camera.main == null || !Camera.main.enabled) {
			if (_InstanceFeiJi != null) {
				_InstanceFeiJi.ActivePlayerCamera();
			}
			else if (_InstanceTanKe != null) {
				_InstanceTanKe.ActivePlayerCamera();
			}
		}
	}

	public void SmothMoveCamera()
	{
		if (XKPlayerHeTiData.IsActiveHeTiPlayer) {
			if (PlayerSt == PlayerTypeEnum.FeiJi || PlayerSt == PlayerTypeEnum.TanKe) {
				this.enabled = false;
				return;
			}
		}

		if (CameraParent == null) {
			return;
		}

		if (PlayerScript.PlayerSt == PlayerTypeEnum.FeiJi
		    || PlayerScript.PlayerSt == PlayerTypeEnum.CartoonCamera) {
			if (Vector3.Distance(CameraTran.position, CameraParent.position) > 30f) {
				CameraTran.position = CameraParent.position;
				CameraTran.rotation = CameraParent.rotation;
			}
			else {
//				CameraTran.position = Vector3.Lerp(CameraTran.position, CameraParent.position, Time.deltaTime);
				CameraTran.position = Vector3.Lerp(CameraTran.position, CameraParent.position, GenZongCamPosVal);
			}
		}
		else {
			if (!CameraShake.IsCameraShake) {
				//CameraTran.position = CameraParent.position;
				if (Vector3.Distance(CameraTran.position, CameraParent.position) > 30f) {
					CameraTran.position = CameraParent.position;
					CameraTran.rotation = CameraParent.rotation;
				}
				else {
					CameraTran.position = Vector3.Lerp(CameraTran.position, CameraParent.position, Time.deltaTime);
				}
			}
		}
		SmothChangeCameraRot();
		
		if (PlayerScript.PlayerSt == PlayerTypeEnum.FeiJi) {
			if (ServerPortCameraCtrl.GetInstanceFJ() != null) {
				ServerPortCameraCtrl.GetInstanceFJ().CheckCameraFollowTran();
			}
		}
		else if (PlayerScript.PlayerSt == PlayerTypeEnum.TanKe) {
			if (ServerPortCameraCtrl.GetInstanceTK() != null) {
				ServerPortCameraCtrl.GetInstanceTK().CheckCameraFollowTran();
			}
		}
	}

	void SmothChangeCameraRot()
	{
		CheckAimNpcObj();
		if (AimTran == null) {
			if (IsOutAim) {
				float angle = Quaternion.Angle(CameraTran.rotation, CameraParent.rotation);
				//Debug.Log("angle ****** "+angle);
				if (angle <= 0.001f) {
					IsChangeSpeedOutAim = true;
				}

				if (IsChangeSpeedOutAim) {
					if (SpeedOutAim > GenZongCamRotVal) {
						SpeedOutAim -= GenZongTmpVal;
					}

					if (SpeedOutAim < GenZongCamRotVal) {
						SpeedOutAim += GenZongTmpVal;
					}

					if (Mathf.Abs(SpeedOutAim - GenZongCamRotVal) <= (GenZongTmpVal * 1.5f)) {
						SpeedOutAim = GenZongCamRotVal;
					}
					IsOutAim = false;
				}
				CameraTran.rotation = Quaternion.Lerp(CameraTran.rotation,
				                                      CameraParent.rotation,
				                                      SpeedOutAim * Time.deltaTime);
			}
			else {
				IsChangeSpeedOutAim = false;
				if (PlayerScript.PlayerSt == PlayerTypeEnum.FeiJi
				    || PlayerScript.PlayerSt == PlayerTypeEnum.CartoonCamera) {
					CameraTran.rotation = Quaternion.Lerp(CameraTran.rotation, CameraParent.rotation, GenZongCamRotVal * Time.deltaTime);
				}
				else {
					if (Quaternion.Angle(CameraTran.rotation, CameraParent.rotation) > 30f) {
						CameraTran.rotation = CameraParent.rotation;
					}
					else {
						CameraTran.rotation = Quaternion.Lerp(CameraTran.rotation,
						                                      CameraParent.rotation,
						                                      GenZongCamRotVal * Time.deltaTime);
					}
				}
			}
		}
		else {
			CheckAimTranObj();
		}
	}

	void CheckAimTranObj()
	{
		if (AimTran == null) {
			return;
		}

		Vector3 endPos = AimTran.position;
		Vector3 startPos = CameraTran.position;
		if (PlayerScript.PlayerSt == PlayerTypeEnum.TanKe) {
			endPos.y = startPos.y = 0f;
		}

		Vector3 forwardVal = endPos - startPos;
		if (forwardVal != Vector3.zero) {
			Quaternion rotTmp = Quaternion.LookRotation(forwardVal);
			CameraTran.rotation = Quaternion.Lerp(CameraTran.rotation, rotTmp, SpeedIntoAim * Time.deltaTime);
//			if (Quaternion.Angle(CameraTran.rotation, rotTmp) <= 5f
//			    && IsHandleCameraAimArray
//			    && CamAimArrayIndex < CameraAimArray.Length) {
//				AimTran = CameraAimArray[CamAimArrayIndex];
//				CamAimArrayIndex++;
//			}
		}
	}

	void ChangeAimTran(Transform aimVal)
	{
		return;
//		Debug.Log("ChangeAimTran...");
//		if (aimVal == null) {
//			if (AimTran != null) {
//				IsOutAim = true;
//			}
//			else {
//				IsOutAim = false;
//			}
//		}
//		else {
//			SpeedIntoAim = AimNpcSpeed;
//			SpeedOutAim = LeaveNpcSpeed;
//		}		
//		AimTran = aimVal;
	}

	public void SetAimTranInfo(AiMark markScript)
	{
		return;
		if (AimNpcObj != null) {
//			Debug.Log("SetAimTranInfo -> AimNpcObj should be null");
			return;
		}

		Transform aimVal = markScript.PlayerCamAimTran;
//		PlayerStopTimeVal = markScript.TimePlayerAni;
//		if (PlayerStopTimeVal > 0f && markScript.PlayerAni == ZhiShengJiAction.Null) {
//			IsHandleCameraAimArray = true;
//			CamAimArrayIndex = 0;
			//CameraAimArray = markScript.CameraAimArray;
			//Invoke("StopCameraAimTranArray", PlayerStopTimeVal);
//		}

		if (aimVal == null) {
			if (AimTran != null) {
				IsOutAim = true;
			}
			else {
				IsOutAim = false;
			}
		}
		else {
			SpeedIntoAim = markScript.SpeedIntoAim;
			SpeedOutAim = markScript.SpeedOutAim;
			//Debug.Log("111*************SpeedOutAim "+SpeedOutAim);
		}

		AimTran = aimVal;
	}

//	void CheckStopCameraAimTranArray()
//	{
//		if (!IsHandleCameraAimArray) {
//			return;
//		}
//
//		if (Time.realtimeSinceStartup - StartCameraAimArrayTime < PlayerStopTimeVal) {
//			return;
//		}
//		StopCameraAimTranArray();
//	}

//	float StartCameraAimArrayTime;
//	void StopCameraAimTranArray()
//	{
//		IsOutAim = true;
//		IsHandleCameraAimArray = false;
//		CamAimArrayIndex = 0;
//		AimTran = null;
//	}

	public Transform GetAimTram()
	{
		return AimTran;
	}

	void CheckAimNpcObj()
	{
		float dTime = Time.realtimeSinceStartup - TimeCheckAimNpcLast;
		if (dTime < 0.05f) {
			return;
		}
		TimeCheckAimNpcLast = Time.realtimeSinceStartup;
		
		if (PlayerScript == null) {
			return;
		}
		
		if (PlayerScript.GetAimNpcObj() == null) {
			if (AimNpcObj != null) {
				AimNpcObj = null;
				ChangeAimTran(null);
			}
			return;
		}
		
		if (PlayerScript.GetAimNpcObj() != AimNpcObj) {
			AimNpcObj = PlayerScript.GetAimNpcObj(); //改变距离主角最近的npc.
			ChangeAimTran(AimNpcObj.transform);
		}
	}

	public void SetCameraAimNpcSpeed(float aimSpeed, float leaveSpeed)
	{
		//AimNpcSpeed = aimSpeed;
//		LeaveNpcSpeed = leaveSpeed;
	}
	
	/**
	 * key == 1 -> 使主角摄像机依附于父级摄像机并且停止跟踪.
	 */
	public void SetPlayerCameraTran(int key)
	{
		switch(key) {
		case 1:
			CameraTran.parent = CameraParent;
			CameraTran.localPosition = Vector3.zero;
			CameraTran.localEulerAngles = Vector3.zero;
			CameraParent = null; //stop move player camera pos.
			break;
		
		default:
			CameraTran.position = CameraParent.position;
			CameraTran.rotation = CameraParent.rotation;
			break;
		}
	}
}