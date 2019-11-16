using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HelpURL("http://example.com/docs/MyComponent.html")]
public class ImprovedInspector : MonoBehaviour
{
	//[ContextMenuItem("Reset", "ResetBiography")]
	[Multiline(8)]
	[SerializeField] private string playerBiography = "";
	/*
	[ContextMenu("This script is intended to help you style your scripts")]
	private void ContextMenu()
	{
		Debug.Log("Do Something");
	}
	*/
	[Header("Health Settings")]
	[SerializeField] private int health = 0;
	[SerializeField] private int maxHealth = 100;

	[Header("Shield Settings")]
	[SerializeField] private int shield = 0;
	[SerializeField] private int maxShield = 0;

	[HideInInspector]
	public int changeAbleFromOutside = 1; // this will not show up in the inspector

	[Space(10)]
	[SerializeField] private ModelImporterIndexFormat model;
	private enum ModelImporterIndexFormat
	{
		Auto = 0,
		[InspectorName("16 bits")]
		UInt16 = 1,
		[InspectorName("32 bits")]
		UInt32 = 2,
	}

	[Space(100)]
	[SerializeField] private int spaceExample = 0;
	[TextArea(4, 8)] // or just [TextArea]
	[SerializeField] private string exampleField;
	[Tooltip("The field does nothing, really")]
	[SerializeField] private int mysteriousField = 1;
}
