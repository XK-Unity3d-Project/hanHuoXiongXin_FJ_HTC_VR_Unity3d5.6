using UnityEngine;
using System.Collections;

public class XKTriggerObjActiveCtrl : MonoBehaviour
{
	public bool IsActiveLvDeng;
	public bool IsActiveHongDeng;
	public AiPathCtrl TestPlayerPath;
	
	void OnTriggerEnter(Collider other)
	{
		XkPlayerCtrl ScriptPlayer = other.GetComponent<XkPlayerCtrl>();
		if (ScriptPlayer == null) {
			return;
		}

		ScriptPlayer.LvDengObj.SetActive(IsActiveLvDeng);
		ScriptPlayer.HongDengObj.SetActive(IsActiveHongDeng);
	}
	
	void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}
		
		if (!enabled) {
			return;
		}

		if (TestPlayerPath != null) {
			TestPlayerPath.DrawPath();
		}
	}
}