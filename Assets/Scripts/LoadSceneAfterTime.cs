using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneAfterTime : MonoBehaviour
{
    public float delayTime = 5.0f; // Time in seconds before loading the new scene
    public string sceneNameToLoad; // Name of the scene you want to load

    private float timer = 0.0f;
    private bool isLoading = false;

    private void Update()
    {
        if (!isLoading)
        {
            timer += Time.deltaTime;

            if (timer >= delayTime)
            {
                isLoading = true;
                LoadNewScene();
            }
        }
    }

    private void LoadNewScene()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }
}
