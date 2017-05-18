using UnityEngine;
using System.Collections;

public class XKSpawnBulletParticle : MonoBehaviour
{
	public ParticleEmitter[] capsuleEmitter;
	bool IsFireTrigger;
	bool IsFireState = true;
	float TimeLastVal;
	static XKSpawnBulletParticle _InstanceOne;
	public static XKSpawnBulletParticle GetInstanceOne()
	{
		return _InstanceOne;
	}
	// Use this for initialization
	void Start()
	{
		_InstanceOne = this;
		SetFireParticleState(false);
		SetIsFireTrigger(true); //test.
	}

	// Update is called once per frame
	void Update()
	{
		if (!IsFireTrigger) {
			return;
		}

		if (Time.realtimeSinceStartup - TimeLastVal < 0.1f) {
			SetFireParticleState(false);
			return;
		}
		TimeLastVal = Time.realtimeSinceStartup;
		SetFireParticleState(true);
	}

	public void SetIsFireTrigger(bool isFireEvent)
	{
		IsFireTrigger = isFireEvent;
	}

	void SetFireParticleState(bool isFire)
	{
		if (IsFireState == isFire) {
			return;
		}
		IsFireState = isFire;

		if (isFire) {
			if(capsuleEmitter != null) {
				for(int i = 0; i < capsuleEmitter.Length; i++) {
					if (capsuleEmitter[i] != null) {
						capsuleEmitter[i].Emit();
					}
				}
			}
		}
		else {
			if(capsuleEmitter != null) {
				for(int i = 0; i < capsuleEmitter.Length; i++) {
					if (capsuleEmitter[i] != null) {
						capsuleEmitter[i].emit = false;
					}
				}
			}
		}

	}
}