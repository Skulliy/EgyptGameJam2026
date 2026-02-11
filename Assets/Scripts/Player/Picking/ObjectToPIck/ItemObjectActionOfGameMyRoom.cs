using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class ItemObjectActionOfGameMyRoom : MonoBehaviour
{
	[Header("Item Settings")]
	[Tooltip("The specific voice line to play when this item is picked up.")]
	public AudioClip voicelineToPlayOnPickUp;

	private PickableObject myPickableObject;

	void Start()
	{
		// Get the PickableObject component from the same GameObject
		myPickableObject = GetComponent<PickableObject>();
	}

	/// <summary>
	/// Connect this method to the 'On Picked Up' event in the Inspector.
	/// </summary>
	public void ActionOnInteraction()
	{
		// Send the data to the GameManager Singleton
		if (GameManager.Instance != null)
		{
			GameManager.Instance.ItemSelected(voicelineToPlayOnPickUp, myPickableObject);
		}
		else
		{
			Debug.LogError("GameManager is missing from the scene!");
		}
	}
}