﻿using UnityEngine;

// implementing a tutorial series from https://catlikecoding.com/unity/tutorials/movement/sliding-a-sphere/
public class MovingSphere : MonoBehaviour
{
	[SerializeField, Range(0f, 100f)]
	private float maxSpeed = 10f;
	[SerializeField, Range(0f, 100f)]
	private float maxAcceleration = 10f;
	[SerializeField]
	private Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);
	[SerializeField, Range(0f, 1f)]
	private float bounciness = 0.5f;
	private Vector3 velocity = new Vector3();

	void Start()
    {
        
    }

    private void Update()
    {
		Vector2 playerInput;
		playerInput.x = Input.GetAxis("Horizontal");
		playerInput.y = Input.GetAxis("Vertical");
		Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
		float maxSpeedChange = maxAcceleration * Time.deltaTime;
		velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
		velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
		Vector3 displacement = velocity * Time.deltaTime;
		Vector3 newPosition = transform.localPosition + displacement;
		if (newPosition.x < allowedArea.xMin)
		{
			newPosition.x = allowedArea.xMin;
			velocity.x = -velocity.x * bounciness;
		}
		else if (newPosition.x > allowedArea.xMax)
		{
			newPosition.x = allowedArea.xMax;
			velocity.x = -velocity.x * bounciness;
		}
		if (newPosition.z < allowedArea.yMin)
		{
			newPosition.z = allowedArea.yMin;
			velocity.z = -velocity.z * bounciness;
		}
		else if (newPosition.z > allowedArea.yMax)
		{
			newPosition.z = allowedArea.yMax;
			velocity.z = -velocity.z * bounciness;
		}
		transform.localPosition = newPosition;
    }
}
