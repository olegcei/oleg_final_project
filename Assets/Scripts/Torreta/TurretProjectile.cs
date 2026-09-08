using UnityEngine;

// Obliga a que el proyectil tenga Rigidbody y Collider.
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class TurretProjectile : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 20f;     // Velocidad del proyectil.
    public float lifeTime = 5f;   // Segundos antes de destruirse si no choca con nada.

    [Header("Daño")]
    public float damage = 10f;    // Vida que quita al jugador.

    [Header("Decal de impacto")]
    public GameObject impactDecalPrefab; // Marca de bala que se pega en la superficie.
    public float decalLifetime = 8f;     // Segundos que dura la marca.
    public float decalScale = 0.2f;     // Tamaño de la marca.

    private GameObject owner;     // Torreta que ha disparado.
    private bool hasImpacted;     // Evita que el mismo proyectil haga daño dos veces.

    private void Start()
    {
        // Empujamos el proyectil hacia delante una sola vez; el Rigidbody hace el resto.
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;

        // Si no choca con nada, se autodestruye.
        Destroy(gameObject, lifeTime);
    }

    // La torreta llama a este metodo justo despues de crear el proyectil.
    public void SetOwner(GameObject newOwner)
    {
        owner = newOwner;

        Collider myCollider = GetComponent<Collider>();
        Collider[] ownerColliders = owner.GetComponentsInChildren<Collider>();

        // Desactivamos la colision con la propia torreta para que no se dispare a si misma.
        foreach (Collider ownerCollider in ownerColliders)
        {
            Physics.IgnoreCollision(myCollider, ownerCollider);
        }
    }

    // Unity llama a este metodo cuando el proyectil choca con algo.
    private void OnCollisionEnter(Collision collision)
    {
        // Si ya hemos impactado, ignoramos choques posteriores.
        if (hasImpacted)
        {
            return;
        }

        hasImpacted = true;

        // Buscamos vida en el objeto golpeado (GetComponentInParent tambien mira al padre,
        // util si el collider esta en un hijo del personaje).
        PlayerHealth playerHealth = collision.collider.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            // Hemos dado a un personaje: le quitamos vida y no dejamos marca de bala.
            playerHealth.TakeDamage(damage);
        }
        else
        {
            // Hemos dado a una superficie: dejamos la marca en el punto del impacto.
            SpawnDecal(collision);
        }

        Destroy(gameObject);
    }

    // Crea la marca de bala pegada a la superficie golpeada.
    private void SpawnDecal(Collision collision)
    {
        if (impactDecalPrefab == null)
        {
            return;
        }

        // El primer punto de contacto nos da la posicion y la normal (hacia donde "mira" la pared).
        ContactPoint contact = collision.GetContact(0);

        // Separamos la marca un poquito de la pared para que no se solapen las dos superficies.
        Vector3 position = contact.point + contact.normal * 0.01f;

        // Orientamos el decal para que su eje Z apunte hacia fuera de la pared.
        Quaternion rotation = Quaternion.LookRotation(contact.normal);

        // El ultimo parametro hace que la marca sea hija del objeto golpeado
        // (asi se mueve con el si es una plataforma movil).
        GameObject decal = Instantiate(impactDecalPrefab, position, rotation, collision.transform);

        decal.transform.localScale = Vector3.one * decalScale;

        // Giro aleatorio sobre la normal para que todas las marcas no salgan iguales.
        decal.transform.Rotate(Vector3.forward, Random.Range(0f, 360f), Space.Self);

        // Si el decal tiene su propio script de borrado le pasamos la duracion;
        // si no, lo destruimos nosotros.
        DecalAutoDestroy autoDestroy = decal.GetComponent<DecalAutoDestroy>();

        if (autoDestroy != null)
        {
            autoDestroy.SetLifetime(decalLifetime);
        }
        else
        {
            Destroy(decal, decalLifetime);
        }
    }
}
