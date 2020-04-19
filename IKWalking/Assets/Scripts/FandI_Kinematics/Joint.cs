using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint : MonoBehaviour
{
	// saved which axis the rotation is
	public Vector3 localAxis = Vector3.right;
	public Vector3 startLocalAngle;
	// offsets in global space
	private Vector3 startOffset;
	
	public float MinAngle = 0;
	public float MaxAngle = 360;

	public Vector3 StartOffset { get => startOffset; }

	private void OnDrawGizmosSelected()
	{
		//Gizmos.color = Color.yellow;
		//Gizmos.DrawSphere(transform.position, 1);
	}

	private void Awake()
	{
		startOffset = transform.localPosition;
		startLocalAngle = transform.localEulerAngles;
	}
	
	public void UpdateAngle(float angle)
	{
		Vector3 eulerAngles = startLocalAngle;
		if (localAxis == Vector3.right)
		{
			eulerAngles.x = angle;
		}
		else if (localAxis == Vector3.up)
		{
			eulerAngles.y = angle;
		}
		else
		{
			eulerAngles.z = angle;
		}

		transform.localEulerAngles = eulerAngles;
	}
}
