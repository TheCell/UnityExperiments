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
	[SerializeField] private float headRotationSpeed;

	// We will put all our animation code in LateUpdate.
	// This allows other systems to update the environment first, 
	// allowing the animation system to adapt to it before the frame is drawn.
	void LateUpdate()
	{
		Vector3 towardObjectFromHead = lookTarget.position - headBone.position;
		Quaternion targetRotation = Quaternion.LookRotation(towardObjectFromHead, transform.up);
		headBone.rotation = Quaternion.Slerp(headBone.rotation, targetRotation, 1 - Thecelleu.Utilities.Damp(1, headRotationSpeed, Time.deltaTime));
	}
}
