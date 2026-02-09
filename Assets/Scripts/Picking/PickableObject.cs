using UnityEngine;

// Define the types of objects we have
public enum ObjectType
{
	NormalObject,
	SpecialObject
}

public class PickableObject : MonoBehaviour
{
	[Header("Object Settings")]
	[Tooltip("Is this a Normal or Special object?")]
	public ObjectType objectType = ObjectType.NormalObject;

	public bool interactable = false;
    public string tooltipText = "Press E";
}