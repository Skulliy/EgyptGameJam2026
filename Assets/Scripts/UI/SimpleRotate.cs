using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
	[Tooltip("Speed of rotation in degrees per second")]
	public float rotationSpeed = 50f;

	// Update is called once per frame
	void Update()
	{
		// Rotates the object around the Y axis
		// We multiply by deltaTime to make it frame-rate independent
		transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
	}
}