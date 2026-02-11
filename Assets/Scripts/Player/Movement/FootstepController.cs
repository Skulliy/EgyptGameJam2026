using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class FootstepController : MonoBehaviour
{
	public  CharacterController controller;

	private AudioSource audioSource;
	private Vector3 lastPosition;

	[Header("Movement Settings")]
	[Tooltip("How much the player must move horizontally to count as 'moving'.")]
	public float movementThreshold = 0.01f;

	[Header("State (Read Only)")]
	public bool isMoving;

	void Start()
	{
		
		audioSource = GetComponent<AudioSource>();

		// Initialize lastPosition so we don't get a huge spike on frame 1
		lastPosition = transform.position;

		// Ensure the audio source is set up for looping
		audioSource.loop = true;
		audioSource.playOnAwake = false;
	}

	void Update()
	{
		CheckMovement();
		HandleAudio();
	}

	void CheckMovement()
	{
		// 1. Calculate horizontal distance only (ignore Y)
		Vector3 currentPos = transform.position;
		Vector3 lastPosHorizontal = new Vector3(lastPosition.x, 0, lastPosition.z);
		Vector3 currentPosHorizontal = new Vector3(currentPos.x, 0, currentPos.z);

		float distanceMoved = Vector3.Distance(lastPosHorizontal, currentPosHorizontal);
		Debug.Log(" distanceMoved is " + distanceMoved);
		// 2. Check if moved enough AND is grounded
		// We use a threshold because floating point math is never exactly zero
		if (distanceMoved > movementThreshold && controller.isGrounded)
		{
			isMoving = true;
		}
		else
		{
			isMoving = false;
		}

		// 3. Store current position for the next frame's comparison
		lastPosition = currentPos;
	}

	void HandleAudio()
	{
		if (isMoving)
		{
			// If we are moving but the audio isn't playing, start it
			if (!audioSource.isPlaying)
			{
				audioSource.Play();
			}
		}
		else
		{
			// If we stop moving, stop the audio
			if (audioSource.isPlaying)
			{
				audioSource.Stop();
			}
		}
	}
}