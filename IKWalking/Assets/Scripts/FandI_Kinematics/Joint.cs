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

	public float MinAngle;
	public float MaxAngle;

	public Vector3 StartOffset { get => startOffset; }

	private void OnDrawGizmosSelected()
	{
		//Gizmos.color = Color.yellow;
		//Gizmos.DrawSphere(transform.position, 1);

		//Debug.Log("Joint position: " + transform.position + " and local Position: " + transform.localPosition);
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
		Debug.Log(eulerAngles);
	}
}
