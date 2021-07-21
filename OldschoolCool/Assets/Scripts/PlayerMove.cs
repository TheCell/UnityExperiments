using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    const float jumpCheckPreventionTime = 0.5f;

    [Header("Physic Setting")]
    [SerializeField] private LayerMask groundLayerMask = new LayerMask();

    [Header("Move & Jump Setting")]
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float fallWeight = 5.0f;
    [SerializeField] private float jumpWeight = 1.0f;
    [SerializeField] private float jumpVelocity = 10.0f;

    private bool isJumping = false;
    private float jumpTimestamp;
    private Vector3 moveVec = Vector3.zero;

    private Animator animator;
    private Rigidbody rigidbody;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rigidbody = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		UpdateAnimation();
	}

	private void FixedUpdate()
	{
		if (moveVec != Vector3.zero)
		{
			transform.LookAt(this.transform.position + moveVec.normalized);
		}

		if (isJumping == false)
		{
			UpdateWhenGrounded();
		}
		else
		{
			UpdateWhenJumping();
		}
	}

	void UpdateWhenJumping()
	{
		bool isFalling = rigidbody.velocity.y <= 0;

		float weight = isFalling ? fallWeight : jumpWeight;

		rigidbody.velocity = new Vector3(moveVec.x * moveSpeed, rigidbody.velocity.y, moveVec.z * moveSpeed);
		rigidbody.velocity += Vector3.up * Physics.gravity.y * weight * Time.deltaTime;

		GroundCheck();
	}

	private void UpdateWhenGrounded()
	{
		rigidbody.velocity = moveVec * moveSpeed;

		CheckShouldFall();
	}

	public void OnJump()
	{
		HandleJump();
	}

	public void OnMove(InputValue input)
	{
		Vector2 inputVec = input.Get<Vector2>();
		Debug.Log(inputVec);

		moveVec = new Vector3(inputVec.x, 0, inputVec.y);
	}

	#region Jump & Fall & Ground Logic

	protected bool HandleJump()
	{
		if (isJumping)
		{
			return false;
		}

		isJumping = true;
		jumpTimestamp = Time.time;
		rigidbody.velocity = new Vector3(0, jumpVelocity, 0); // Set initial jump velocity

		return true;
	}

	void CheckShouldFall()
	{
		if (isJumping)
		{
			return; // No need to check if in the air
		}

		bool hasHit = Physics.CheckSphere(transform.position, 0.1f, groundLayerMask);

		if (hasHit == false)
		{
			isJumping = true;
		}
	}

	private void GroundCheck()
	{
		if (isJumping == false)
		{
			return; // No need to check
		}

		if (Time.time < jumpTimestamp + jumpCheckPreventionTime)
		{
			return;
		}

		bool hasHit = Physics.CheckSphere(transform.position, 0.1f, groundLayerMask);

		if (hasHit)
		{
			isJumping = false;
		}
	}

	#endregion

	private void UpdateAnimation()
	{
		if (animator == null)
		{
			return;
		}

		animator.SetBool("jumping", isJumping);
		animator.SetFloat("moveSpeed", moveVec.magnitude);
	}
}
