using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	// The "Singleton" Instance - this allows other scripts to say "GameManager.Instance"
	public static GameManager Instance { get; private set; }

	[Header("Audio Settings")]
	[Tooltip("Drag the Audio Source that will play the voice lines here.")]
	public AudioSource voiceLineSource;

	[Header("Game State (Read Only)")]
	public bool itemWasPicked = false;
	public bool puzzleSolvedCorrectly = false;

	

    private int numberOfLosses = 0;

    private int currentLevelLossTimes = 0;

    private AudioSource audioSource;

    private int currentLevel = 1;

    [SerializeField] List<LevelData> levelSounds;



    [Header("Fade Settings")]
    private CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 2.0f; // Control the time here

    void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            // If so, destroy this new object to enforce the singleton pattern
            Destroy(this.gameObject);
            return;
        }
        audioSource = GetComponent<AudioSource>();
        fadeCanvasGroup = GameObject.Find("CanvasFadeGroup").GetComponent<CanvasGroup>();
        StartCoroutine(FadeOut());

        AudioPlay(levelSounds[0].audioEntries[0].clip);

        // If not, set the instance to this and make it persistent
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Restart()
    {
        numberOfLosses++;
        currentLevelLossTimes++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        StartCoroutine(FadeOut());
        
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
        }
        else
        {
            AudioPlay(GetSoundFromList(currentLevel, "Respawn"));
        }
    }

    private IEnumerator FadeOut()
    {
        float timer = 0;

        // Ensure the alpha starts at 1 (fully black)
        fadeCanvasGroup.alpha = 1;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Linearly interpolate alpha from 1 to 0
            fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null; // Wait for the next frame
        }

        fadeCanvasGroup.alpha = 0;
        fadeCanvasGroup.blocksRaycasts = false; // Allows clicking "through" the faded screen
    }

    public void EndOfLevelCheck()
    {
        if (puzzleSolvedCorrectly)
        {
            if (currentLevel == 3 || currentLevel == 5)
            {
                AudioPlay(GetSoundFromList(currentLevel, "SFX"));
                StartCoroutine(DelayedAudioPlay(3, GetSoundFromList(currentLevel, "Win")));
                StartCoroutine(LoadNextScene(6));
            }
            else if (currentLevel == 7)
            {
                AudioPlay(GetSoundFromList(currentLevel, "Win"));
                StartCoroutine(LoadNextScene(12));

            }else if (currentLevel == 9)
            {
                AudioPlay(GetSoundFromList(currentLevel, "SFX"));
                StartCoroutine(LoadNextScene(3));
            }
            else if (currentLevel == 10)
            {
                AudioPlay(GetSoundFromList(currentLevel, "SFX"));
                StartCoroutine(LoadNextScene(2));
            }
            else if (currentLevel != 4 && currentLevel > 1)
            {
                AudioPlay(GetSoundFromList(currentLevel, "Win"));
                StartCoroutine(LoadNextScene(2));
            }else if(currentLevel == 4)
            {
                StartCoroutine(LoadNextScene(0));
            }
        }
        else
        {
            LossTrigger();
            fadeCanvasGroup.alpha = 1;
        }

    }

    private void LossTrigger()
    {
        //Trigger the Loss/Restart UI

        if (currentLevelLossTimes > 2)
            AudioPlay(GetSoundFromList(11,"Provoke"));
        else if (currentLevel > 3)
            AudioPlay(GetSoundFromList(11,"Flatline"));
    }

    private void StoryAudio()
    {
        AudioPlay(GetSoundFromList(currentLevel,"Story"));
    }

    private void DoorAudioFirstLevel()
    {
        AudioPlay(GetSoundFromList(currentLevel, "Door"));
    }

    private void BedAudioFirstLevel()
    {
        AudioPlay(GetSoundFromList(currentLevel, "Bed"));
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

            fadeCanvasGroup = GameObject.Find("FadeImage").GetComponent<CanvasGroup>();
            StartCoroutine(FadeOut());

            audioSource.Stop();

            if (currentLevel <= 10) {
                AudioPlay(GetSoundFromList(currentLevel, "Spawn"));
            }

            if (currentLevel == 6) {

                DelayedAudioPlay(15, GetSoundFromList(currentLevel,"Hint"));
                StartCoroutine(UnlockDoorOnLevel6(6));
            }
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
        itemWasPicked = true;
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
		// 1. Play the voice line (if one was provided)
		if (voiceLine != null && voiceLineSource != null)
		{
			voiceLineSource.Stop(); // Stop any previous line so they don't overlap
			voiceLineSource.PlayOneShot(voiceLine);
		}

		// 2. Mark that an item was picked
		itemWasPicked = true;

		// 3. Check if the puzzle is solved based on the object type
		// We assume 'SpecialObject' is the correct solution
		if (obj.objectType == ObjectType.SpecialObject)
		{
			puzzleSolvedCorrectly = true;
			Debug.Log("Puzzle Solved! Correct item selected.");
		}
		else
		{
			puzzleSolvedCorrectly = false;
			Debug.Log("Wrong item selected.");
		}
	}

}


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