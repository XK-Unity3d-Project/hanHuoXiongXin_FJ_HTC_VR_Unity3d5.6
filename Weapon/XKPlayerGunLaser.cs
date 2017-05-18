using UnityEngine;
using System.Collections;

public class XKPlayerGunLaser : MonoBehaviour
{
	public PlayerEnum PlayerSt = PlayerEnum.PlayerOne;
	public float scrollSpeed = 0.5f;
	public float pulseSpeed = 1.5f;
	public float noiseSize = 1.0f;
	public float maxWidth = 0.5f;
	public float minWidth = 0.2f;
	public GameObject pointer = null;
	public Transform PlayerCursorTr;
	public LayerMask LaserHit;
	private LineRenderer lRenderer;
	private Transform tr;
	private float aniDir = 1.0f;
	
	public float MinPointScl;
	public float MaxPointScl;
	public float MinPointDis;
	public float MaxPointDis;
	float KeyPointScl;
	float CurPoinScl;
	float CurPointDis;
	float TimeAniVal;
	static XKPlayerGunLaser _InstanceOne;
	public static XKPlayerGunLaser GetInstanceOne()
	{
		return _InstanceOne;
	}
	static XKPlayerGunLaser _InstanceTwo;
	public static XKPlayerGunLaser GetInstanceTwo()
	{
		return _InstanceTwo;
	}
	void Awake()
	{
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			_InstanceOne = this;
			break;
		case PlayerEnum.PlayerTwo:
			_InstanceTwo = this;
			break;
		}
	}

	void Start()
	{
		tr = transform;
		KeyPointScl = (MaxPointScl - MinPointScl) / (MaxPointDis - MinPointDis);
		lRenderer = gameObject.GetComponent<LineRenderer>();
		
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			SetActivePlayerLaser(XkGameCtrl.IsActivePlayerOne);
			break;
		case PlayerEnum.PlayerTwo:
			SetActivePlayerLaser(XkGameCtrl.IsActivePlayerTwo);
			break;
		}
		
		// Change some animation values here and there
		//StartCoroutine(ChoseNewAnimationTargetCoroutine());
	}

	void Update()
	{
		UpdatePlayerLaser();
		UpdateCursorScl();
		ChoseNewAnimationTarget();
//		UpdatePointerScl();
	}
	
	void UpdateCursorScl()
	{
		KeyPointScl = (MaxPointScl - MinPointScl) / (MaxPointDis - MinPointDis); //test
		CurPointDis = Vector3.Distance(PlayerCursorTr.transform.position, tr.position);
		CurPoinScl = KeyPointScl * (CurPointDis - MinPointDis) + MinPointScl;
		PlayerCursorTr.transform.localScale = new Vector3(CurPoinScl, CurPoinScl, CurPoinScl);
	}

//	void UpdatePointerScl()
//	{
//		CurPointDis = Vector3.Distance(pointer.transform.position, tr.position);
//		CurPoinScl = KeyPointScl * (CurPointDis - MinPointDis) + MinPointScl;
//		pointer.transform.localScale = new Vector3(CurPoinScl, CurPoinScl, CurPoinScl);
//	}

	void UpdatePlayerLaser()
	{
		Vector2 veOffset = GetComponent<Renderer>().material.mainTextureOffset;
		veOffset.x += Time.deltaTime * aniDir * scrollSpeed;
		GetComponent<Renderer>().material.mainTextureOffset = veOffset;
		GetComponent<Renderer>().material.SetTextureOffset("_NoiseTex", new Vector2(-Time.time * aniDir * scrollSpeed, 0.0f));
		
		float aniFactor = Mathf.PingPong (Time.time * pulseSpeed, 1.0f);
		aniFactor = Mathf.Max (minWidth, aniFactor) * maxWidth;
		lRenderer.SetWidth (aniFactor, aniFactor);
		
		// Cast a ray to find out the end point of the laser
		RaycastHit hitInfo;
		if (Physics.Raycast(tr.position, tr.forward, out hitInfo, 500f, LaserHit.value)) {
			lRenderer.SetPosition (1, (hitInfo.distance * Vector3.forward));
			veOffset = GetComponent<Renderer>().material.mainTextureScale;
			veOffset.x = 0.1f * (hitInfo.distance);
			GetComponent<Renderer>().material.mainTextureScale = veOffset;
			GetComponent<Renderer>().material.SetTextureScale ("_NoiseTex", new Vector2(0.1f * hitInfo.distance * noiseSize, noiseSize));		
			
			// Use point and normal to align a nice & rough hit plane
			//if (pointer) {
				//pointer.renderer.enabled = true;
				//pointer.transform.position = hitInfo.point + (tr.position - hitInfo.point) * 0.01f;
			//}
			PlayerCursorTr.position = hitInfo.point + (tr.position - hitInfo.point) * 0.01f;
		}
		else {
//			if (pointer) {
//				pointer.renderer.enabled = false;
//			}
			float maxDist = 200.0f;
			lRenderer.SetPosition (1, (maxDist * Vector3.forward));
			veOffset = GetComponent<Renderer>().material.mainTextureScale;
			veOffset.x = 0.1f * (maxDist);
			GetComponent<Renderer>().material.mainTextureScale = veOffset;
			GetComponent<Renderer>().material.SetTextureScale ("_NoiseTex", new Vector2(0.1f * (maxDist) * noiseSize, noiseSize));
			PlayerCursorTr.position = tr.forward * 300f + tr.position;	
		}
	}

	void ChoseNewAnimationTarget()
	{
		if (Time.realtimeSinceStartup - TimeAniVal < 1f) {
			return;
		}
		TimeAniVal = Time.realtimeSinceStartup;
		aniDir = aniDir * 0.9f + Random.Range (0.5f, 1.5f) * 0.1f;
		minWidth = minWidth * 0.8f + Random.Range (0.1f, 1.0f) * 0.2f;
	}

	public void SetActivePlayerLaser(bool isActive)
	{
		gameObject.SetActive(isActive);
		PlayerCursorTr.gameObject.SetActive(isActive);
	}
}