using System.Collections;
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

	// Legwork
	[SerializeField] private LegStepper frontRightLegStepper;
	[SerializeField] private LegStepper frontLeftLegStepper;
	[SerializeField] private LegStepper backRightLegStepper;
	[SerializeField] private LegStepper backLeftLegStepper;

	// Movement
	[SerializeField] private float turnSpeed;
	[SerializeField] private float moveSpeed;

	[SerializeField] private float turnAcceleration;
	[SerializeField] private float moveAcceleration;

	[SerializeField] private float minDistToTarget;
	[SerializeField] private float maxDistToTarget;

	[SerializeField] private float maxAngToTarget;

	private Vector3 currentVelocity;
	private float currentAngularVelocity;

	private void Awake()
	{
		StartCoroutine(LegUpdateCoroutine());
	}

	private void LateUpdate()
	{
		RootMotionUpdate();
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
		Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

		// Apply smoothing
		headBone.localRotation = Quaternion.Slerp(
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

	private void RootMotionUpdate()
	{
		// rotation
		Vector3 towardTarget = target.position - transform.position;
		Vector3 towardTargetProjected = Vector3.ProjectOnPlane(towardTarget, transform.up);
		float angToTarget = Vector3.SignedAngle(transform.forward, towardTargetProjected, transform.up);
		float targetAngularVelocity = 0;

		if (Mathf.Abs(angToTarget) > maxAngToTarget)
		{
			if (angToTarget > 0)
			{
				targetAngularVelocity = turnSpeed;
			}
			else
			{
				targetAngularVelocity = -turnSpeed;
			}
		}

		currentAngularVelocity = Mathf.Lerp
			(
				currentAngularVelocity,
				targetAngularVelocity,
				1 - Mathf.Exp(-turnAcceleration * Time.deltaTime)
			);

		transform.Rotate(0, Time.deltaTime * currentAngularVelocity, 0, Space.World);

		// translation
		Vector3 targetVelocity = Vector3.zero;
		if (Mathf.Abs(angToTarget) < 90)
		{
			float distToTarget = Vector3.Distance(transform.position, target.position);

			if (distToTarget > maxDistToTarget)
			{
				targetVelocity = moveSpeed * towardTargetProjected.normalized;
			}
			else if (distToTarget < minDistToTarget)
			{
				targetVelocity = moveSpeed * -towardTargetProjected.normalized;
			}
		}

		currentVelocity = Vector3.Lerp
			(
			currentVelocity,
			targetVelocity,
			1 - Mathf.Exp(-moveAcceleration * Time.deltaTime)
			);
		transform.position += currentVelocity * Time.deltaTime;
	}

	IEnumerator LegUpdateCoroutine()
	{
		while (true)
		{
			do
			{
				frontLeftLegStepper.TryMove();
				backRightLegStepper.TryMove();
				yield return null;
			}
			while (backRightLegStepper.isMoving || frontLeftLegStepper.isMoving);

			do
			{
				frontRightLegStepper.TryMove();
				backLeftLegStepper.TryMove();
				yield return null;
			}
			while (backLeftLegStepper.isMoving || frontRightLegStepper.isMoving);
		}
	}
}
