using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class TransitionMusic : MonoBehaviour
{
    private static TransitionMusic instance;
    private AudioSource audioSource;
    private bool musicTriggered = false;

    private string currentSceneName;
    private float currentSceneDelay = 0f;

    // Ensure only one instance of MusicPlayer exists
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    // Play music if already triggered
    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        currentSceneDelay = GetSceneDelay(currentSceneName);

        if (currentSceneDelay > 0f && !musicTriggered)
        {
            StartCoroutine(StartMusicWithDelay(currentSceneDelay));
        }
        else if (musicTriggered && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    // Call this method to trigger the music playback
    public void TriggerMusic()
    {
        musicTriggered = true;
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    // Wait for the initial delay before triggering music
    private IEnumerator StartMusicWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        TriggerMusic();
    }

    // Check if music should play when a new scene is loaded
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string newSceneName = scene.name;
        currentSceneName = newSceneName;
        currentSceneDelay = GetSceneDelay(newSceneName);

        // Always trigger the music when a new scene is loaded, ignoring the initial delay for subsequent scenes
        if (currentSceneDelay > 0f && !musicTriggered)
        {
            StartCoroutine(StartMusicWithDelay(currentSceneDelay));
        }
        else if (musicTriggered && !audioSource.isPlaying)
        {
            audioSource.Play();
        }

        // Stop the music when transitioning to Scene3
        if (currentSceneName == "titleScreen")
        {
            audioSource.Stop();
        }
    }

    private float GetSceneDelay(string sceneName)
    {
        // Set different delay times for specific scenes
        if (sceneName == "CutscenePurpleGuyTransform")
        {
            return 17f; // Replace with the desired delay time for Scene1 in seconds
        }
        else if (sceneName == "FreddyPart2")
        {
            return 0.1f; // Replace with the desired delay time for Scene2 in seconds
        }

        // Add more conditions for other scenes as needed
        // If the scene is not listed here, it will have no delay (0 seconds)

        return 0f; // Default delay time for scenes not specified above
    }
}
