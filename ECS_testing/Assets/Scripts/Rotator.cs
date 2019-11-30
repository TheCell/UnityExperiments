using UnityEngine;
using Unity.Entities;

public struct Rotator : IComponentData
{
	public float Rotation;
	public float RotationSpeed;
}
