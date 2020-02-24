using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[AddComponentMenu("UI/Blur Panel")]
public class BlurPanel : Image
{
    public bool animate;
    public float time = 0.5f;
    public float delay = 0f;

    private CanvasGroup canvas;

    protected override void Reset()
    {
        base.Reset();
        //color = Color.black * 0.1f;
    }

    protected override void Awake()
    {
        //base.Awake();
        canvas = GetComponent<CanvasGroup>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (Application.isPlaying)
        {
            canvas.alpha = 0;
            StartCoroutine("Fader");
        }
    }

    private IEnumerator Fader()
    {
        float startTime = Time.realtimeSinceStartup + delay;
        while (Time.realtimeSinceStartup < startTime)
        {
            yield return null;
        }

        while (Time.realtimeSinceStartup < startTime + time)
        {
            canvas.alpha = Mathf.Lerp(0, 1, Mathf.InverseLerp(startTime, startTime + time, Time.realtimeSinceStartup));
            yield return null;
        }

        canvas.alpha = 1.0f;
    }
}
