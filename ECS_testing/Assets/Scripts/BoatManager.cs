using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;

// this is based on https://www.patrykgalach.com/2019/07/01/unity-ecs-introduction/
public class BoatManager : MonoBehaviour
{
	private EntityManager entityManager;

	[Header("Entities Config")]
	[SerializeField] private int numberOfEntitiesToSpawn = 10;

	[SerializeField] private BoatSettings config = new BoatSettings();

	private void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		EntityManager entityManager = World.Active.EntityManager;

		NativeArray<Entity> cubeEntities = new NativeArray<Entity>(numberOfEntitiesToSpawn, Allocator.Temp);

		EntityArchetype cubeArchetype = entityManager.CreateArchetype(
			typeof(RenderMesh),
			typeof(LocalToWorld),
			typeof(Translation),
			typeof(Rotation),
			typeof(NonUniformScale),
			typeof(Rotator)
			);

		entityManager.CreateEntity(cubeArchetype, cubeEntities);

		Unity.Mathematics.Random rnd = new Unity.Mathematics.Random();
		rnd.InitState((uint)System.DateTime.UtcNow.Ticks);

		for (int i = 0; i < cubeEntities.Length; i++)
		{
			Entity cubeEntity = cubeEntities[i];
			entityManager.SetSharedComponentData(cubeEntity, new RenderMesh { mesh = config.Mesh, material = config.Material });
			entityManager.SetComponentData(cubeEntity, new Translation { Value = rnd.NextFloat3(new float3(config.MinEntityPosition.x, 0, config.MinEntityPosition.y), new float3(config.MaxEntityPosition.x, 0, config.MaxEntityPosition.y)) });
			entityManager.SetComponentData(cubeEntity, new NonUniformScale { Value = rnd.NextFloat3(new float3(config.MinEntityScale.x, config.MinEntityScale.y, config.MinEntityScale.x), new float3(config.MaxEntityScale.x, config.MaxEntityScale.y, config.MaxEntityScale.x)) });
			entityManager.SetComponentData(cubeEntity, new Rotator { Rotation = rnd.NextFloat(0, 360), RotationSpeed = rnd.NextFloat(config.MinSpin, config.MaxSpin) });
		}

		cubeEntities.Dispose();
	}
}
