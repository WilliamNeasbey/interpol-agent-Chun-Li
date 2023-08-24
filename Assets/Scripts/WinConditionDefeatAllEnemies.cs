using UnityEngine;
using UnityEngine.SceneManagement;

public class WinConditionDefeatAllEnemies : MonoBehaviour
{
    public GameObject winScreen;
    public GameObject playerObject; 
    public AudioSource musicAudioSource; 
    public float delayBeforeLoading = 3f;
    public string nextSceneName;

    private EnemyManager enemyManager;
    private bool isPlayerDisabled = false;
    private bool isMusicDisabled = false;

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
        if (enemyManager.aliveEnemyCount == 0)
        {
            // Enable the win screen (assuming it's initially disabled in the Inspector)
            if (winScreen != null)
                winScreen.SetActive(true);
            playerObject.SetActive(false);
            musicAudioSource.mute = true;

            // Load the next scene after the specified delay
            //Invoke("LoadNextScene", delayBeforeLoading);
        }
    }

    private void LoadNextScene()
    {
        // Load the next scene using SceneManager
        SceneManager.LoadScene(nextSceneName);
    }
}
