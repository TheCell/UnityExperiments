using UnityEngine;

// https://catlikecoding.com/unity/tutorials/movement/orbit-camera/
[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
	[SerializeField]
	private Transform focus = default;
	[SerializeField, Range(1f, 20f)]
	private float distance = 5f;
	[SerializeField, Min(0f)]
	private float focusRadius = 1f;
	[SerializeField, Range(0f, 1f)]
	private float focusCentering = 0.5f;

	private Vector3 focusPoint;

	private void Awake()
	{
		focusPoint = focus.position;
	}
	
	private void LateUpdate()
	{
		UpdateFocusPoint();
		Vector3 lookDirection = transform.forward;
		transform.localPosition = focusPoint - lookDirection * distance;
	}

	private void UpdateFocusPoint()
	{
		Vector3 targetPoint = focus.position;
		if (focusRadius > 0f)
		{
			float distance = Vector3.Distance(targetPoint, focusPoint);
			if (distance > focusRadius)
			{
				focusPoint = Vector3.Lerp(
					targetPoint, focusPoint, focusRadius / distance
				);
			}
			if (distance > 0.01f && focusCentering > 0f)
			{
				focusPoint = Vector3.Lerp(
					targetPoint, focusPoint,
					Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime)
				);
			}
		}
		else
		{
			focusPoint = targetPoint;
		}
	}
}
