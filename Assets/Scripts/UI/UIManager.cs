using UnityEngine;
using UnityEngine.UI; // Required for the Image component
using TMPro;          // Required for TextMeshPro and RTLTMPro

public class UIManager : MonoBehaviour
{
	[Header("UI References")]
	[Tooltip("Drag your RTLTMPro (or standard TMP) component here.")]
	public TMP_Text interactionText;

	[Tooltip("Drag your Crosshair image here.")]
	public Image crosshairImage;

	[Header("Settings")]
	public Color normalColor = Color.white;
	public Color interactColor = Color.green;

	// Use this for initialization to ensure the UI starts in the correct state
	void Start()
	{
		// Ensure the text is hidden when the game starts
		if (interactionText != null)
		{
			interactionText.gameObject.SetActive(false);
		}

		// Reset crosshair color
		if (crosshairImage != null)
		{
			crosshairImage.color = normalColor;
		}
	}


	/// <summary>
	/// Connect this to 'On Object Enter'.
	/// </summary>
	public void PickableObjectIsInRange(PickableObject obj)
	{
		// Show the text object
		if (interactionText != null)
		{
			interactionText.gameObject.SetActive(true);

			// Optional: You can still change the text string based on the object
			//interactionText.text = "Press E to Interact";
		}

		// Change crosshair to green for visual feedback
		if (crosshairImage != null)
		{
			crosshairImage.color = interactColor;
		}
	}

	/// <summary>
	/// Connect this to 'On Object Exit'.
	/// </summary>
	public void PickableObjectIsNotInRange(PickableObject obj)
	{
		// Hide the text object completely
		if (interactionText != null)
		{
			interactionText.gameObject.SetActive(false);
		}

		// Change crosshair back to white
		if (crosshairImage != null)
		{
			crosshairImage.color = normalColor;
		}
	}

/*
	/// <summary>
	/// Connect this to the 'On Object Enter' event in your Raycast Script.
	/// </summary>
	public void PickableObjectIsInRange(PickableObject obj)
	{
		UpdateUIColors(interactColor);

		// Bonus: Show the text if you want!
		if (interactionText != null)
		{
			interactionText.text = "Pick up";
		}
	}

	/// <summary>
	/// Connect this to the 'On Object Exit' event in your Raycast Script.
	/// </summary>
	public void PickableObjectIsNotInRange(PickableObject obj)
	{
		UpdateUIColors(normalColor);

		// Bonus: Clear the text
		if (interactionText != null)
		{
			interactionText.text = "";
		}
	}

	// Helper method to keep code clean
	private void UpdateUIColors(Color color)
	{
		if (crosshairImage != null)
		{
			crosshairImage.color = color;
		}

		if (interactionText != null)
		{
			interactionText.color = color;
		}
	}*/
}