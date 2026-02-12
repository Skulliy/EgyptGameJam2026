using UnityEngine;
using System.Collections;

public class GameOverPlayerActivator : MonoBehaviour
{
	[Header("Object Management")]
	[Tooltip("The first object to disable (e.g., The Player)")]
	public GameObject objectToDisable1;

	[Tooltip("The second object to disable (e.g., The Gameplay UI)")]
	public GameObject objectToDisable2;

	[Tooltip("The object to enable (e.g., The Game Over Screen)")]
	public GameObject objectToEnable;

	[Header("Timing")]
	[Tooltip("How long to wait before restarting the game.")]
	public float restartDelay = 3.0f;

	/// <summary>
	/// The main method to trigger the Game Over sequence.
	/// Call this from your game logic when the player loses.
	/// </summary>
	public void TriggerGameOver()
	{
		// 1. Disable the gameplay objects
		if (objectToDisable1 != null) objectToDisable1.SetActive(false);
		if (objectToDisable2 != null) objectToDisable2.SetActive(false);

		// 2. Enable the Game Over UI/Camera
		if (objectToEnable != null) objectToEnable.SetActive(true);

		// 3. Start the timer to restart
		StartCoroutine(WaitAndRestart());
	}

	private IEnumerator WaitAndRestart()
	{
		// Wait for the specified number of seconds
		yield return new WaitForSeconds(restartDelay);

		// Call the Restart method on our GameManager Singleton
		if (GameManager.Instance != null)
		{
			GameManager.Instance.Restart();
		}
		else
		{
			Debug.LogError("GameManager Instance not found!");
		}
	}
}