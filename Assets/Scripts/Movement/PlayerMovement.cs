using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
	[Header("Movement Settings")]
	[Tooltip("How fast the player moves in units per second.")]
	public float moveSpeed = 4f;

	[Header("Gravity Settings")]
	[Tooltip("The force pulling the player down (Standard Earth gravity is -9.81).")]
	public float gravity = -9.81f;

	[Tooltip("A small constant force to keep the player stuck to the ground.")]
	public float groundedGravity = -2.0f;

	// Reference to the CharacterController
	private CharacterController controller;

	// This stores our vertical speed (falling/jumping)
	private Vector3 velocity;

	void Start()
	{
		controller = GetComponent<CharacterController>();
	}

	void Update()
	{
		// ---------------------------------------------------------
		// 1. HORIZONTAL MOVEMENT (X and Z)
		// ---------------------------------------------------------

		// Get Input (WASD / Arrows)
		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");

		// Calculate direction relative to where we are facing
		Vector3 move = transform.right * x + transform.forward * z;

		// Normalize to prevent faster diagonal movement
		if (move.magnitude > 1f) move.Normalize();

		// Move the controller horizontally
		controller.Move(move * moveSpeed * Time.deltaTime);


		// ---------------------------------------------------------
		// 2. GRAVITY LOGIC (Y Axis)
		// ---------------------------------------------------------

		// Check if the controller is touching the ground
		if (controller.isGrounded && velocity.y < 0)
		{
			// We don't set this to 0 because the isGrounded check can be flickery.
			// -2 ensures we stay "snapped" firmly to the floor.
			velocity.y = groundedGravity;
		}

		// Apply Gravity to the velocity (v = a * t)
		velocity.y += gravity * Time.deltaTime;

		// Apply the vertical velocity to the controller (d = v * t)
		// Note: We multiply by deltaTime AGAIN because physics formulas are (1/2 * g * t^2)
		controller.Move(velocity * Time.deltaTime);
	}

}