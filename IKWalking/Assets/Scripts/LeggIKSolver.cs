using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeggIKSolver : MonoBehaviour
{
    private void Start()
    {
        
    }

	private void Update()
    {
        
    }

	private float PartialGradient (Vector3 target, float[] angles, int i)
	{
		float angle = angles[i];

		// Gradient: [F:(x + SamplingDistance) - F(x)] / h
		

		return 1.0f;
	}
}
