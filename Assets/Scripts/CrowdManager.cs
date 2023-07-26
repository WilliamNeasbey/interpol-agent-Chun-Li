using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    public GameObject[] crowdModels;
    public int maxCrowdSize = 100;
    public BoxCollider spawnArea;
    public float spacing = 2f;

    private int currentCrowdSize = 0;

    private void Start()
    {
        SpawnAllCrowdMembers();
    }

    private void SpawnAllCrowdMembers()
    {
        Vector3 center = spawnArea.center;
        Vector3 size = spawnArea.size;

        // Calculate the number of crowd members that can fit in the BoxCollider grid
        int xCount = Mathf.FloorToInt(size.x / spacing);
        int zCount = Mathf.FloorToInt(size.z / spacing);
        int totalSpawns = xCount * zCount;
        int spawnCount = Mathf.Min(totalSpawns, maxCrowdSize);

        Debug.Log("Spawn Count: " + spawnCount);

        for (int i = 0; i < spawnCount; i++)
        {
            int xIndex = i % xCount;
            int zIndex = i / xCount;

            float x = center.x - size.x / 2f + xIndex * spacing + spacing / 2f;
            float y = center.y; // Keeps the Y-axis the same as the center of the BoxCollider.
            float z = center.z - size.z / 2f + zIndex * spacing + spacing / 2f;

            Vector3 spawnPosition = new Vector3(x, y, z);

            int randomModelIndex = Random.Range(0, crowdModels.Length);
            GameObject crowdMember = Instantiate(crowdModels[randomModelIndex], spawnPosition, Quaternion.identity);
            currentCrowdSize++;

            Debug.Log("Spawned at: " + spawnPosition);
        }
    }
}
