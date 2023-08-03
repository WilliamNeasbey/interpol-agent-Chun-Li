using UnityEngine;
using System.Collections;

public class EnableObjectAfterDelay : MonoBehaviour
{
    public GameObject objectToEnable;
    public float delayInSeconds = 2f;

    private void Start()
    {
        StartCoroutine(EnableObjectWithDelay());
    }

    private IEnumerator EnableObjectWithDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayInSeconds);

        // Enable the GameObject after the delay
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }

        // Trigger the music playback here 
        TransitionMusic TransitionMusic = FindObjectOfType<TransitionMusic>();
        if (TransitionMusic != null)
        {
            TransitionMusic.TriggerMusic();
        }
    }
}
