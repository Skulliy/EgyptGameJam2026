using UnityEngine;
using UnityEngine.Events; // Required for UnityEvents

public class ObjectSelector : MonoBehaviour
{
	// Define the modes for our raycast
	public enum SelectionMode
	{
		LookingForNormal,
		LookingForSpecial,
		LookingForBoth
	}

	[Header("Raycast Settings")]
	[Tooltip("The point from which the ray is fired (usually a child of the Camera).")]
	public Transform rayOrigin;

	[Tooltip("How far the player can reach.")]
	public float maxDistance = 3.0f;

	[Tooltip("What kind of objects are we currently interested in?")]
	public SelectionMode currentSelectionMode = SelectionMode.LookingForBoth;

	[Header("Debug Info")]
	[Tooltip("The object currently being looked at (Read Only).")]
	[SerializeField] private PickableObject currentlyHitObject;

	[Header("Events")]
	// These events allow you to drag-and-drop actions in the Inspector
	public UnityEvent<PickableObject> OnObjectEnter;
	public UnityEvent<PickableObject> OnObjectExit;

	void Update()
	{
		HandleRaycast();
	}

	void HandleRaycast()
	{
		RaycastHit hit;

		// Fire the ray forward from the origin
		if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, maxDistance))
		{
			// Try to get the PickableObject script from the thing we hit
			PickableObject newPickable = hit.collider.GetComponent<PickableObject>();

			// VALIDATION:
			// 1. Is it actually a pickable object?
			// 2. Does it match the type we are looking for?
			if (newPickable != null && IsTypeMatch(newPickable))
			{
				// Logic: We are looking at a VALID object
				if (currentlyHitObject != newPickable)
				{
					// If we were looking at a DIFFERENT object before, deselect the old one
					if (currentlyHitObject != null)
					{
						OnObjectExit.Invoke(currentlyHitObject);
					}

					// Register the new object
					currentlyHitObject = newPickable;
					OnObjectEnter.Invoke(currentlyHitObject);
				}
			}
			else
			{
				// We hit something (like a wall), but it's not a valid PickableObject
				ClearCurrentObject();
			}
		}
		else
		{
			// We hit nothing (looking at the sky/air)
			ClearCurrentObject();
		}
	}

	// Helper function to handle the "Exited" logic
	void ClearCurrentObject()
	{
		if (currentlyHitObject != null)
		{
			OnObjectExit.Invoke(currentlyHitObject);
			currentlyHitObject = null;
		}
	}

	// Checks if the object matches our current Selection Mode
	bool IsTypeMatch(PickableObject obj)
	{
		if (currentSelectionMode == SelectionMode.LookingForBoth) return true;

		if (currentSelectionMode == SelectionMode.LookingForNormal &&
			obj.objectType == ObjectType.NormalObject) return true;

		if (currentSelectionMode == SelectionMode.LookingForSpecial &&
			obj.objectType == ObjectType.SpecialObject) return true;

		return false;
	}
}