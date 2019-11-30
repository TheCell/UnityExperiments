using UnityEngine;
using Unity.Entities;

public class Circler : IComponentData
{
	public Vector3 circleCenter;
	public float speed;
	public float distanceToCenter;
}
