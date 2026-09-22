using UnityEngine;
using UnityEngine.AI;

// Este atributo obliga a que el GameObject tenga un NavMeshAgent.
// Si no lo tiene, Unity lo añade automáticamente cuando pongamos este script.
[RequireComponent(typeof(NavMeshAgent))]
public class IaEnemigo : MonoBehaviour
{
    // --- Máquina de estados ---
    // El enemigo siempre está en UNO de estos estados. Cada estado tiene su propio
    // método (Patrullar, Perseguir, Atacar) y decide cuándo cambiar a otro estado.
    public enum Estado { Patrulla, Persecucion, Ataque }

    public Estado estadoActual = Estado.Patrulla; // Visible en el Inspector para depurar.

    // Referencia al jugador que el enemigo va a perseguir.
    private Transform player;

    // Distancia máxima a la que el enemigo detecta al jugador (además necesita verlo).
    [SerializeField] float chaseRange = 15f;

    // Distancia mínima a la que el enemigo se quedará del jugador.
    // Esto evita que se "meta dentro" del jugador.
    private float stopDistance = 2f;

    // --- Patrulla ---
    [SerializeField] float radioPatrulla = 10f; // Radio (desde donde nació) en el que elige puntos al azar.
    [SerializeField] float tiempoEspera = 2f;   // Segundos parado al llegar a un punto antes de ir al siguiente.
    private Vector3 puntoOrigen;                // Posición donde nació el enemigo (centro de la patrulla).
    private float tiempoLlegada;                // Momento en que llegó al último punto de patrulla.
    private bool esperando;                     // true mientras está parado en un punto.

    // --- Ataque cuerpo a cuerpo simple ---
    private float danio = 10f;          // Vida que quita al player en cada ataque.
    private float cadenciaAtaque = 1.5f; // Segundos de espera entre ataque y ataque.
    private float proximoAtaque;         // Momento (Time.time) del siguiente ataque permitido.
    private PlayerHealth vidaPlayer;     // Vida del player, para poder hacerle daño.

    // Referencia al componente NavMeshAgent que controla el movimiento sobre el NavMesh.
    private NavMeshAgent agent;

    private void Awake()
    {
        // Obtenemos y guardamos la referencia al NavMeshAgent del mismo GameObject.
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        // Buscamos al player en la escena por su tag.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) player = playerObject.transform;

        // Configuramos la distancia de parada del NavMeshAgent
        // para que coincida con el valor de stopDistance.
        agent.stoppingDistance = stopDistance;

        // Buscamos tambien el componente de vida del player, para poder atacarlo.
        if (player != null) vidaPlayer = player.GetComponent<PlayerHealth>();

        // Guardamos dónde nace: patrullará alrededor de este punto.
        puntoOrigen = transform.position;
    }

    private void Update()
    {
        // Si no tenemos jugador asignado, no hacemos nada.
        if (player == null) return;

        // Si el NavMeshAgent no está activo o no está sobre el NavMesh, tampoco hacemos nada.
        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        // Según el estado en el que estemos, ejecutamos un comportamiento u otro.
        switch (estadoActual)
        {
            case Estado.Patrulla: Patrullar();
                break;
            case Estado.Persecucion: Perseguir();
                break;
            case Estado.Ataque: Atacar();
                break;
        }
    }

    // Cambia de estado. Tenerlo en un método permite hacer cosas al entrar en cada estado
    // (por ejemplo, limpiar el camino al volver a patrullar).
    private void CambiarEstado(Estado nuevo)
    {
        estadoActual = nuevo;

        if (nuevo == Estado.Patrulla)
        {
            agent.ResetPath();
            esperando = false;
        }
    }

    // ESTADO PATRULLA: va a puntos al azar cerca de su origen. Si ve al player, pasa a persecución.
    private void Patrullar()
    {
        if (VePlayer())
        {
            CambiarEstado(Estado.Persecucion);
            return;
        }

        // Si estamos parados en un punto, esperamos un rato antes de elegir el siguiente.
        if (esperando)
        {
            if (Time.time >= tiempoLlegada + tiempoEspera)
            {
                esperando = false;
                IrAPuntoAleatorio();
            }
            return;
        }

        // Si no tenemos destino o ya hemos llegado, nos paramos a esperar.
        if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance)
        {
            esperando = true;
            tiempoLlegada = Time.time;
        }
    }

    // ESTADO PERSECUCIÓN: sigue al player. Si se acerca lo suficiente ataca; si lo pierde, vuelve a patrullar.
    private void Perseguir()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Si el player se aleja bastante más que el rango (margen para no parpadear entre estados), lo perdemos.
        if (distance > chaseRange * 1.5f)
        {
            CambiarEstado(Estado.Patrulla);
            return;
        }

        // Actualizamos el destino del agente para que vaya hacia el jugador.
        agent.SetDestination(player.position);

        if (distance <= stopDistance + 0.5f)
        {
            CambiarEstado(Estado.Ataque);
        }
    }

    // ESTADO ATAQUE: golpea al player cada cierto tiempo. Si el player se aleja, vuelve a perseguirlo.
    private void Atacar()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stopDistance + 1f)
        {
            CambiarEstado(Estado.Persecucion);
            return;
        }

        // Nos giramos hacia el player (sin inclinarnos) para que el ataque tenga sentido visual.
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir != Vector3.zero) transform.rotation = Quaternion.LookRotation(dir);

        if (Time.time >= proximoAtaque)
        {
            if (vidaPlayer != null) vidaPlayer.TakeDamage(danio);
            proximoAtaque = Time.time + cadenciaAtaque;
        }
    }

    // Detección: el player está en rango Y hay línea de visión (un raycast sin paredes en medio).
    private bool VePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > chaseRange) return false;

        // Lanzamos un rayo desde el enemigo hacia el player. Si lo primero que toca es el player, lo vemos.
        Vector3 origen = transform.position + Vector3.up * 0.5f;
        Vector3 destino = player.position + Vector3.up * 0.5f;
        RaycastHit hit;
        if (Physics.Raycast(origen, destino - origen, out hit, chaseRange))
        {
            return hit.transform == player || hit.transform.IsChildOf(player);
        }
        return false;
    }

    // Elige un punto al azar sobre el NavMesh dentro de radioPatrulla y va hacia él.
    private void IrAPuntoAleatorio()
    {
        Vector3 aleatorio = puntoOrigen + Random.insideUnitSphere * radioPatrulla;
        NavMeshHit hit;

        // SamplePosition busca el punto del NavMesh más cercano al que hemos elegido.
        if (NavMesh.SamplePosition(aleatorio, out hit, radioPatrulla, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    // Dibuja en la Scene View el rango de detección y el radio de patrulla (solo con el enemigo seleccionado).
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioPatrulla);
    }
}
