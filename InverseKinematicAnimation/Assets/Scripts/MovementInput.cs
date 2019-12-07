//Copyright Filmstorm (C) 2018 - Movement Controller for Root Motion and built in IK solver
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour
{
	#region Variables
	private float InputX; //Left and Right Inputs
	private float InputZ; //Forward and Back Inputs
	private Vector3 desiredMoveDirection; //Vector that holds desired Move Direction
	private bool blockRotationPlayer = false; //Block the rotation of the player?
	[Range(0, 0.5f)]
	[SerializeField] private float desiredRotationSpeed = 0.1f; //Speed of the players rotation
	[SerializeField] private Animator anim; //Animator
	private float Speed; //Speed player is moving
	[Range(0, 1f)]
	[SerializeField] private float allowPlayerRotation = 0.1f; //Allow player rotation from inputs once past x
	[SerializeField] private Camera cam; //Main camera (make sure tag is MainCamera)
	[SerializeField] private CharacterController controller; //Character Controller, auto added on script addition
	private bool isGrounded; //is Grounded - work in progress

	[Header("Feet Grounder")]
	private Vector3 rightFootPosition, leftFootPosition, leftFootIKPosition, rightFootIKPosition;
	private Quaternion leftFootIKRotation, rightFootIKRotation;
	private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;
	[SerializeField] private bool enableFeetIK = true;
	[Range(0, 2)]
	[SerializeField] private float heightFromGroundRaycast = 1.14f;
	[Range(0, 2)]
	[SerializeField] private float raycastDownDistance = 1.5f;
	[SerializeField] private LayerMask environmentLayer;
	[SerializeField] private float pelvisOffset = 0f;
	[Range(0, 1)]
	[SerializeField] private float pelvisUpAndDownSpeed = 0.28f;
	[Range(0, 1)]
	[SerializeField] private float feetToIKPositionSpeed = 0.5f;
	[SerializeField] private string leftFootAnimVariableName = "LeftFootCurve";
	[SerializeField] private string rightFootAnimVariableName = "RightFootCurve";
	[SerializeField] private bool useProIKFeature = false;
	[SerializeField] private bool showSolverDebug = true;

	[Header("Animation Smoothing")]
	[Range(0, 1f)]
	[SerializeField] private float HorizontalAnimSmoothTime = 0.2f; //InputX dampening
	[Range(0, 1f)]
	[SerializeField] private float VerticalAnimTime = 0.2f; //InputZ dampening
	[Range(0, 1f)]
	[SerializeField] private float StartAnimTime = 0.3f; //dampens the time of starting the player after input is pressed
	[Range(0, 1f)]
	[SerializeField] private float StopAnimTime = 0.15f; //dampens the time of stopping the player after release of input
	#endregion

	private float verticalVel; //Vertical velocity -- currently work in progress
	private Vector3 moveVector; //movement vector -- currently work in progress
	
	// Update is called once per frame
	private void Update()
	{
		InputMagnitude();

	}

	/// <summary>
	/// updating the AdjustFeetTarget method, find position of each foot inside our Solver Position
	/// </summary>
	private void FixedUpdate()
	{
		if (!enableFeetIK)
		{
			return;
		}

		if (anim == null)
		{
			return;
		}

		AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
		AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

		FeetPositionSolver(rightFootPosition, ref rightFootIKPosition, ref rightFootIKRotation);
		FeetPositionSolver(leftFootPosition, ref leftFootIKPosition, ref leftFootIKRotation);
	}

	#region PlayerMovement
	private void PlayerMoveAndRotation()
	{
		UpdateInputValues();

		var camera = Camera.main;
		var forward = cam.transform.forward;
		var right = cam.transform.right;

		forward.y = 0f;
		right.y = 0f;

		forward.Normalize();
		right.Normalize();

		desiredMoveDirection = forward * InputZ + right * InputX;

		if (blockRotationPlayer == false)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
		}
	}

	private void InputMagnitude()
	{
		//Calculate Input Vectors
		UpdateInputValues();

		anim.SetFloat("InputZ", InputZ, VerticalAnimTime, Time.deltaTime * 2f);
		anim.SetFloat("InputX", InputX, HorizontalAnimSmoothTime, Time.deltaTime * 2f);

		//Calculate the Input Magnitude
		Speed = new Vector2(InputX, InputZ).sqrMagnitude;

		//Physically move player
		if (Speed > allowPlayerRotation)
		{
			anim.SetFloat("InputMagnitude", Speed, StartAnimTime, Time.deltaTime);
			PlayerMoveAndRotation();
		}
		else if (Speed < allowPlayerRotation)
		{
			anim.SetFloat("InputMagnitude", Speed, StopAnimTime, Time.deltaTime);
		}
	}

	private void UpdateInputValues()
	{
		InputX = Input.GetAxis("Horizontal");
		InputZ = Input.GetAxis("Vertical");
	}

	#endregion

	#region FeetGrounding
	

	private void OnAnimatorIK(int layerIndex)
	{
		if (!enableFeetIK)
		{
			return;
		}
		if (anim == null)
		{
			return;
		}

		MovePelvisHeight();

		anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
		anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

		if (useProIKFeature)
		{
			anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat(rightFootAnimVariableName));
			anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat(leftFootAnimVariableName));
		}

		MoveFeetToIKPoint(
			AvatarIKGoal.RightFoot, 
			rightFootIKPosition, 
			rightFootIKRotation, 
			ref lastRightFootPositionY);
		MoveFeetToIKPoint(
			AvatarIKGoal.LeftFoot, 
			leftFootIKPosition, 
			leftFootIKRotation, 
			ref lastLeftFootPositionY);
	}
	#endregion

	#region FeetGroundingMethods

	private void MoveFeetToIKPoint(
		AvatarIKGoal foot, 
		Vector3 positionIKHolder, 
		Quaternion rotationIKHolder,
		ref float lastFootPositionY)
	{
		Vector3 targetIKPosition = anim.GetIKPosition(foot);

		if (positionIKHolder != Vector3.zero)
		{
			targetIKPosition = transform.InverseTransformPoint(targetIKPosition);
			positionIKHolder = transform.InverseTransformPoint(positionIKHolder);

			float yVariable = Mathf.Lerp(lastFootPositionY, positionIKHolder.y, feetToIKPositionSpeed);
			targetIKPosition.y += yVariable;

			lastFootPositionY = yVariable;
			targetIKPosition = transform.TransformPoint(targetIKPosition);

			anim.SetIKRotation(foot, rotationIKHolder);
		}

		anim.SetIKPosition(foot, targetIKPosition);
	}

	private void MovePelvisHeight()
	{
		if (
			rightFootIKPosition == Vector3.zero 
			|| leftFootIKPosition == Vector3.zero 
			|| lastPelvisPositionY == 0)
		{
			lastPelvisPositionY = anim.bodyPosition.y;
			return;
		}

		float leftOffsetPosition = leftFootIKPosition.y - transform.position.y;
		float rightOffsetPosition = rightFootIKPosition.y - transform.position.y;
		float totalOffset = (leftOffsetPosition < rightOffsetPosition) ? leftOffsetPosition : rightOffsetPosition;

		Vector3 newPelvisPosition = anim.bodyPosition + Vector3.up * totalOffset;
		newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisUpAndDownSpeed);
		anim.bodyPosition = newPelvisPosition;
		lastPelvisPositionY = anim.bodyPosition.y;
	}

	private void FeetPositionSolver(
		Vector3 fromSkyPosition,
		ref Vector3 feetIKPositions,
		ref Quaternion feetIKRotations)
	{
		RaycastHit feetOutHit;

		if (showSolverDebug)
		{
			Debug.DrawLine(
				fromSkyPosition,
				fromSkyPosition + Vector3.down * (raycastDownDistance + heightFromGroundRaycast),
				Color.yellow);
		}

		if (Physics.Raycast(
			fromSkyPosition, 
			Vector3.down,
			out feetOutHit, 
			raycastDownDistance + heightFromGroundRaycast, environmentLayer))
		{
			feetIKPositions = fromSkyPosition;
			feetIKPositions.y = feetOutHit.point.y + pelvisOffset;
			feetIKRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

			return;
		}

		feetIKPositions = Vector3.zero; // it didn't work..
	}

	private void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
	{
		feetPositions = anim.GetBoneTransform(foot).position;
		feetPositions.y = transform.position.y + heightFromGroundRaycast;
	}
	#endregion

	#region Initialization

	// Initialization of variables
	private void Start()
	{
		anim = this.GetComponent<Animator>();
		cam = Camera.main;
		controller = this.GetComponent<CharacterController>();

		if (anim == null)
			Debug.LogError("We require " + transform.name + " game object to have an animator. This will allow for Foot IK to function");
	}
	#endregion
}





