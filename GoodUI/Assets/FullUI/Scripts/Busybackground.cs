using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Busybackground : MonoBehaviour
{
    private float randomRotation;

    private void Start()
    {
        randomRotation = Random.Range(-15f, 15f);
    }
    private void Update()
    {
        transform.rotation = transform.rotation * Quaternion.Euler(0, randomRotation * Time.deltaTime, 0);
    }
}
