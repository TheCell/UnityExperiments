using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FKChain : MonoBehaviour
{
	[SerializeField] private Transform trackdot;
	[SerializeField] private Joint[] joints;
	private float samplingDistance;
	private float learningRate;
	private float distanceThreshold;
	private float[] angles;
	private float testangle = 0f;

	private void Start()
	{
		//joints = gameObject.GetComponentsInChildren<Joint>();
		angles = new float[joints.Length];
		for (int i = 0; i < joints.Length; i++)
		{
			float angle = 0f;
			if (joints[i].localAxis == Vector3.right)
			{
				angle = joints[i].transform.eulerAngles.x;
			}
			else if (joints[i].localAxis == Vector3.up)
			{
				angle = joints[i].transform.eulerAngles.y;
			}
			else
			{
				angle = joints[i].transform.eulerAngles.z;
			}

			angles[i] = angle;
		}
	}

	private void OnDrawGizmos()
	{
		DrawOffsets();
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.TransformPoint(ForwardKinematicsLocalPos()), 1f);
	}

	private void Update()
	{
		TestAngles(testangle);
		testangle++;
		//InverseKinematics(trackdot.position, angles);

		//for (int i = 0; i < angles.Length; i++)
		//{
		//	Debug.Log(angles[i]);
		//}
		//UpdatedLocalAngles();
	}

	//private void SetOffsets()
	//{
	//	Vector3 previousPosition = transform.position;

	//	for (int i = 0; i < joints.Length; i++)
	//	{
	//		Vector3 currentPos = joints[i].transform.position;
	//		joints[i].StartOffset = currentPos - previousPosition;
	//	}
	//}

	// every joint can only rotate around one local axis
	//public void InverseKinematics(Vector3 target, float[] angles)
	//{
	//	if (DistanceFromTarget(target, angles) < distanceThreshold)
	//	{
	//		return;
	//	}

	//	for (int i = joints.Length - 1; i >= 0; i--)
	//	{
	//		float gradient = PartialGradientDescent(target, angles, i);
	//		angles[i] -= learningRate * gradient;

	//		if (DistanceFromTarget(target, angles) < distanceThreshold)
	//		{
	//			return;
	//		}
	//	}
	//}

	//public float PartialGradientDescent(Vector3 target, float[] angles, int i)
	//{
	//	// save the current angle
	//	float angle = angles[i];

	//	// Gradient : [F(x+SamplingDistance) - F(x)] / h
	//	float f_x = DistanceFromTarget(target, angles);
	//	angles[i] += samplingDistance;
	//	float f_x_plus_d = DistanceFromTarget(target, angles);
	//	float gradient = (f_x_plus_d - f_x) / samplingDistance;

	//	// restore current angle
	//	angles[i] = angle;
	//	return gradient;
	//}

	//public float DistanceFromTarget(Vector3 target, float[] angles)
	//{
	//	Vector3 point = ForwardKinematics(angles);
	//	return Vector3.Distance(point, target);
	//}

	private void TestAngles(float angle)
	{
		for (int i = 0; i < joints.Length; i++)
		{
			angles[i] = angle;
		}

		UpdatedLocalAngles();
	}

	private void UpdatedLocalAngles()
	{
		for (int i = 0; i < joints.Length; i++)
		{
			joints[i].UpdateAngle(angles[i]);
		}
	}

	private Vector3 ForwardKinematicsLocalPos()
	{
		Vector3 previousPoint = joints[0].transform.localPosition;
		Quaternion rotation = Quaternion.identity;

		for (int i = 1; i < angles.Length; i++)
		{
			rotation *= Quaternion.AngleAxis(angles[i - 1], joints[i - 1].localAxis);
			previousPoint = previousPoint + rotation * joints[i].StartOffset;
		}

		return previousPoint;
	}

	private void DrawOffsets()
	{
		if (joints == null)
		{
			return;
		}

		Gizmos.color = Color.yellow;
		Vector3 previousPos = transform.position;

		for (int i = 1; i < joints.Length; i++)
		{
			Vector3 jointGlobalPos = joints[i - 1].transform.TransformPoint(joints[i].StartOffset);

			Gizmos.DrawLine(
				previousPos,
				jointGlobalPos
			);
			previousPos = jointGlobalPos;
			Gizmos.DrawSphere(previousPos, 1f);
		}
	}
}
