using UnityEngine;
using System.Collections;

public class XKPlayerDongGanCtrl : MonoBehaviour {
	public PlayerTypeEnum PlayerSt = PlayerTypeEnum.TanKe;
	/**
QiNangStateTK[0] -> 前气囊
QiNangStateTK[1] -> 后气囊
QiNangStateTK[2] -> 左气囊
QiNangStateTK[3] -> 右气囊
	 */
	public static int[] QiNangStateTK = {0, 0, 0, 0};
	/**
QiNangStateFJ[0] -> 前气囊
QiNangStateFJ[1] -> 后气囊
QiNangStateFJ[2] -> 左气囊
QiNangStateFJ[3] -> 右气囊
	 */
	public static int[] QiNangStateFJ = {0, 0, 0, 0};
	Vector3 EulerAngle;
	// Use this for initialization
	void Start()
	{
		QiNangStateTK = new int[]{0, 0, 0, 0};
		QiNangStateFJ = new int[]{0, 0, 0, 0};
	}
	
	// Update is called once per frame
	void Update()
	{
		if (PlayerSt == PlayerTypeEnum.TanKe) {
			if (DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi()
			    || (!XkGameCtrl.IsActivePlayerOne && !XkGameCtrl.IsActivePlayerTwo)) {
				pcvr.CloseQiNangQian();
				pcvr.CloseQiNangHou();
				return;
			}

			EulerAngle = transform.eulerAngles;
			if (EulerAngle.x > 180f) {
				EulerAngle.x -= 360f;
			}

			if (EulerAngle.z > 180f) {
				EulerAngle.z -= 360f;
			}
			float eulerAngleX = EulerAngle.x;
			float eulerAngleZ = EulerAngle.z;
			float offsetAngle = 2f;
			if (Mathf.Abs(eulerAngleX) <= offsetAngle) {
				//前后气囊放气.
				QiNangStateTK[0] = 0;
				QiNangStateTK[1] = 0;
				pcvr.CloseQiNangQian();
				pcvr.CloseQiNangHou();
			}
			else if  (eulerAngleX < 0f) {
				//前气囊充气,后气囊放气.
				QiNangStateTK[0] = 1;
				QiNangStateTK[1] = 0;
				pcvr.OpenQiNangQian();
				pcvr.CloseQiNangHou();
			}
			else if (eulerAngleX > 0f) {
				//后气囊充气,前气囊放气.
				QiNangStateTK[0] = 0;
				QiNangStateTK[1] = 1;
				pcvr.OpenQiNangHou();
				pcvr.CloseQiNangQian();
			}
			
			if (Mathf.Abs(eulerAngleZ) <= offsetAngle) {
				//左右气囊放气.
				QiNangStateTK[2] = 0;
				QiNangStateTK[3] = 0;
				pcvr.CloseQiNangZuo();
				pcvr.CloseQiNangYou();
			}
			else if  (eulerAngleZ > 0f) {
				//右气囊充气,左气囊放气.
				QiNangStateTK[2] = 0;
				QiNangStateTK[3] = 1;
				pcvr.OpenQiNangYou();
				pcvr.CloseQiNangZuo();
			}
			else if (eulerAngleZ < 0f) {
				//左气囊充气,右气囊放气.
				QiNangStateTK[2] = 1;
				QiNangStateTK[3] = 0;
				pcvr.OpenQiNangZuo();
				pcvr.CloseQiNangYou();
			}
		}
		else  if (PlayerSt == PlayerTypeEnum.FeiJi) {
			EulerAngle = transform.eulerAngles;
			if (EulerAngle.x > 180f) {
				EulerAngle.x -= 360f;
			}
			
			if (EulerAngle.z > 180f) {
				EulerAngle.z -= 360f;
			}
			float eulerAngleX = EulerAngle.x;
			float eulerAngleZ = EulerAngle.z;
			float offsetAngle = 1f;
			if (Mathf.Abs(eulerAngleX) <= offsetAngle) {
				//前后气囊放气.
				QiNangStateFJ[0] = 0;
				QiNangStateFJ[1] = 0;
			}
			else if  (eulerAngleX < 0f) {
				//前气囊充气,后气囊放气.
				QiNangStateFJ[0] = 1;
				QiNangStateFJ[1] = 0;
			}
			else if (eulerAngleX > 0f) {
				//后气囊充气,前气囊放气.
				QiNangStateFJ[0] = 0;
				QiNangStateFJ[1] = 1;
			}
			
			if (Mathf.Abs(eulerAngleZ) <= offsetAngle) {
				//左右气囊放气.
				QiNangStateFJ[2] = 0;
				QiNangStateFJ[3] = 0;
			}
			else if  (eulerAngleZ > 0f) {
				//右气囊充气,左气囊放气.
				QiNangStateFJ[2] = 0;
				QiNangStateFJ[3] = 1;
			}
			else if (eulerAngleZ < 0f) {
				//左气囊充气,右气囊放气.
				QiNangStateFJ[2] = 1;
				QiNangStateFJ[3] = 0;
			}
		}
	}
}