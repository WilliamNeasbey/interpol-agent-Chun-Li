using System.Collections;
using UnityEngine;

public class ChairSpawner : MonoBehaviour
{
    public GameObject chairPrefab;
    public float minSpawnDelay = 0.5f;
    public float maxSpawnDelay = 1f;

    private void Start()
    {
        StartCoroutine(SpawnChairs());
    }

    private IEnumerator SpawnChairs()
    {
        yield return new WaitForSeconds(5f); // Wait for 5 seconds before starting to spawn chairs

        while (true)
        {
            float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(waitTime);

            SpawnChair();
        }
    }

    private void SpawnChair()
    {
        // Instantiate the chair prefab at a desired position and rotation
        Instantiate(chairPrefab, transform.position, transform.rotation);
    }
}
