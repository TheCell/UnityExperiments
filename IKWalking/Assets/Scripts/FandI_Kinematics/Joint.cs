using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint : MonoBehaviour
{
	public Vector3 Axis;
	public Vector3 StartOffset;

	void Awake()
	{
		StartOffset = transform.localPosition;
	}
}
