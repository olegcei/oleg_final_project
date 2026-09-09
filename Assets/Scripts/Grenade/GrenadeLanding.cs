using UnityEngine;

public class GrenadeLanding : MonoBehaviour
{
    public GameObject smokeParticlePrefab;
    public string validSurfaceTag = "Floor";
    public float destroyDelay = 0.1f;

    private bool hasLanded = false;

    void OnCollisionEnter(Collision collision)
    {
        if (hasLanded) return;

        if (collision.gameObject.CompareTag(validSurfaceTag))
        {
            hasLanded = true;

            // stop it dead so it doesn't roll around
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;

            Instantiate(smokeParticlePrefab, transform.position, Quaternion.identity);

            Destroy(gameObject, destroyDelay);
        }
    }
}