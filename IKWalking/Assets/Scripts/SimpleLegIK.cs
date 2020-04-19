using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLegIK : MonoBehaviour
{
    [SerializeField] private Transform kneeTarget;
    [SerializeField] private Transform legOrigin;
    [SerializeField] private Transform lowerLegOrigin;

    [SerializeField] private Transform footPlacement;

    private void Start()
    {
    }

    private void Update()
    {
        UpdateLegPos();
    }

    private void UpdateLegPos()
    {
        legOrigin.transform.LookAt(kneeTarget, legOrigin.transform.up);
        lowerLegOrigin.transform.LookAt(footPlacement, legOrigin.transform.up);

    }
}
