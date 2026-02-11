using UnityEngine;
using UnityEngine.Events;

public class ObjectSelector : MonoBehaviour
{
	public enum SelectionMode
	{
		LookingForNormal,
		LookingForSpecial,
		LookingForBoth,
		DoorOnly // New state added here
	}

	[Header("Raycast Settings")]
	public Transform rayOrigin;
	public float maxDistance = 3.0f;
	public SelectionMode currentSelectionMode = SelectionMode.LookingForBoth;

	[Header("Debug Info")]
	[SerializeField] private PickableObject currentlyHitObject;

	[Header("Events")]
	public UnityEvent<PickableObject> OnObjectEnter;
	public UnityEvent<PickableObject> OnObjectExit;

	void Update() => HandleRaycast();

	void HandleRaycast()
	{
		RaycastHit hit;
		if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, maxDistance))
		{
			PickableObject newPickable = hit.collider.GetComponent<PickableObject>();

			if (newPickable != null && IsTypeMatch(newPickable))
			{
				if (currentlyHitObject != newPickable)
				{
					if (currentlyHitObject != null) OnObjectExit.Invoke(currentlyHitObject);
					currentlyHitObject = newPickable;
					OnObjectEnter.Invoke(currentlyHitObject);
				}
			}
			else
			{
				ClearCurrentObject();
			}
		}
		else
		{
			ClearCurrentObject();
		}
	}

	void ClearCurrentObject()
	{
		if (currentlyHitObject != null)
		{
			OnObjectExit.Invoke(currentlyHitObject);
			currentlyHitObject = null;
		}
	}

	// UPDATED LOGIC
	bool IsTypeMatch(PickableObject obj)
	{
		// 1. If it's an Interactable Object, it ALWAYS matches (unless it's null)
		/*if (obj.objectType == ObjectType.InteractableObject)
			return true;*/

		// 2. If we are ONLY looking for interactables, and we got here, it's NOT one.
		if (currentSelectionMode == SelectionMode.DoorOnly)
		{
			if (obj.objectType == ObjectType.Door)
			{
				return true;
			}
			return false;
		}
		if (obj.objectType == ObjectType.Door)
		{
			return false;
		}


			// 3. Standard filtering for everything else
			if (currentSelectionMode == SelectionMode.LookingForBoth) return true;

		if (currentSelectionMode == SelectionMode.LookingForNormal &&
			obj.objectType == ObjectType.NormalObject) return true;

		if (currentSelectionMode == SelectionMode.LookingForSpecial &&
			obj.objectType == ObjectType.SpecialObject) return true;

		return false;
	}
}