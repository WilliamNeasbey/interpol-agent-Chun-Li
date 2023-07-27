using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName;

    private bool isVideoFinished = false;

    private void Awake()
    {
        // Make sure to assign the VideoPlayer component to the videoPlayer variable in the Inspector.
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer reference is missing! Please assign the VideoPlayer component in the Inspector.");
            this.enabled = false;
        }
    }

    private void Start()
    {
        // Subscribe to the videoPlayer loopPointReached event to detect when the video has finished playing.
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // The video has finished playing.
        isVideoFinished = true;
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        // Load the specified next scene.
        SceneManager.LoadScene(nextSceneName);
    }
}
