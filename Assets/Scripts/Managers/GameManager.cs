using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool puzzlesolved = false;

    private int numberOfLosses = 0;

    private int currentLevelLossTimes = 0;

    private AudioSource audioSource;

    private int currentLevel = 1;

    [SerializeField] List<LevelData> levelSounds;

    // Static instance of GameManager which allows it to be accessed by any other script.
    public static GameManager Instance { get; private set; }

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
        
        if(currentLevel == 2 && currentLevelLossTimes < 1)
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
        if (puzzlesolved)
        {
            if (currentLevel == 3 || currentLevel == 5 || currentLevel == 7 || currentLevel == 10)
                AudioPlay(GetSoundFromList(currentLevel, "SFX"));
            else if(currentLevel != 4)
                AudioPlay(GetSoundFromList(currentLevel, "Win"));

            StartCoroutine(LoadNextScene(2));
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

    public void StoryAudio(int levelnumber)
    {
        AudioPlay(GetSoundFromList(levelnumber,"Story"));
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
            SceneManager.LoadScene(nextSceneIndex);

            currentLevel++;

            fadeCanvasGroup = GameObject.Find("FadeImage").GetComponent<CanvasGroup>();
            StartCoroutine(FadeOut());

            AudioPlay(GetSoundFromList(currentLevel, "Spawn"));
        }
        else
        {
            // Optional: Load the first scene (index 0) if at the end of the build list
            SceneManager.LoadScene(0);
        }
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