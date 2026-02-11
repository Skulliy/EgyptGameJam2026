using UnityEngine;
using UnityEngine.Events; // Required for UnityEvents

public enum ObjectType
{
	NormalObject,
	SpecialObject,
	InteractableObject // New state added here
}

public class PickableObject : MonoBehaviour
{
	[Header("Object Settings")]
	public ObjectType objectType = ObjectType.NormalObject;

	[Header("Interactions")]
	[Tooltip("This event fires ONLY for this specific object when it is picked up.")]
	public UnityEvent OnPickedUp;
}