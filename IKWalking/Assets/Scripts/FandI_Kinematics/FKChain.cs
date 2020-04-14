using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FKChain : MonoBehaviour
{
	private Joint[] joints;
	private float samplingDistance;
	private float learningRate;

	private void Start()
	{
		joints = gameObject.GetComponentsInChildren<Joint>();
	}

	public float DistanceFromTarget (Vector3 target, float[] angles)
	{
		Vector3 point = ForwardKinematics(angles);
		return Vector3.Distance(point, target);
	}

	public Vector3 ForwardKinematics (float[] angles)
	{
		Vector3 previousPoint = joints[0].transform.position;
		Quaternion rotation = Quaternion.identity;
		for (int i = 1; i < angles.Length; i++)
		{
			rotation *= Quaternion.AngleAxis(angles[i - 1], joints[i - 1].Axis);
			Vector3 nextPoint = previousPoint + rotation * joints[i].StartOffset;

			previousPoint = nextPoint;
		}

		return previousPoint;
	}

	public void InverseKinematics (Vector3 target, float[] angles)
	{
		for (int i = 0; i < joints.Length; i++)
		{
			float gradient = PartialGradientDescent(target, angles, i);
			angles[i] -= learningRate * gradient;
		}
	}

	public float PartialGradientDescent(Vector3 target, float[] angles, int i)
	{
		// save the current angle
		float angle = angles[i];

		// Gradient : [F(x+SamplingDistance) - F(x)] / h
		float f_x = DistanceFromTarget(target, angles);
		angles[i] += samplingDistance;
		float f_x_plus_d = DistanceFromTarget(target, angles);
		float gradient = (f_x_plus_d - f_x) / samplingDistance;

		// restore current angle
		angles[i] = angle;
		return gradient;
	}
}
