using UnityEngine;

// Se pone en el prefab del decal (la marca de bala).
// Su unica tarea es borrar el decal despues de unos segundos.
public class DecalAutoDestroy : MonoBehaviour
{
    // Segundos que el decal permanece en la pared antes de desaparecer.
    public float lifetime = 20f;

    // El proyectil llama a este metodo para cambiar la duracion desde su propio script.
    public void SetLifetime(float newLifetime)
    {
        lifetime = newLifetime;
    }

    // Start se ejecuta una sola vez, al crearse el decal.
    private void Start()
    {
        // Destroy con dos parametros = "destruye este objeto dentro de X segundos".
        Destroy(gameObject, lifetime);
    }
}
