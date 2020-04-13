using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegStepper : MonoBehaviour
{
	[SerializeField] Transform homeTransform;
	[SerializeField] float wantStepAtDistance;
	[SerializeField] float moveDuration;
	private bool isMoving;

	private void Update()
	{
		if (isMoving)
		{
			return;
		}

		float distFromHome = Vector3.Distance(transform.position, homeTransform.position);

		if (distFromHome > wantStepAtDistance)
		{
			StartCoroutine(MoveToHome());
		}
	}

	IEnumerator MoveToHome()
	{
		isMoving = true;

		Quaternion startRot = transform.rotation;
		Vector3 startPoint = transform.position;
		Quaternion endRot = homeTransform.rotation;
		Vector3 endPoint = homeTransform.position;
		float timeElapsed = 0f;

		do
		{
			timeElapsed += Time.deltaTime;
			float normalizedTime = timeElapsed / moveDuration;

			transform.position = Vector3.Lerp(startPoint, endPoint, normalizedTime);
			transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

			yield return null;
		}
		while (timeElapsed < moveDuration);

		isMoving = false;
	}
}
