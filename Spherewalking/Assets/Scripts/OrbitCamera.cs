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
	[SerializeField, Range(1f, 360f)]
	private float rotationSpeed = 90f;
	[SerializeField, Range(-89f, 89f)]
	private float minVerticalAngle = -30f, maxVerticalAngle = 60f;
	private Vector2 orbitAngles = new Vector2(45f, 0f);

	private Vector3 focusPoint;

	private void OnValidate()
	{
		if (maxVerticalAngle < minVerticalAngle)
		{
			maxVerticalAngle = minVerticalAngle;
		}
	}

	private void Awake()
	{
		focusPoint = focus.position;
		transform.localRotation = Quaternion.Euler(orbitAngles);
	}
	
	private void LateUpdate()
	{
		UpdateFocusPoint();
		Quaternion lookRotation;
		if (ManualRotation())
		{
			ConstrainAngles();
			lookRotation = Quaternion.Euler(orbitAngles);
		}
		else
		{
			lookRotation = transform.localRotation;
		}
		Vector3 lookDirection = lookRotation * Vector3.forward;
		Vector3 lookPosition = focusPoint - lookDirection * distance;
		transform.SetPositionAndRotation(lookPosition, lookRotation);
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

	private bool ManualRotation ()
	{
		Vector2 input = new Vector2(
			Input.GetAxis("Vertical Camera"),
			Input.GetAxis("Horizontal Camera")
			);
		const float e = 0.001f;
		if (input.x < -e || input.x > e || input.y < -e || input.y > e)
		{
			orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
			return true;
		}

		return false;
	}

	private void ConstrainAngles()
	{
		orbitAngles.x = Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

		if (orbitAngles.y < 0f)
		{
			orbitAngles.y += 360f;
		}
		else if (orbitAngles.y > 360f)
		{
			orbitAngles.y -= 360f;
		}
	}
}
