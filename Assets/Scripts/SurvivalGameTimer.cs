using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SurvivalGameTimer : MonoBehaviour
{
    public GameObject playerObject;
    public Canvas timerCanvas;
    public TMP_Text currentTimeText;
    public TMP_Text bestTimeText;

    public Canvas resultsCanvas;
    public TMP_Text resultsTimeText;
    public TMP_Text resultsBestTimeText;

    private float startTime;
    private float currentTime;
    private float bestTime;
    private bool playerDeleted;

    private const string BestTimeKey = "BestTime";

    void Start()
    {
        // Load the best time from PlayerPrefs
        bestTime = PlayerPrefs.GetFloat(BestTimeKey, float.MaxValue);

        // Initialize variables
        startTime = Time.time;
        currentTime = 0f;
        playerDeleted = false;

        // Display the timer UI
        DisplayTimerUI();

        // Disable the mouse cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Check if the "0" key is pressed to reset best time
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ResetBestTime();
        }
        // Check if the player object is deleted from the scene
        if (playerObject == null && !playerDeleted)
        {
            // End the timer
            EndTimer();

            // Save the best time to PlayerPrefs
            PlayerPrefs.SetFloat(BestTimeKey, bestTime);
            PlayerPrefs.Save();

            playerDeleted = true;

            // Display the results UI
            DisplayResultsUI();
        }

        // Update the current time continuously
        UpdateTime();

        // Update the UI
        UpdateUI();

        // Other game logic...
        // Update game state...
    }

    void EndTimer()
    {
        currentTime = Time.time - startTime;
        if (currentTime > bestTime)
        {
            bestTime = currentTime;
        }
    }

    void UpdateTime()
    {
        if (!playerDeleted)
        {
            currentTime = Time.time - startTime;
        }
    }

    void UpdateUI()
    {
        currentTimeText.text = "Current Time: " + currentTime.ToString("F2");
        bestTimeText.text = "Best Time: " + bestTime.ToString("F2");

        resultsTimeText.text = "Time: " + currentTime.ToString("F2");
        resultsBestTimeText.text = "Best Time: " + bestTime.ToString("F2");
    }

    void DisplayTimerUI()
    {
        timerCanvas.gameObject.SetActive(true);
        resultsCanvas.gameObject.SetActive(false);
    }

    void DisplayResultsUI()
    {
        timerCanvas.gameObject.SetActive(false);
        resultsCanvas.gameObject.SetActive(true);
        // Enable the mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void ResetBestTime()
    {
        bestTime = 0f;
        PlayerPrefs.SetFloat(BestTimeKey, bestTime);
        PlayerPrefs.Save();
    }
}
