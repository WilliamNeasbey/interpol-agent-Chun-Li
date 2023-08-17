using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed = 5f;
    public float despawnTime = 6f;
    public GameObject ragdollPrefab;

    private Rigidbody rb;
    private bool collided;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, despawnTime);
    }

    private void FixedUpdate()
    {
        MoveForward();
    }

    private void MoveForward()
    {
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !collided)
        {
            EnableRagdoll(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void EnableRagdoll(GameObject player)
    {
        GameObject ragdoll = Instantiate(ragdollPrefab, player.transform.position, player.transform.rotation);
        ragdoll.SetActive(true);
        Destroy(player);
    }
}
