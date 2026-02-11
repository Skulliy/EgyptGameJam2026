using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class ItemObjectActionOfGameMyRoom : MonoBehaviour
{
	[Header("Item Settings")]

	[Range(0f, 1f)]
	public float percentage; // 0 = White, 1 = Light Green


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
		//change color
		UpdateColor();


		// Send the data to the GameManager Singleton
		if (GameManager.Instance != null)
		{
			GameManager.Instance.ItemSelected(null, myPickableObject);
		}
		else
		{
			Debug.LogError("GameManager is missing from the scene!");
		}
	}


	

	void UpdateColor()
	{
		// Define our target colors
		Color colorWhite = Color.white;
		Color colorGreen = Color.green;

		// Lerp calculates the color in between based on the percentage
		// If percentage is 0.5, it returns a pale green.
		Color lerpedColor = Color.Lerp(colorWhite, colorGreen, percentage);

		// Apply it to the material's main color property
		this.GetComponent<Renderer>().material.color = lerpedColor;
	}
}