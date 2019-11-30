using UnityEngine;

[System.Serializable]
public class BoatSettings
{
	[Header("Rendering")]
	[SerializeField] private Mesh mesh;
	[SerializeField] private Material material;

	[Header("Position")]
	[SerializeField] private Vector2 minEntityPosition;
	[SerializeField] private Vector2 maxEntityPosition;

	[Header("Spin")]
	[SerializeField] private float minSpin;
	[SerializeField] private float maxSpin;

	[Header("Scale")]
	[SerializeField] private Vector2 minEntityScale;
	[SerializeField] private Vector2 maxEntityScale;


	public Mesh Mesh { get => mesh; set => mesh = value; }
	public Material Material { get => material; set => material = value; }
	public Vector2 MinEntityPosition { get => minEntityPosition; set => minEntityPosition = value; }
	public Vector2 MaxEntityPosition { get => maxEntityPosition; set => maxEntityPosition = value; }
	public float MinSpin { get => minSpin; set => minSpin = value; }
	public float MaxSpin { get => maxSpin; set => maxSpin = value; }
	public Vector2 MinEntityScale { get => minEntityScale; set => minEntityScale = value; }
	public Vector2 MaxEntityScale { get => maxEntityScale; set => maxEntityScale = value; }
}
