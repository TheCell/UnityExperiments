using UnityEngine;
using Unity.Entities;

public class Circler : IComponentData
{
	public Vector3 circleCenter;
	public Vector3 objectPosition;
	public Quaternion rotation;
}
