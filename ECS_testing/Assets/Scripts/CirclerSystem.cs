using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class CirclerSystem : ComponentSystem
{
	protected override void OnUpdate()
	{
		Entities.WithAll<Circler>().ForEach((Entity entity, ref Rotator rotator, ref Rotation rotation) =>
		{
			//rotator.Rotation += rotator.RotationSpeed * Time.deltaTime;
			//rotation.Value = quaternion.RotateY(rotator.Rotation);
		});
	}
}
