using UnityEngine;
using System.Collections;

public class TestNpcAni : MonoBehaviour {
	public Transform[] GunTrArray;
	public Transform[] ForearmTrArray;
	public Transform[] HandTrArray;
	// Update is called once per frame
	void LateUpdate()
	{
//		HandTrArray[0].position = GunTrArray[0].position;
//		HandTrArray[1].position = GunTrArray[1].position;
		HandTrArray[0].forward = GunTrArray[0].forward;
		HandTrArray[1].forward = GunTrArray[1].forward;
	}
}