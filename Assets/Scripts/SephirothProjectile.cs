using UnityEngine;

public class SephirothProjectile : MonoBehaviour
{
    public float speed = 10f; // Adjust the speed as needed
    public GameObject explosionPrefab; // Assign the explosion prefab in the Inspector

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Apply forward force to the rigidbody
        rb.velocity = transform.forward * speed;

        // Rotate the projectile 180 degrees around the Y-axis
        transform.Rotate(0f, 180f, 0f);

        // Invoke the SpawnExplosion method after 2 seconds
        Invoke("SpawnExplosion", 2f);

        // Destroy the projectile after a certain time (e.g., 3 seconds)
        Destroy(gameObject, 2f);
    }

    private void SpawnExplosion()
    {
        if (explosionPrefab != null)
        {
            // Spawn the explosion prefab at the projectile's position
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Rename the projectile
            gameObject.name = "Sephiroth Projectile Start";
        }
        else
        {
            Debug.LogWarning("Explosion prefab not assigned!");
        }
    }
}
