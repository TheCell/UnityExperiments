using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// second implementation with the tutorial from https://www.weaverdev.io/blog/bonehead-procedural-animation but with my own rig
public class MainCharIK : MonoBehaviour
{
	[SerializeField] private Transform lookTarget;
	[SerializeField] private Transform headBone;
	[SerializeField] private float headRotationSpeed = 0.1f;
	[SerializeField] private float headMaxTurnAngle = 40f;

	[SerializeField] LegStepper frontLeftLeg;
	[SerializeField] LegStepper frontRightLeg;
	[SerializeField] LegStepper backLeftLeg;
	[SerializeField] LegStepper backRightLeg;

	private void Awake()
	{
		StartCoroutine(LegUpdateCoroutine());
	}

	private void Start()
	{
	}

	private void Update()
	{
		
	}

	// We will put all our animation code in LateUpdate.
	// This allows other systems to update the environment first, 
	// allowing the animation system to adapt to it before the frame is drawn.
	void LateUpdate()
	{
		Quaternion currentLocalRotation = headBone.localRotation;
		headBone.localRotation = Quaternion.identity;

		Vector3 targetWorldLookDir = lookTarget.position - headBone.position;
		Vector3 targetLocalLookDir = headBone.InverseTransformDirection(targetWorldLookDir);

		targetLocalLookDir = Vector3.RotateTowards(
			Vector3.forward,
			targetLocalLookDir,
			Mathf.Deg2Rad * headMaxTurnAngle,
			0
		);

		Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

		headBone.localRotation = Quaternion.Slerp(
			currentLocalRotation,
			targetLocalRotation,
			1 - Thecelleu.Utilities.Damp(1, headRotationSpeed, Time.deltaTime)
		);
	}

	IEnumerator LegUpdateCoroutine()
	{
		while (true)
		{
			do
			{
				frontLeftLeg.TryMove();
				backRightLeg.TryMove();
				yield return null;
			}
			while (backRightLeg.IsMoving || frontLeftLeg.IsMoving);

			do
			{
				frontRightLeg.TryMove();
				backLeftLeg.TryMove();
				yield return null;
			}
			while (backLeftLeg.IsMoving || frontRightLeg.IsMoving);
		}
	}
}
