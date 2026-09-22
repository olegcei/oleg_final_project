using UnityEngine;
using UnityEngine.AI;

// Este script permite que un enemigo se mueva a un punto del suelo
// cuando hacemos clic con el botón izquierdo del ratón.
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyClickMove : MonoBehaviour
{
    // Cámara desde la que lanzaremos el rayo.
    // Si no se asigna en el Inspector, intentaremos usar Camera.main.
    [SerializeField] private Camera mainCamera;

    // Capa del suelo para que el raycast solo detecte el plano.
    // Es recomendable crear una layer llamada "Ground".
    [SerializeField] private LayerMask groundLayer;

    // Distancia máxima del raycast.
    [SerializeField] private float rayDistance = 200f;

    // Referencia al NavMeshAgent del enemigo.
    private NavMeshAgent agent;

    // Distancia de referencia a la que consideramos que el tamaño actual es "correcto".
    private float referenceDistance;

    // Escala original del objeto en ese momento de referencia.
    private Vector3 referenceScale;

    private void Awake()
    {
        // Guardamos la referencia al NavMeshAgent.
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        mainCamera = GameObject.FindAnyObjectByType<Camera>();

        // Si no hemos asignado cámara manualmente, usamos la cámara principal.
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Guardamos la escala inicial del enemigo.
        referenceScale = transform.localScale;

        // Guardamos la distancia inicial entre cámara y enemigo.
        // Esa será nuestra distancia de referencia.
        referenceDistance = Vector3.Distance(mainCamera.transform.position, transform.position);
    }

    private void Update()
    {
        // Si no hay cámara, no podemos lanzar el rayo.
        if (mainCamera == null) return;

        // Si el agente no está activo o no está colocado sobre el NavMesh, salimos.
        if (!agent.isActiveAndEnabled) return;
        if (!agent.isOnNavMesh) return;

        // Detectamos clic izquierdo del ratón.
        if (Input.GetMouseButtonDown(0))
        {
            // Creamos un rayo desde la cámara hacia la posición del ratón en pantalla.
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // Variable donde guardaremos la información del impacto.
            RaycastHit hit;

            // Lanzamos el raycast contra la capa del suelo.
            if (Physics.Raycast(ray, out hit, rayDistance, groundLayer))
            {
                // Mandamos al NavMeshAgent al punto exacto donde hemos hecho clic.
                agent.SetDestination(hit.point);
            }
        }
        MantenerTamanoAparente();
    }

    /// <summary>
    /// Escala el objeto en función de su distancia a la cámara
    /// para que aparente mantener el mismo tamaño en pantalla.
    /// </summary>
    private void MantenerTamanoAparente()
    {
        // Si no hay cámara, no hacemos nada.
        if (mainCamera == null) return;

        // Calculamos la distancia actual entre la cámara y el enemigo.
        float currentDistance = Vector3.Distance(mainCamera.transform.position, transform.position);

        // Evitamos divisiones raras o escalas inválidas.
        if (referenceDistance <= 0.001f) return;

        // Calculamos el factor de escala:
        // - Si el objeto está más lejos, crecerá.
        // - Si está más cerca, se hará más pequeño.
        float scaleFactor = currentDistance / referenceDistance;

        // Aplicamos la nueva escala respetando la proporción original.
        transform.localScale = referenceScale * scaleFactor;
    }
}