using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
	[Header("Movement Settings")]
	[Tooltip("How fast the player moves in units per second.")]
	public float moveSpeed = 6.0f;

	// Reference to the CharacterController component
	private CharacterController controller;

	void Start()
	{
		// Automatically find the CharacterController attached to this object
		controller = GetComponent<CharacterController>();
	}

	void Update()
	{
		// 1. Get Input (Returns -1 to 1 based on WASD or Arrow Keys)
		float x = Input.GetAxis("Horizontal"); // A / D
		float z = Input.GetAxis("Vertical");   // W / S

		// 2. Calculate the direction relative to where the player is facing
		// transform.right is the local Red axis (Left/Right)
		// transform.forward is the local Blue axis (Forward/Back)
		Vector3 move = transform.right * x + transform.forward * z;

		// 3. Normalize the vector
		// Without this, moving diagonally (W + D) would make you move faster (~1.41x speed)
		// We check magnitude > 1 so we don't force slow analog stick movements to be full speed
		if (move.magnitude > 1f)
		{
			move.Normalize();
		}

		// 4. Move the Controller
		controller.Move(move * moveSpeed * Time.deltaTime);
	}
}