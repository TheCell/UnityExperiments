using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMoveScript : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime);
        }
    }
}
