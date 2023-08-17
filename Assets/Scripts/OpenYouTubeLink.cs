using UnityEngine;

public class OpenYouTubeLink : MonoBehaviour
{
    // This function is called when the button is pressed
    public void OpenYouTube()
    {
        // Replace this with your desired YouTube link
        string youtubeLink = "https://www.youtube.com/watch?v=YOUR_VIDEO_ID";

        // Open the YouTube link in the default web browser
        Application.OpenURL(youtubeLink);
    }
}
