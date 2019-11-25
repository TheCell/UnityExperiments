using System;
using Unity.Entities;

namespace happyShips.ECS
{
    [Serializable]
	public struct MoveSpeed : IComponentData
	{
		public float Value;
	}
}
