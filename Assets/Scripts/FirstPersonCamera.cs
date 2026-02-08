using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
	[Header("Camera Settings")]
	[Tooltip("Controls how fast the camera rotates with the mouse.")]
	public float mouseSensitivity = 100f;

	[Header("Vertical Limits")]
	[Tooltip("The highest angle the player can look up (negative value).")]
	public float minVerticalAngle = -90f;

	[Tooltip("The lowest angle the player can look down (positive value).")]
	public float maxVerticalAngle = 90f;

	//[Header("References")]
	//[Tooltip("Drag the Player object (the parent capsule) here.")]
	public Transform playerBody;

	private float xRotation = 0f;
	

	void Start()
	{
		// Locks the cursor to the center of the screen and hides it
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		// Get mouse input using the default Input Manager
		// Time.deltaTime ensures rotation is frame-rate independent
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

		// Calculate vertical rotation (looking up and down)
		xRotation -= mouseY;
		

		// Clamp the rotation so the camera doesn't flip or get stuck
		// This restricts the view between the min and max angles defined above
		xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

		// Apply the vertical rotation to the Camera (local X axis)
		transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

		// Rotate the Player Body horizontally (Global Y axis) based on Mouse X
		// We rotate the body so the movement controls align with where we are looking
		playerBody.Rotate(Vector3.up * mouseX);



	}
}