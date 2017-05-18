using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XKNpcHealthCtrl : MonoBehaviour {
	public NpcJiFenEnum NpcJiFen = NpcJiFenEnum.ShiBing; //控制主角所击杀npc的积分逻辑.
	[Range(0, 500)] public int YouLiangDian = 1;
	[Range(1, 1000)] public int MaxPuTongAmmo = 1; //单人模式下.
	[Range(1, 1000)] public int MaxPuTongAmmoSR = 1; //双人模式下.
	[Range(1, 1000)] public int LJMaxPuTongAmmo = 1; //联机单人模式下.
	[Range(1, 1000)] public int LJMaxPuTongAmmoSR = 1; //联机双人模式下.
	[Range(1, 1000)] public int MaxGaoBaoAmmo = 1;
	[Range(1, 1000)] public int MaxDaoDanAmmo = 1;
	[Range(0, 100)] public int MaxAmmoHurtLiZi = 0;
	public GameObject[] HiddenNpcObjArray; //npc死亡时需要立刻隐藏的对象.
	public GameObject HurtLiZiObj; //飞机npc的受伤粒子.
	[Range(0.1f, 100f)] public float YouTongDamageDis = 10f;
	int PuTongAmmoCount;
	int GaoBaoAmmoCount;
	int DaoDanAmmoCount;
	int AmmoHurtCount;
	bool IsDeathNpc;
	XKNpcMoveCtrl NpcScript;
	XKCannonCtrl CannonScript;
	public bool IsYouTongNpc;
	void Start()
	{
		NpcShouJiFanKui = GetComponent<XKNpcDamageCtrl>();
		DaPaoScript = GetComponentInParent<XKDaPaoCtrl>();
		int max = HiddenNpcObjArray.Length;
		for (int i = 0; i < max; i++) {
			if (HiddenNpcObjArray[i] == null) {
				Debug.LogWarning("HiddenNpcObjArray was wrong! index "+i);
				GameObject obj = null;
				obj.name = "null";
				break;
			}
		}

		if (MaxAmmoHurtLiZi > 0) {
			if (HurtLiZiObj == null) {
				Debug.LogError("HurtLiZiObj is null");
				HurtLiZiObj.name = "null";
			}
		}
		NpcScript = GetComponentInParent<XKNpcMoveCtrl>();
	}

	void OnCollisionEnter(Collision collision)
	{
		PlayerZhiShengJiCtrl playerScript = collision.gameObject.GetComponent<PlayerZhiShengJiCtrl>();
		if (playerScript == null) {
			return;
		}

		if (IsDeathNpc) {
			return;
		}
//		Debug.Log("**********OnCollisionEnter-> collision "+collision.gameObject.name);

		if (playerScript.GetPlayerType() != PlayerTypeEnum.TanKe) {
			return;
		}

		if (XkGameCtrl.GameJiTaiSt != GameJiTaiType.TanKeJiTai) {
			return;
		}
		
		if (!IsYouTongNpc) {
			XkGameCtrl.GetInstance().AddPlayerKillNpc(PlayerEnum.Null, NpcJiFen);
			XkGameCtrl.GetInstance().AddYouLiangDian(YouLiangDian, PlayerEnum.Null);
			//YouLiangDianUICtrl.GetInstance().HandleNpcYouLiangDian(YouLiangDian, transform);
		}

		if (NpcScript != null) {
			IsDeathNpc = true;
			NpcScript.TriggerRemovePointNpc(1);
		}
		else if (CannonScript != null) {
			IsDeathNpc = true;
			CannonScript.OnRemoveCannon(PlayerEnum.Null, 1);
		}
		CheckHiddenNpcObjArray();
	}

	public void CheckHiddenNpcObjArray()
	{
		if (HiddenNpcObjArray.Length <= 0) {
			return;
		}

		int max = HiddenNpcObjArray.Length;
		for (int i = 0; i < max; i++) {
			if (HiddenNpcObjArray[i].activeSelf) {
				XKNpcAnimatorCtrl aniScript = HiddenNpcObjArray[i].GetComponent<XKNpcAnimatorCtrl>();
				if (aniScript != null) {
					aniScript.ResetNpcAnimation(1);
				}
				HiddenNpcObjArray[i].SetActive(false);
			}
		}
	}

	public XKNpcMoveCtrl GetNpcMoveScript()
	{
		return NpcScript;
	}

	public void SetNpcMoveScript(XKNpcMoveCtrl scriptVal)
	{
		NpcScript = scriptVal;
		ResetNpcHealthInfo();
	}

	public bool GetIsDeathNpc()
	{
		return IsDeathNpc;
	}

	XKNpcDamageCtrl NpcShouJiFanKui;
	public void OnDamageNpc(PlayerAmmoType state, PlayerEnum playerSt)
	{
		if (IsDeathNpc) {
			return;
		}

		if (NpcScript == null || (NpcScript != null && !NpcScript.GetIsWuDi())) {
			switch (state) {
			case PlayerAmmoType.PuTongAmmo:
				PuTongAmmoCount++;
				break;
				
			case PlayerAmmoType.GaoBaoAmmo:
				GaoBaoAmmoCount++;
//				Debug.Log(gameObject.name+":: GaoBaoAmmoCount "+GaoBaoAmmoCount+", MaxGaoBaoAmmo "+MaxGaoBaoAmmo
//				          +", playerSt "+playerSt);
				break;
				
			case PlayerAmmoType.DaoDanAmmo:
				DaoDanAmmoCount++;
//				Debug.Log(gameObject.name+":: DaoDanAmmoCount "+MaxDaoDanAmmo+", MaxGaoBaoAmmo "+MaxDaoDanAmmo
//				          +", playerSt "+playerSt);
				break;
			}
		}

		if (NpcShouJiFanKui != null) {
			NpcShouJiFanKui.PlayNpcDamageEvent();
		}

		AmmoHurtCount++;
		if (MaxAmmoHurtLiZi > 0 && AmmoHurtCount >= MaxAmmoHurtLiZi) {
			if (HurtLiZiObj != null && !HurtLiZiObj.activeSelf) {
				HurtLiZiObj.SetActive(true);
			}
		}

		int puTongAmmoNum = MaxPuTongAmmo; //单机单人.
		if (XkGameCtrl.GameModeVal == GameMode.LianJi
		    && XkPlayerCtrl.PlayerTranFeiJi != null
		    && XkPlayerCtrl.PlayerTranTanKe != null) {
			if (XkGameCtrl.IsActivePlayerOne && XkGameCtrl.IsActivePlayerTwo) {
				puTongAmmoNum = LJMaxPuTongAmmoSR; //联机双人.
			}
			else {
				puTongAmmoNum = LJMaxPuTongAmmoSR; //联机单人.
			}
		}
		else {
			if (XkGameCtrl.IsActivePlayerOne && XkGameCtrl.IsActivePlayerTwo) {
				puTongAmmoNum = MaxPuTongAmmoSR; //单机双人.
			}
		}

		if (PuTongAmmoCount >= puTongAmmoNum
		    || GaoBaoAmmoCount >= MaxGaoBaoAmmo
		    || DaoDanAmmoCount >= MaxDaoDanAmmo ) {
			if (IsDeathNpc) {
				return;
			}
			IsDeathNpc = true;
			CheckHiddenNpcObjArray();

			bool isAddKillNpcNum = true;
			if (NpcScript != null && CannonScript != null) {
				if (NpcScript.GetIsDeathNPC()) {
					isAddKillNpcNum = false;
				}
			}
			
			if (!IsYouTongNpc) {
				if (Network.peerType != NetworkPeerType.Server) {
					if (isAddKillNpcNum) {
						XkGameCtrl.GetInstance().AddPlayerKillNpc(playerSt, NpcJiFen);
					}
					XkGameCtrl.GetInstance().AddYouLiangDian(YouLiangDian, playerSt);

					if (YouLiangDianUICtrl.GetInstance() != null) {
						YouLiangDianUICtrl.GetInstance().HandleNpcYouLiangDian(YouLiangDian, transform);
					}
				}
			}
			else {
				if (playerSt != PlayerEnum.Null) {
					CheckYouTongDamageNpc();
				}
			}

			if (NpcScript != null) {
				if (CannonScript != null) {
					CannonScript.OnRemoveCannon(playerSt, 1);
				}
				NpcScript.TriggerRemovePointNpc(1, CannonScript);
			}
			else if (CannonScript != null) {
				CannonScript.OnRemoveCannon(playerSt, 1);
			}
		}
	}

	[Range(0f, 100f)]public float YouTongDamageYTDis = 1f;
	void CheckYouTongDamageNpc()
	{
		if (!IsYouTongNpc) {
			return;
		}

		XKNpcHealthCtrl healthScript = null;
		Transform[] npcArray = XkGameCtrl.GetInstance().GetNpcTranList().ToArray();
		int max = npcArray.Length;
		Vector3 posA = transform.position;
		Vector3 posB = Vector3.zero;
		for (int i = 0; i < max; i++) {
			if (npcArray[i] == null) {
				continue;
			}
			
			posB = npcArray[i].position;
			if (Vector3.Distance(posA, posB) <= YouTongDamageDis) {
				healthScript = npcArray[i].GetComponentInChildren<XKNpcHealthCtrl>();
				if (healthScript != null) {
					//Add Damage Npc num to PlayerInfo.
					if (!healthScript.IsYouTongNpc) {
						healthScript.OnDamageNpc(PlayerAmmoType.DaoDanAmmo, PlayerEnum.Null);
					}
				}
			}

			
			if (Vector3.Distance(posA, posB) <= YouTongDamageYTDis) {
				healthScript = npcArray[i].GetComponentInChildren<XKNpcHealthCtrl>();
				if (healthScript != null) {
					//Add Damage Npc num to PlayerInfo.
					if (healthScript.IsYouTongNpc) {
						
						if (DaPaoScript != null) {
							DaPaoScript.AddYouTongScript(healthScript);
						}
					}
				}
			}
		}
		
		if (DaPaoScript != null) {
			DaPaoScript.CallListYouTongDamage(TimeYouTongDamage);
		}
	}

	XKDaPaoCtrl DaPaoScript;
	[Range(0f, 5f)]public float TimeYouTongDamage = 0.2f;

	public void SetCannonScript(XKCannonCtrl script)
	{
		CannonScript = script;
		ResetNpcHealthInfo();
	}

	void ResetNpcHealthInfo()
	{
		XkGameCtrl.GetInstance().AddNpcTranToList(transform);
		if (DaPaoScript != null) {
			DaPaoScript.ClearYouTongScriptList();
		}
		IsDeathNpc = false;
		PuTongAmmoCount = 0;
		GaoBaoAmmoCount = 0;
		DaoDanAmmoCount = 0;
		AmmoHurtCount = 0;

		int max = HiddenNpcObjArray.Length;
		for (int i = 0; i < max; i++) {
			if (!HiddenNpcObjArray[i].activeSelf) {
				HiddenNpcObjArray[i].SetActive(true);
			}
		}
	}
}