using UnityEngine;

public class GrenadeLanding : MonoBehaviour
{
    public GameObject smokeParticlePrefab;
    public GameObject explosionPrefab;
    public string validSurfaceTag = "Floor";
    public LayerMask environmentLayer; // set this to your "Environment" layer in the Inspector
    public float destroyDelay = 0.1f;

    private bool hasLanded = false;

    void OnCollisionEnter(Collision collision)
    {
        if (hasLanded) return;

        int otherLayer = collision.gameObject.layer;

        // Ignore collisions with anything that isn't part of the environment
        // (e.g. player, enemies, other grenades) - let it just bounce/pass normally
        if ((environmentLayer.value & (1 << otherLayer)) == 0)
        {
            return;
        }

        // It's an environment object - only actually "land" if it's tagged Floor
        if (collision.gameObject.CompareTag(validSurfaceTag))
        {
            Land();
        }
        // else: it's environment but not floor (e.g. a wall) -> do nothing,
        // physics bounce happens automatically based on the Physic Material
    }

    private void Land()
    {
        hasLanded = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity); 

        GameObject smokeInstance = Instantiate(smokeParticlePrefab, transform.position, Quaternion.identity);
        SmokeController smokeController = smokeInstance.GetComponent<SmokeController>();
        if (smokeController != null)
        {
            smokeController.PlaySmoke();
        }
        else
        {
            Debug.LogWarning("SmokeController not found on instantiated smoke prefab!");
        }


        Destroy(gameObject, destroyDelay);
    }
}