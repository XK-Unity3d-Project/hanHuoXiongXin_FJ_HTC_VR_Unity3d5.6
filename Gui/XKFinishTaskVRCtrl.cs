using UnityEngine;
using System.Collections;

public class XKFinishTaskVRCtrl : MonoBehaviour
{
	public GameObject VRUIRootObj;
	public GameObject VRCameraObj;
	static XKFinishTaskVRCtrl _Instance;
	public static XKFinishTaskVRCtrl GetInstance()
	{
		return _Instance;
	}
	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		VRUIRootObj.SetActive(false);
	}

	public void ShowFinishTask()
	{
		if (XKPlayerGunRotCtrl.GetInstanceOne() != null) {
			XKPlayerGunRotCtrl.GetInstanceOne().gameObject.SetActive(false);
		}
		if (XKPlayerGunRotCtrl.GetInstanceTwo() != null) {
			XKPlayerGunRotCtrl.GetInstanceTwo().gameObject.SetActive(false);
		}
		TweenAlpha twAlpha = gameObject.AddComponent<TweenAlpha>();
		twAlpha.enabled = false;
		twAlpha.from = 0f;
		twAlpha.to = 1f;
		EventDelegate.Add(twAlpha.onFinished, delegate{
			Invoke("OnFinishTaskAlphaToEnd", 2f);
		});
		twAlpha.enabled = true;
		twAlpha.PlayForward();

		VRUIRootObj.SetActive(true);
	}

	void OnFinishTaskAlphaToEnd()
	{
		VRCameraObj.SetActive(false);
		//loading movie scene.
		XkGameCtrl.LoadingGameMovie();
	}
}