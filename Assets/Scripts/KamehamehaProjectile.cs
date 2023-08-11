using UnityEngine;

public class KamehamehaProjectile : MonoBehaviour
{
    public float speed = 10f; // Adjust the speed as needed

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Apply forward force to the rigidbody
        rb.velocity = transform.forward * speed;

        // Destroy the projectile after a certain time (e.g., 3 seconds)
        Destroy(gameObject, 3f);
    }
}
