using UnityEngine;

public class RagdollTrigger : MonoBehaviour
{
    public GameObject ragdollPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Chair"))
        {
            DisablePlayerObject();
            SpawnRagdoll();
        }
    }

    private void DisablePlayerObject()
    {
        gameObject.SetActive(false);
    }

    private void SpawnRagdoll()
    {
        Instantiate(ragdollPrefab, transform.position, transform.rotation);
    }
}
