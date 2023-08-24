using System.Collections;
using UnityEngine;

public class ObjectScaling : MonoBehaviour
{
    public Transform objectToScale;
    public Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 endScale = new Vector3(2.0f, 2.0f, 2.0f);
    public float scalingDuration = 2.0f;

    private float elapsedTime = 0.0f;

    private void Start()
    {
        StartCoroutine(ScaleObject());
    }

    private IEnumerator ScaleObject()
    {
        Vector3 initialScale = startScale;
        Vector3 targetScale = endScale;

        while (elapsedTime < scalingDuration)
        {
            float t = elapsedTime / scalingDuration;
            objectToScale.localScale = Vector3.Lerp(initialScale, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the object reaches exactly the target scale
        objectToScale.localScale = targetScale;
    }
}
