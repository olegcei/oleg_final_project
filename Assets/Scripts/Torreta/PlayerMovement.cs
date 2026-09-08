using UnityEngine;

// Obliga a que el objeto tenga un CharacterController (Unity lo añade solo).
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 6f;      // Velocidad de desplazamiento en metros por segundo.
    public float gravity = -20f;      // Gravedad. Negativa porque tira hacia abajo.
    public float rotationSpeed = 12f; // Lo rapido que el personaje gira hacia su direccion.

    [Header("Referencias")]
    public Transform cameraTransform; // Camara que marca la orientacion del movimiento.

    private CharacterController controller; // Componente que mueve al personaje.
    private float verticalSpeed;            // Velocidad de caida acumulada.

    private void Awake()
    {
        // Guardamos el CharacterController una sola vez para no buscarlo cada frame.
        controller = GetComponent<CharacterController>();

        // Si no hemos arrastrado una camara al Inspector, usamos la camara principal.
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    // Update se ejecuta una vez por frame: aqui leemos el mando/teclado y movemos.
    private void Update()
    {
        // Valores de -1, 0 o 1 segun las teclas A/D y W/S (o las flechas).
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Direccion en la que queremos ir. La Y va a 0 porque no volamos.
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical);

        // Si hay camara, adaptamos la direccion a como esta mirando.
        if (cameraTransform != null)
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            // Aplanamos los dos vectores para que la camara no nos hunda ni nos eleve.
            cameraForward.y = 0f;
            cameraRight.y = 0f;

            moveDirection = cameraForward.normalized * vertical + cameraRight.normalized * horizontal;
        }

        // normalized deja el vector con longitud 1: asi en diagonal no se va mas rapido.
        moveDirection = moveDirection.normalized;

        // Movimiento horizontal.
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Si nos estamos moviendo, giramos poco a poco hacia esa direccion.
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Gravedad: en el suelo reiniciamos la caida con un valor pequeño
        // para que el personaje se mantenga pegado al terreno.
        if (controller.isGrounded)
        {
            verticalSpeed = -2f;
        }

        verticalSpeed = verticalSpeed + gravity * Time.deltaTime;

        // Movimiento vertical (la caida), en un segundo Move mas facil de leer.
        controller.Move(new Vector3(0f, verticalSpeed, 0f) * Time.deltaTime);
    }
}
