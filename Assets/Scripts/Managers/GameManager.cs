using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
	// The "Singleton" Instance - this allows other scripts to say "GameManager.Instance"
	public static GameManager Instance { get; private set; }

	/*[Header("Audio Settings")]
	[Tooltip("Drag the Audio Source that will play the voice lines here.")]
	public AudioSource voiceLineSource;*/

	[Header("Game State (Read Only)")]
	public bool itemWasPicked = false;
	public bool puzzleSolvedCorrectly = false;

	// This variable will hold your player reference
	public GameObject currentPlayerReference;

    [SerializeField] private Vector3 roomEntryPosition;

	private int numberOfLosses = 0;

    private int currentLevelLossTimes = 0;

    private AudioSource audioSource;

    private int currentLevel;

    [SerializeField] List<LevelData> levelSounds;

    [Header("Fade Settings")]
    private CanvasGroup fadeCanvasGroup;

    private Coroutine level6Loss;

    void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            // If so, destroy this new object to enforce the singleton pattern
            Destroy(this.gameObject);
            return;
        }
        currentPlayerReference = GameObject.FindGameObjectWithTag("Player");
        //for debugging
        currentLevel = SceneManager.GetActiveScene().buildIndex;

        audioSource = GetComponent<AudioSource>();

		roomEntryPosition = GameObject.FindGameObjectWithTag("Position").transform.position;

		fadeCanvasGroup = GameObject.Find("CanvasFadeGroup").GetComponent<CanvasGroup>();

		StartCoroutine(DelayedAudioPlay(1, GetSoundFromList(currentLevel, "Spawn")));

        // If not, set the instance to this and make it persistent
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Restart()
    {
        numberOfLosses++;
        currentLevelLossTimes++;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        itemWasPicked = false;

        FadeOut();
        
        if(currentLevel == 2 && currentLevelLossTimes == 1)
        {
            AudioPlay(GetSoundFromList(currentLevel, "Hint"));
        }
        else if (currentLevel == 2 && currentLevelLossTimes > 1)
        {
            AudioPlay(GetSoundFromList(currentLevel, "Respawn"));
        }
        else if (currentLevel == 6)
        {
            AudioPlay(GetSoundFromList(currentLevel, "Hint"));
            StartCoroutine(UnlockDoorOnLevel6(1));
            level6Loss = StartCoroutine(Level6Death(25));
        }
        else
        {
            AudioPlay(GetSoundFromList(currentLevel, "Respawn"));
        }
    }

    private void FadeOut()
    {
        fadeCanvasGroup.gameObject.GetComponent<Animator>().enabled = true;
        fadeCanvasGroup.gameObject.GetComponent<Animator>().SetTrigger("Fade");
    }

    public void EndOfLevelCheck()
    {
        if (puzzleSolvedCorrectly)
        {
            
            fadeCanvasGroup.gameObject.GetComponent<Animator>().enabled = false;
            fadeCanvasGroup.alpha = 1;
            if (currentLevel == 3 || currentLevel == 5)
            {
                AudioPlay(GetSoundFromList(currentLevel, "Win"));
                StartCoroutine(DelayedAudioPlay(3, GetSoundFromList(currentLevel, "SFX")));
                StartCoroutine(LoadNextScene(7));
            }
            else if (currentLevel == 6)
            {
                AudioPlay(GetSoundFromList(currentLevel, "SFX"));
                StopCoroutine(level6Loss);
                StartCoroutine(LoadNextScene(4));
            }
            else if (currentLevel == 7)
            {
                AudioPlay(GetSoundFromList(currentLevel, "Win"));
                StartCoroutine(LoadNextScene(12));
            }
            else if (currentLevel == 9)
            {
                AudioPlay(GetSoundFromList(currentLevel, "SFX"));
                StartCoroutine(LoadNextScene(3));
            }
            else if (currentLevel == 10)
            {
                StartCoroutine(LoadNextScene(7));
            }
            else if (currentLevel != 4 && currentLevel > 1)
            {
                AudioPlay(GetSoundFromList(currentLevel, "Win"));
                StartCoroutine(LoadNextScene(3));
            }
            else if (currentLevel == 4)
                StartCoroutine(LoadNextScene(0));
            else if (currentLevel == 1)
                StartCoroutine(LoadNextScene(2));
        }
        else
        {
            LossTrigger();
        }

    }

    private void LossTrigger()
    {
        fadeCanvasGroup.gameObject.GetComponent<Animator>().enabled = false;
        fadeCanvasGroup.alpha = 1;
        //Trigger the Loss/Restart UI
        TriggerGameOverSequence();

		if (currentLevelLossTimes > 2)
            AudioPlay(GetSoundFromList(11,"Provoke"));
        else if (currentLevel > 3)
            AudioPlay(GetSoundFromList(11,"Flatline"));
    }
	public void TriggerGameOverSequence()
	{
		// Search the scene for the script
		GameOverPlayerActivator activator = FindObjectOfType<GameOverPlayerActivator>();

		if (activator != null)
		{
			activator.TriggerGameOver();
			Debug.Log("Game Over Sequence Started.");
		}
		else
		{
			Debug.LogError("Could not find a GameOverPlayerActivator in the scene!");
		}
	}

	private void StoryAudio()
    {
        AudioPlay(GetSoundFromList(currentLevel,"Story"));
    }

    private void DoorAudioFirstLevel()
    {
        AudioPlay(GetSoundFromList(currentLevel, "Door"));

        EndOfLevelCheck();
    }

    private void BedAudioFirstLevel()
    {
        AudioPlay(GetSoundFromList(currentLevel, "Bed"));

        EndOfLevelCheck();
    }

    private void RoomEntry()
    {
        FadeOut();
        currentPlayerReference.GetComponent<CharacterController>().enabled = false;
        currentPlayerReference.transform.position = roomEntryPosition;
        currentPlayerReference.GetComponent<CharacterController>().enabled = true;
        StartCoroutine(DelayedAudioPlay(2, GetSoundFromList(currentLevel, "Room")));

		//Call this function when you interact with the door in the corridor
		//feel free to add any functionality you want to this code
	}

    private void AudioPlay(AudioClip sound)
    {
        audioSource.PlayOneShot(sound);
    }

    IEnumerator LoadNextScene(float delay)
    {
        yield return new WaitForSeconds(delay);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Optional: Check if the next scene index is valid (prevents error if at the last scene)
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            currentLevel++;
            SceneManager.LoadScene(nextSceneIndex);

            itemWasPicked = false;
            puzzleSolvedCorrectly = false;

            FadeOut();

            audioSource.Stop();

            if (currentLevel == 6)
            {
                StartCoroutine(DelayedAudioPlay(4, GetSoundFromList(currentLevel, "Spawn")));
                StartCoroutine(DelayedAudioPlay(12, GetSoundFromList(currentLevel, "Hint")));
                StartCoroutine(UnlockDoorOnLevel6(11));
                level6Loss = StartCoroutine(Level6Death(28));

            }else if(currentLevel == 9)
            {
                StartCoroutine(DelayedAudioPlay(1, GetSoundFromList(currentLevel, "Spawn")));
                itemWasPicked = true;
                puzzleSolvedCorrectly = true;
            }
            else if (currentLevel < 10)
                StartCoroutine(DelayedAudioPlay(1, GetSoundFromList(currentLevel, "Spawn")));
            if (currentLevel == 11)
                Destroy(gameObject);
        }
        else
        {
            // Optional: Load the first scene (index 0) if at the end of the build list
            SceneManager.LoadScene(0);
        }
    }

    IEnumerator DelayedAudioPlay(int delay, AudioClip audio)
    {
        yield return new WaitForSeconds(delay);
        AudioPlay(audio);
    }

    IEnumerator UnlockDoorOnLevel6(int delay)
    {
        yield return new WaitForSeconds(delay);
        currentPlayerReference.GetComponentInChildren<ObjectSelector>().ToggleDoorOnly();
        itemWasPicked = true;
        puzzleSolvedCorrectly = true;
    }

    IEnumerator Level6Death(int timelimit)
    {
        yield return new WaitForSeconds(timelimit);
        puzzleSolvedCorrectly = false;
        EndOfLevelCheck();
    }

    private AudioClip GetSoundFromList(int targetLevel, string targetName)
    {
        // 1. Find the LevelData object that matches your level number
        LevelData level = levelSounds.Find(l => l.levelNumber == targetLevel);

        if (level != null)
        {
            // 2. Search inside that level's audioEntries for the matching name
            SoundEntry entry = level.audioEntries.Find(e => e.name == targetName);

            if (entry != null)
            {
                return entry.clip; // Success!
            }
            else
            {
                Debug.LogError($"Sound name '{targetName}' not found in Level {targetLevel}");
            }
        }
        else
        {
            Debug.LogError($"Level {targetLevel} not found in database");
        }
        return null; // Return null if either the level or sound name is missing
    }

	/// <summary>
	/// Called by ItemObjectActionOfGameMyRoom when an item is interacted with.
	/// </summary>
	public void ItemSelected(AudioClip voiceLine, PickableObject obj)
	{
        //play audio
        if (obj.gameObject.CompareTag("HallwayDoor"))
        {
            RoomEntry();
            return;
        }
		// 2. Mark that an item was picked
		itemWasPicked = true;
        TurnOffItimSelection();

		void TurnOffItimSelection()
        {
			ObjectSelector objSelect = currentPlayerReference.GetComponentInChildren<ObjectSelector>();
			if (objSelect != null)
			{
                objSelect.currentSelectionMode = ObjectSelector.SelectionMode.DoorOnly;
			}
			else
			{
				Debug.LogError("ObjectSelector component not found on player.");
			}
		}
		

		// 3. Check if the puzzle is solved based on the object type
		// We assume 'SpecialObject' is the correct solution
		if (obj.objectType == ObjectType.SpecialObject)
		{
			puzzleSolvedCorrectly = true;
			Debug.Log("Puzzle Solved! Correct item selected.");
            if (obj.gameObject.CompareTag("Story") && currentLevel == 10)
            {
                StoryAudio();
                StartCoroutine(DelayedAudioPlay(2, GetSoundFromList(currentLevel, "SFX")));
                EndOfLevelCheck();
            }
            else if (obj.gameObject.CompareTag("Story"))
                StoryAudio();
            else if (obj.gameObject.CompareTag("DoorFirstLevel"))
                DoorAudioFirstLevel();
            else if (obj.gameObject.CompareTag("BedFirstLevel"))
                BedAudioFirstLevel();
		}
		else
		{
			puzzleSolvedCorrectly = false;
			Debug.Log("Wrong item selected.");
		}

	}

    //this region is for getting a refrence to the player
	#region 


	private void OnEnable()
	{
		// Tell Unity to run our function whenever a scene loads
		SceneManager.sceneLoaded += FindPlayerInScene;
	}

	private void OnDisable()
	{
		// Clean up the link when the object is destroyed
		SceneManager.sceneLoaded -= FindPlayerInScene;
	}

	// This method runs every time a new scene is loaded
	private void FindPlayerInScene(Scene scene, LoadSceneMode mode)
	{
		// Attempt to find the object with the "Player" tag
		GameObject player = GameObject.FindWithTag("Player");

		if (player != null)
		{
			currentPlayerReference = player;
			
		}
		else
		{
			currentPlayerReference = null;
			
		}

        fadeCanvasGroup = GameObject.Find("CanvasFadeGroup").GetComponent<CanvasGroup>();
    }

}
#endregion

[System.Serializable]
public class SoundEntry
{
    public string name;
    public AudioClip clip;
}

[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public List<SoundEntry> audioEntries;
}