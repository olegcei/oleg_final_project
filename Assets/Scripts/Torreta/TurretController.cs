using UnityEngine;

// Torreta que busca al jugador, le apunta y le dispara proyectiles.
public class TurretController : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;            // Si lo dejamos vacio, la torreta busca al jugador por su tag.
    public string playerTag = "Player"; // Tag que debe tener el jugador en la escena.

    [Header("Apuntado")]
    public Transform rotatingPart;      // Parte que gira (la cabeza). Si esta vacia gira la torreta entera.
    public float rotationSpeed = 180f;  // Grados por segundo.

    [Header("Disparo")]
    public Transform firePoint;             // Punto por donde sale el proyectil.
    public GameObject projectilePrefab;     // Prefab del proyectil.
    public float timeBetweenShots = 1f;     // Segundos de espera entre disparo y disparo.
    public float attackRange = 20f;         // Distancia maxima a la que dispara.
    public float firstShotDelay = 0.5f;     // Espera antes del primer disparo.

    [Header("Linea de vision")]
    public LayerMask visionMask;      // Capas que TAPAN al jugador (paredes, suelo...). No incluir al jugador.

    private float nextShotTime;         // Momento (en segundos de partida) del siguiente disparo.

    private void Start()
    {
        // Si no hemos asignado la parte giratoria, giramos el objeto completo.
        if (rotatingPart == null)
        {
            rotatingPart = transform;
        }

        // Buscamos al jugador una sola vez, al empezar la partida.
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);

            if (player != null)
            {
                target = player.transform;
            }
        }

        // Time.time son los segundos que llevamos de partida.
        nextShotTime = Time.time + firstShotDelay;
    }

    private void Update()
    {
        // Sin objetivo (o si el jugador se ha destruido) no hay nada que hacer.
        if (target == null)
        {
            return;
        }

        // Si el jugador esta demasiado lejos, la torreta ni apunta ni dispara.
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > attackRange)
        {
            return;
        }

        AimAtTarget();

        // Dispara si ya toca por tiempo y si ve al jugador.
        if (Time.time >= nextShotTime && HasLineOfSight())
        {
            Shoot();
            nextShotTime = Time.time + timeBetweenShots;
        }
    }

    // Gira la torreta hacia el jugador, solo en el eje Y (no se inclina).
    private void AimAtTarget()
    {
        Vector3 direction = target.position - rotatingPart.position;
        direction.y = 0f;

        // Si el jugador esta justo encima o debajo, la direccion es casi cero: no giramos.
        if (direction == Vector3.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // RotateTowards gira un poco cada frame, sin pasarse del angulo final.
        rotatingPart.rotation = Quaternion.RotateTowards(rotatingPart.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Devuelve true si no hay ningun obstaculo entre el punto de disparo y el jugador.
    private bool HasLineOfSight()
    {
        if (firePoint == null)
        {
            return false;
        }

        Vector3 direction = target.position - firePoint.position;
        float distance = direction.magnitude;

        if (Physics.Raycast(firePoint.position, direction.normalized, out RaycastHit hit, distance, visionMask, QueryTriggerInteraction.Collide))
        {
            // Si lo primero que toca el rayo es el jugador (o algo dentro de su jerarquia), hay vision.
            return hit.transform == target || hit.transform.IsChildOf(target);
        }

        // Si el rayo no golpea nada dentro del rango, tampoco hay vision (raro, pero por seguridad).
        return false;
    }

    // Crea un proyectil en el punto de disparo, mirando hacia donde mira ese punto.
    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Faltan referencias en la torreta: Projectile Prefab o Fire Point.");
            return;
        }

        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Le decimos al proyectil quien lo ha disparado para que no se choque con la torreta.
        TurretProjectile projectile = newProjectile.GetComponent<TurretProjectile>();

        if (projectile != null)
        {
            projectile.SetOwner(gameObject);
        }
    }

    // Dibuja ayudas visuales en el editor cuando la torreta esta seleccionada.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (firePoint != null && target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(firePoint.position, target.position);
        }
    }
}
