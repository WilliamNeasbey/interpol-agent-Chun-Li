using System.Collections;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public Transform startObject;
    public Transform endObject;
    public float movementDuration = 2.0f;

    private float elapsedTime = 0.0f;

    private void Start()
    {
        StartCoroutine(MoveObject());
    }

    private IEnumerator MoveObject()
    {
        Vector3 initialPosition = startObject.position;
        Vector3 targetPosition = endObject.position;

        while (elapsedTime < movementDuration)
        {
            float t = elapsedTime / movementDuration;
            startObject.position = Vector3.Lerp(initialPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the object reaches exactly the target position
        startObject.position = targetPosition;
    }
}
