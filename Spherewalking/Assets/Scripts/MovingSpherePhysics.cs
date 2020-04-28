using UnityEngine;

// implementing a tutorial series from
// https://catlikecoding.com/unity/tutorials/movement/sliding-a-sphere/
// https://catlikecoding.com/unity/tutorials/movement/physics/
public class MovingSpherePhysics : MonoBehaviour
{
	[SerializeField, Range(0f, 100f)]
	private float maxSpeed = 10f;
	[SerializeField, Range(0f, 100f)]
	private float maxAcceleration = 10f, maxAirAcceleration = 1f;
	[SerializeField, Range(0f, 10f)]
	private float jumpHeight = 2f;
	[SerializeField, Range(0, 5)]
	int maxAirJumps = 0;
	private Vector3 velocity = new Vector3();
	private Vector3 desiredVelocity = new Vector3();
	private Rigidbody body;
	private bool desiredJump;
	private bool onGround;
	private int jumpPhase;

	private void Awake()
	{
		body = GetComponent<Rigidbody>();
	}

	private void Start()
    {
        
    }

    private void Update()
    {
		Vector2 playerInput;
		playerInput.x = Input.GetAxis("Horizontal");
		playerInput.y = Input.GetAxis("Vertical");
		desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
		desiredJump |= Input.GetButtonDown("Jump");
    }

	private void FixedUpdate()
	{
		UpdateState();
		float acceleration = onGround ? maxAcceleration : maxAirAcceleration;
		float maxSpeedChange = acceleration * Time.deltaTime;
		velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
		velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
		if (desiredJump)
		{
			desiredJump = false;
			Jump();
		}
		body.velocity = velocity;
		onGround = false;
	}

	private void UpdateState()
	{
		velocity = body.velocity;
		if (onGround)
		{
			jumpPhase = 0;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		EvaluateCollision(collision);
	}

	private void OnCollisionStay(Collision collision)
	{
		EvaluateCollision(collision);
	}

	private void EvaluateCollision(Collision collision)
	{
		for (int i = 0; i < collision.contactCount; i++)
		{
			Vector3 normal = collision.GetContact(i).normal;
			onGround |= normal.y >= 0.9f;
		}
	}

	private void Jump()
	{
		if (onGround || jumpPhase < maxAirJumps)
		{
			jumpPhase += 1;
			float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
			if (velocity.y > 0f)
			{
				// make sure we don't slow down if we already jump faster then the jumpspeed
				jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
			}
			velocity.y += jumpSpeed;
		}
	}
}
