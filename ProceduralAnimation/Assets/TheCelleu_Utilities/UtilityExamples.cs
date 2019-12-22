using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityExamples : MonoBehaviour
{
    [SerializeField] private GameObject dampLerpCubeZero;
    [SerializeField] private GameObject dampLerpCubeAtoB;
    private bool dampLerpReverse = false;
    private float dampMinimalZ;
    private float dampMaximalZ;

    private void Start()
    {
        dampMinimalZ = dampLerpCubeAtoB.transform.position.z;
        dampMaximalZ = dampMaximalZ + 5f;
    }

    private void Update()
    {
        DampExample();
    }

    private void DampExample()
    {
        // from A to Zero
        Vector3 posToZero = dampLerpCubeZero.transform.position;
        posToZero.z = Thecelleu.Utilities.Damp(posToZero.z, 0.3f, Time.deltaTime);
        dampLerpCubeZero.transform.position = posToZero;

        // from A to B
        Vector3 pos = dampLerpCubeAtoB.transform.position;
        if (dampLerpReverse)
        {
            pos.z = Thecelleu.Utilities.Damp(pos.z, dampMinimalZ, 0.7f, Time.deltaTime);
        }
        else
        {
            pos.z = Thecelleu.Utilities.Damp(pos.z, dampMaximalZ, 0.3f, Time.deltaTime);
        }
        dampLerpCubeAtoB.transform.position = pos;

        if (pos.z >= dampMaximalZ - 0.1f) // because it is theoretically never gonna hit the dampMaximalZ
        {
            dampLerpReverse = !dampLerpReverse;
        }
        else if (pos.z <= dampMinimalZ + 0.1f) // because it is theoretically never gonna hit the dampMinimalZ
        {
            dampLerpReverse = !dampLerpReverse;
        }
    }
}
