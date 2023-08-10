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

    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        currentSceneDelay = GetSceneDelay(currentSceneName);

        if (ShouldPlayMusicOnScene(currentSceneName))
        {
            if (currentSceneDelay > 0f && !musicTriggered)
            {
                StartCoroutine(StartMusicWithDelay(currentSceneDelay));
            }
            else if (musicTriggered && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (musicTriggered)
        {
            audioSource.Stop();
        }
    }

    public void TriggerMusic()
    {
        musicTriggered = true;
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private IEnumerator StartMusicWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        TriggerMusic();
    }

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

        if (ShouldPlayMusicOnScene(currentSceneName))
        {
            if (currentSceneDelay > 0f && !musicTriggered)
            {
                StartCoroutine(StartMusicWithDelay(currentSceneDelay));
            }
            else if (musicTriggered && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (musicTriggered)
        {
            audioSource.Stop();
        }
    }

    private bool ShouldPlayMusicOnScene(string sceneName)
    {
        return sceneName == "CutscenePurpleGuyTransform" || sceneName == "FreddyPart2";
    }

    private float GetSceneDelay(string sceneName)
    {
        if (sceneName == "CutscenePurpleGuyTransform")
        {
            return 17f; // Replace with the desired delay time for Scene1 in seconds
        }
        else if (sceneName == "FreddyPart2")
        {
            return 0.1f; // Replace with the desired delay time for Scene2 in seconds
        }

        return 0f; // Default delay time for scenes not specified above
    }
}
