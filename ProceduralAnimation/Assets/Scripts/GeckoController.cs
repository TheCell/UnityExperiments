using UnityEngine;

/// <summary>
/// Tutorial by https://weaverdev.io/blog/bonehead-procedural-animation
/// </summary>
public class GeckoController : MonoBehaviour
{
	[SerializeField] private Transform target;

	// head
	[SerializeField] private Transform headBone;
	[SerializeField] private float headMaxTurnAngle = 30f;
	[SerializeField] private float headTrackingSpeed = 10f;

	// Eyes
	[SerializeField] Transform leftEyeBone;
	[SerializeField] Transform rightEyeBone;

	[SerializeField] private float eyeTrackingSpeed;
	[SerializeField] private float leftEyeMaxYRotation;
	[SerializeField] private float leftEyeMinYRotation;
	[SerializeField] private float rightEyeMaxYRotation;
	[SerializeField] private float rightEyeMinYRotation;


	private void LateUpdate()
	{
		// the order is important. Update parents before children
		HeadTrackingUpdate();
		EyeTrackingUpdate();
	}

	private void HeadTrackingUpdate()
	{
		Quaternion currentLocalRotation = headBone.localRotation;
		// Reset the head rotation so our world to local space transformation will use the head's zero rotation. 
		headBone.localRotation = Quaternion.identity;

		Vector3 targetWorldLookDir = target.position - headBone.position;
		Vector3 targetLocalLookDir = headBone.InverseTransformDirection(targetWorldLookDir);

		// Apply angle limit
		targetLocalLookDir = Vector3.RotateTowards(
			Vector3.forward,
			targetLocalLookDir,
			Mathf.Deg2Rad * headMaxTurnAngle, // Note we multiply by Mathf.Deg2Rad here to convert degrees to radians
			0
		);

		// Get the local rotation by using LookRotation on a local directional vector
		Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, transform.up);

		// Apply smoothing
		headBone.rotation = Quaternion.Slerp(
			currentLocalRotation,
			targetLocalRotation,
			1 - Mathf.Exp(-headTrackingSpeed * Time.deltaTime)
		);
	}

	private void EyeTrackingUpdate()
	{
		Quaternion targetEyeRotation = Quaternion.LookRotation(
			target.position - headBone.position,
			transform.up
		);

		leftEyeBone.rotation = Quaternion.Slerp(
			leftEyeBone.rotation,
			targetEyeRotation,
			1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime)
		);

		rightEyeBone.rotation = Quaternion.Slerp(
			rightEyeBone.rotation,
			targetEyeRotation,
			1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime)
		);

		float leftEyeCurrentYRotation = leftEyeBone.localEulerAngles.y;
		float rightEyeCurrentYRotation = rightEyeBone.localEulerAngles.y;

		if (leftEyeCurrentYRotation > 180)
		{
			leftEyeCurrentYRotation -= 360;
		}
		if (rightEyeCurrentYRotation > 180)
		{
			rightEyeCurrentYRotation -= 360;
		}

		float leftEyeClampedYRotation = Mathf.Clamp(
			leftEyeCurrentYRotation,
			leftEyeMinYRotation,
			leftEyeMaxYRotation
		);

		float rightEyeClampedYRotation = Mathf.Clamp(
			rightEyeCurrentYRotation,
			rightEyeMinYRotation,
			rightEyeMaxYRotation
		);

		leftEyeBone.localEulerAngles = new Vector3(
			leftEyeBone.localEulerAngles.x,
			leftEyeClampedYRotation,
			leftEyeBone.localEulerAngles.z
		);
		rightEyeBone.localEulerAngles = new Vector3(
			rightEyeBone.localEulerAngles.x,
			rightEyeClampedYRotation,
			rightEyeBone.localEulerAngles.z
		);
	}
}
