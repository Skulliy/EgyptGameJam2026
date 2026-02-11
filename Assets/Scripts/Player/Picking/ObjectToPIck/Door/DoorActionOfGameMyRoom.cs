using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class DoorActionOfGameMyRoom : MonoBehaviour
{
	/// <summary>
	/// Connect this method to the 'On Picked Up' event of the 
	/// PickableObject component on your Door.
	/// </summary>
	public void ActionOnInteraction()
	{
		if (GameManager.Instance != null)
		{
			Debug.Log("Attempting to open door...");
			GameManager.Instance.EndOfLevelCheck();
		}
		else
		{
			Debug.LogError("No GameManager found in the scene!");
		}
	}
}