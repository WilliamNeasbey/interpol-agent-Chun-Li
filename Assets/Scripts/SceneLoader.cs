using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public string sceneNameToLoad; // Name of the scene you want to load
    public Button loadSceneButton; // Reference to the Button component in the UI

    private void Start()
    {
        // Add an event listener to the button to call LoadSceneOnClick when clicked
        loadSceneButton.onClick.AddListener(LoadSceneOnClick);
    }

    void LoadSceneOnClick()
    {
        // Load the specified scene
        SceneManager.LoadScene(sceneNameToLoad);
    }
}
