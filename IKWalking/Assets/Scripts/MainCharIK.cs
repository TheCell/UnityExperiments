using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// second implementation with the tutorial from https://www.weaverdev.io/blog/bonehead-procedural-animation but with my own rig
public class MainCharIK : MonoBehaviour
{
	// The target we are going to track
	[SerializeField] private Transform lookTarget;
	// A reference to the gecko's neck
	[SerializeField] private Transform headBone;
	[SerializeField] private float headRotationSpeed = 0.1f;
	//[SerializeField] private float headMaxTurnAngle = 40f;

	private void Start()
	{
	}

	// We will put all our animation code in LateUpdate.
	// This allows other systems to update the environment first, 
	// allowing the animation system to adapt to it before the frame is drawn.
	void LateUpdate()
	{
		//Quaternion currentLocalRotation = headBone.localRotation;
		Quaternion currentRotation = headBone.rotation;
		// localrot is not zero in my case

		Vector3 targetWorldLookDir = lookTarget.position - headBone.position;
		//Vector3 targetLocalLookDir = headBone.InverseTransformDirection(targetWorldLookDir);

		//targetLocalLookDir = Vector3.RotateTowards(
		//	Vector3.forward,
		//	targetLocalLookDir,
		//	Mathf.Deg2Rad * headMaxTurnAngle,
		//	0
		//);

		//Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);
		//headBone.localRotation = targetLocalRotation;
		//headBone.localRotation = Quaternion.Slerp(
		//		currentLocalRotation,
		//		targetLocalRotation,
		//		1 - Thecelleu.Utilities.Damp(1, headRotationSpeed, Time.deltaTime)
		//	);

		Quaternion targetRotation = Quaternion.LookRotation(targetWorldLookDir, transform.up);
		headBone.rotation = Quaternion.Slerp(headBone.rotation, targetRotation, 1 - Thecelleu.Utilities.Damp(1, headRotationSpeed, Time.deltaTime));
	}
}
