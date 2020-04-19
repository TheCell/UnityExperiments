using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// thanks to https://www.alanzucconi.com/2017/04/17/procedural-animations/
// but there were a lot of things mixed up (worldpositions and localpositions)
public class FKChain : MonoBehaviour
{
	[SerializeField] private Transform trackdot;
	[SerializeField] private Joint[] joints;
	private float deltaAngleToTestWith = 10f;
	private float learningRate = 5f;
	// stop jittering at the target
	private float distanceThreshold = 0.01f;
	private float[] angles;
	private float debugAngle = 0f;
	private bool drawInRunmode = false;

	private void Start()
	{
		drawInRunmode = true;

		// we get the joints manually to ensure order
		angles = new float[joints.Length];
		for (int i = 0; i < joints.Length; i++)
		{
			float angle = 0f;

			angles[i] = angle;
		}
	}

	private void OnDrawGizmos()
	{
		if (drawInRunmode)
		{
			DrawOffsets();
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(ForwardKinematics(angles), 1f);
		}
	}

	private void Update()
	{
		//TestAngles(testangle);
		//testangle++;
		//UpdatedLocalAngles();
		InverseKinematics();
	}

	// every joint can only rotate around one local axis
	public void InverseKinematics()
	{
		Vector3 target = trackdot.position;

		if (RealDistanceFromTarget(target, angles) < distanceThreshold)
		{
			return;
		}

		for (int i = joints.Length - 1; i >= 0; i--)
		{
			float gradient = PartialGradientDescent(i);
			angles[i] -= learningRate * gradient;

			angles[i] = Mathf.Clamp(
				angles[i], 
				joints[i].MinAngle, 
				joints[i].MaxAngle);

			if (RealDistanceFromTarget(target, angles) < distanceThreshold)
			{
				return;
			}
		}

		UpdatedLocalAngles();
	}

	public float PartialGradientDescent(int i)
	{
		// save the current angle
		float angle = angles[i];
		Vector3 target = trackdot.position;

		// Gradient : [F(x+SamplingDistance) - F(x)] / h
		float f_x = RealDistanceFromTarget(target, angles);
		angles[i] += deltaAngleToTestWith;
		float f_x_plus_d = RealDistanceFromTarget(target, angles);
		// why do we do a division by delta angle?
		//float gradient = (f_x_plus_d - f_x) / deltaAngleToTestWith;
		float gradient = (f_x_plus_d - f_x);
		//Debug.Log("gradient for " + i + " is " + gradient);
		//Debug.Log(i + " current Dist: " + f_x + " new dist: " + f_x_plus_d + " results in " + gradient);

		// restore current angle
		angles[i] = angle;
		return gradient;
	}

	public float RealDistanceFromTarget(Vector3 target, float[] testAngles)
	{
		Vector3 point = ForwardKinematics(testAngles);
		return Vector3.Distance(point, target);
	}

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

	// returns the position of the end effector
	private Vector3 ForwardKinematics(float[] anglesToTestWith)
	{
		Vector3 previousPoint = joints[0].transform.localPosition;
		Quaternion rotation = Quaternion.Euler(joints[0].startLocalAngle);

		for (int i = 1; i < angles.Length; i++)
		{
			rotation *= Quaternion.AngleAxis(
				anglesToTestWith[i - 1], 
				joints[i - 1].localAxis);
			previousPoint = previousPoint + rotation * joints[i].StartOffset;
		}

		return transform.TransformPoint(previousPoint);
	}

	private void DrawOffsets()
	{
		Gizmos.color = Color.yellow;
		Vector3 previousPoint = joints[0].transform.localPosition;
		Quaternion rotation = Quaternion.Euler(joints[0].startLocalAngle);

		for (int i = 1; i < angles.Length; i++)
		{
			rotation *= Quaternion.AngleAxis(
				angles[i - 1], 
				joints[i - 1].localAxis);
			previousPoint = previousPoint + rotation * joints[i].StartOffset;
			Gizmos.DrawSphere(transform.TransformPoint(previousPoint), 1f);
		}
	}
}
