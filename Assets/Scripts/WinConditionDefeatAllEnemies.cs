using UnityEngine;
using UnityEngine.SceneManagement;

public class WinConditionDefeatAllEnemies : MonoBehaviour
{
    public GameObject winScreen;
    public GameObject playerObject;
    public GameObject playerUIObject;
    public GameObject playerCamera;
    public GameObject playerfinalhitCamera;
    public AudioSource musicAudioSource;
    public float delayBeforeLoading = 2f;
    public string nextSceneName;

    private EnemyManager enemyManager;
    private bool isPlayerDisabled = false;
    private bool hasWon = false; // Flag to track if the win screen has been shown
    public Vector3 winScreenSpawnPosition;

    private void Start()
    {
        // Find and reference the EnemyManager script in the scene
        enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager == null)
        {
            Debug.LogError("No EnemyManager script found in the scene!");
        }
    }

    private void Update()
    {
        // Check if all enemies are dead (aliveEnemyCount reaches 0)
        if (!hasWon && enemyManager.aliveEnemyCount == 0)
        {
            // Enable the win screen (assuming it's initially disabled in the Inspector)
            if (winScreen != null)
            {
                Time.timeScale = 1f;
                Instantiate(winScreen, winScreenSpawnPosition, transform.rotation);
                playerObject.SetActive(false);
                musicAudioSource.mute = true;
                playerUIObject.SetActive(false);
                playerCamera.SetActive(false);
                playerfinalhitCamera.SetActive(false);
                isPlayerDisabled = true;
                hasWon = true; // Set the flag to true after showing the win screen
                //Invoke("ActivateWinScreen", delayBeforeLoading);
            }
        }
    }

    private void ActivateWinScreen()
    {
        winScreen.SetActive(true);
        playerObject.SetActive(false);
        musicAudioSource.mute = true;
        playerUIObject.SetActive(false);
        playerCamera.SetActive(false);
        isPlayerDisabled = true;
    }

    private void CountdownTimer()
    {
        // Invoke the ActivateWinScreen method with the specified delay
        Invoke("ActivateWinScreen", delayBeforeLoading);
    }

    private void LoadNextScene()
    {
        // Load the next scene using SceneManager
        SceneManager.LoadScene(nextSceneName);
    }
}
