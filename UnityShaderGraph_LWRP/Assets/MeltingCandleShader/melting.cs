using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class melting : MonoBehaviour
{
	private Material candleMaterial;
	[SerializeField] private Transform flamePos;
	[SerializeField] private float secondsToMelt = 3.0f;
	private float amount;
	public Transform flameStartPos;
	public Transform flameEndPos;
	//private Vector3 startScale;

	// Start is called before the first frame update
	void Start()
    {
		candleMaterial = gameObject.GetComponent<Renderer>().material;
		//startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
		amount = 1 - (Time.timeSinceLevelLoad / secondsToMelt) % 1;
		candleMaterial.SetFloat("_Amount", amount);
		if (flamePos != null)
		{
			flamePos.position = Vector3.Lerp(flameEndPos.position, flameStartPos.position, amount);
		}
		//transform.localScale = new Vector3(startScale.x, startScale.y * amount, startScale.z);
	}
}
