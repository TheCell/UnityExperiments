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
	[SerializeField, Range(0, 90)]
	private float maxGroundAngle = 25f;
	private Vector3 velocity = new Vector3();
	private Vector3 desiredVelocity = new Vector3();
	private Rigidbody body;
	private bool desiredJump;
	//private bool onGround;
	private int jumpPhase;
	private float minGroundDotProduct;
	Vector3 contactNormal;
	private int groundContactCount;
	private bool OnGround => groundContactCount > 0;

	private void Awake()
	{
		body = GetComponent<Rigidbody>();
		OnValidate();
	}

	private void OnValidate()
	{
		minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
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
		AdjustVelocity();
		//float acceleration = onGround ? maxAcceleration : maxAirAcceleration;
		//float maxSpeedChange = acceleration * Time.deltaTime;
		//velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
		//velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

		if (desiredJump)
		{
			desiredJump = false;
			Jump();
		}
		body.velocity = velocity;
		//onGround = false;
		ClearState();
	}

	private void ClearState()
	{
		groundContactCount = 0;
		contactNormal = Vector3.zero;
	}

	private void UpdateState()
	{
		velocity = body.velocity;
		if (OnGround)
		{
			jumpPhase = 0;
			if (groundContactCount > 1)
			{
				contactNormal.Normalize();
			}
		}
		else
		{
			contactNormal = Vector3.up;
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
			if (normal.y >= minGroundDotProduct)
			{
				groundContactCount += 1;
				contactNormal += normal;
			}
		}
	}

	private void Jump()
	{
		if (OnGround || jumpPhase < maxAirJumps)
		{
			jumpPhase += 1;
			float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
			float alignedSpeed = Vector3.Dot(velocity, contactNormal);
			if (velocity.y > 0f)
			{
				// make sure we don't slow down if we already jump faster then the jumpspeed
				jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
			}
			velocity += contactNormal * jumpSpeed;
		}
	}

	private Vector3 ProjectOnContactPlane (Vector3 vector)
	{
		return vector - contactNormal * Vector3.Dot(vector, contactNormal);
	}

	private void AdjustVelocity ()
	{
		Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
		Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

		float currentX = Vector3.Dot(velocity, xAxis);
		float currentZ = Vector3.Dot(velocity, zAxis);

		float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
		float maxSpeedChange = acceleration * Time.deltaTime;

		float newX = 
			Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
		float newZ =
			Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

		velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
	}
}
