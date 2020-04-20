using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegStepper : MonoBehaviour
{
	[SerializeField] Transform homeTransform;
	[SerializeField] float wantStepAtDistance;
	[SerializeField] float moveDuration;
	[SerializeField] float stepOvershootFraction;

	private bool isMoving;

	public bool IsMoving { get => isMoving; }

	public void TryMove()
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
		//Vector3 endPoint = homeTransform.position;
		float timeElapsed = 0f;

		Vector3 towardHome = (homeTransform.position - transform.position);
		float overshootDistance = wantStepAtDistance * stepOvershootFraction;
		Vector3 overshootVector = towardHome * overshootDistance;
		overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);
		Vector3 endPoint = homeTransform.position + overshootVector;
		Vector3 centerPoint = (startPoint + endPoint) / 2;
		centerPoint += homeTransform.up * Vector3.Distance(startPoint, endPoint) / 2f;

		do
		{
			timeElapsed += Time.deltaTime;
			float normalizedTime = timeElapsed / moveDuration;
			normalizedTime = Easing.Cubic.InOut(normalizedTime);

			transform.position = 
				Vector3.Lerp(
					Vector3.Lerp(startPoint, centerPoint, normalizedTime),
					Vector3.Lerp(centerPoint, endPoint, normalizedTime),
					normalizedTime
					);
			transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

			yield return null;
		}
		while (timeElapsed < moveDuration);

		isMoving = false;
	}
}
