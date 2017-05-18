using UnityEngine;
using System.Collections;

public class TestSpawnTeXiaoCtrl : MonoBehaviour
{
	static bool IsCanSpawnExp = true;
	public GameObject ExpObj;
	public float TimeVal = 3f;
	// Use this for initialization
	void Start()
	{
		if (!IsCanSpawnExp) {
			return;
		}
		InvokeRepeating("SpawnExpObj", 0f, TimeVal);
	}
	
	void SpawnExpObj()
	{
		Instantiate(ExpObj, transform.position, transform.rotation);
	}
}