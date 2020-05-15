using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslationKeyTestAccess : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(TranslationFromKeysManager.Instance.GetStringForID(0));
    }
}
