using UnityEngine;
using UnityEngine.Events;

public class ObjectSelectionTrigger : MonoBehaviour
{
	[Header("Input Settings")]
	[Tooltip("Select which key the player must press to pick up the object.")]
	public KeyCode interactionKey = KeyCode.E;

	[Header("Output Events")]
	[Tooltip("This fires when the player successfully clicks the button on a valid object.")]
	public UnityEvent<PickableObject> OnObjectSelected;

	// Internal state variables
	private bool isListening = false;
	public  PickableObject currentTarget = null;

	void Update()
	{
		// Only run this check if we are currently looking at a valid object
		if (isListening && currentTarget != null)
		{
			// Check for the button press (Legacy Input)
			if (Input.GetKeyDown(interactionKey))
			{
				// Fire the event!
				// This broadcasts the specific object we just clicked on.
				OnObjectSelected.Invoke(currentTarget);
			}
		}
	}

	/// <summary>
	/// Call this when a valid object enters the view.
	/// Connect this to the 'On Object Enter' event in your ObjectSelector.
	/// </summary>
	public void StartListening(PickableObject obj)
	{
		currentTarget = obj;
		isListening = true;
	}

	/// <summary>
	/// Call this when the object leaves the view.
	/// Connect this to the 'On Object Exit' event in your ObjectSelector.
	/// </summary>
	public void StopListening(PickableObject obj)
	{
		// We accept the 'obj' parameter just to keep it compatible 
		// with the UnityEvent signature, but we mainly just need to stop listening.
		isListening = false;
		currentTarget = null;
	}
}