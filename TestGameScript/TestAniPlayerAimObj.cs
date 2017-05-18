using UnityEngine;
using System.Collections;

public class TestAniPlayerAimObj : MonoBehaviour
{
	Quaternion headRotation;
	Vector3 facingDirection;
	public float aimingSpeed = 100f;
	public Transform head;
	public Transform AimTargetTr;
	void LateUpdate()
	{
		CheckAimTarget();
	}
	
	void CheckAimTarget()
	{
		if (head == null || AimTargetTr == null) {
			return;
		}
		
		facingDirection = AimTargetTr.position - head.position;
		// Target with head
		if (facingDirection != Vector3.zero) {
			Quaternion targetRotation = Quaternion.LookRotation (facingDirection);
			headRotation = Quaternion.RotateTowards (
				headRotation,
				targetRotation,
				aimingSpeed * Time.deltaTime
				);
			head.rotation = headRotation * Quaternion.Inverse (transform.rotation) * head.rotation;
		}
	}
}