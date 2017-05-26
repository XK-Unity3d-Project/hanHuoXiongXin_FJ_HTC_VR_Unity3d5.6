using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XKAmmoHiddenCtrl : MonoBehaviour
{
	[Range(0, 10)]public float TimeHidden = 1;
	public GameObject[] HiddenArray;
	public void HiddenAmmoObj()
	{
		if (HiddenArray == null || HiddenArray.Length < 1) {
			return;
		}

		for (int i = 0; i < HiddenArray.Length; i++) {
			if (HiddenArray[i] != null) {
				HiddenArray[i].SetActive (false);
			}
		}
	}
}