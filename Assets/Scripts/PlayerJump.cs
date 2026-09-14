using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] float gravity;   // negativa
    [SerializeField] float jumpHeight;  // altura que quieres alcanzar
    public Vector3 velocity; // solo usaremos y para salto/gravedad

    float coyoteTime = 0.1f;
    public float coyoteTimeCounter;
    public float jumpBufferTime = 0.1f;
    public float jumpBufferCounter;
    int jumpsMax = 1;
    int jumpsCount;
    public bool isJumping;
    public bool jumpCancel;
    bool isGrounded;

    CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        //playerDashScript = GetComponent<PlayerDash>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Detectar suelo en CharacterController
        isGrounded = characterController.isGrounded;

        // Aplicar gravedad acumulada
        if (velocity.y > -50) velocity.y += gravity * Time.deltaTime;

        if (isGrounded && velocity.y < 0)
        {
            coyoteTimeCounter = coyoteTime;
            jumpsCount = jumpsMax;
            isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        //FixedUpdate para RB
        if (jumpBufferCounter > 0) //El buffer del salto controla si se salta o no
        {
            if (!isJumping && isGrounded) //Primer salto desde el suelo
            {
                Salto();
            }
            else
                if (!isJumping && !isGrounded) //Salto desde el precipicio
                {
                    if (coyoteTimeCounter > 0f)
                    {
                        Salto();
                    }
                }
                else
                    if (isJumping && jumpsCount > 0)//Saltos a partir del primero //&& (!playerControl.ComprobacionSuelo() || !playerControl.isColliderTopLadder())//
                    {
                        Salto();
                    }
        }

        //Cancela el salto al soltar la tecla o boton
        if (jumpCancel && !isGrounded)
        {
            if (velocity.y > 0 && (jumpBufferCounter + 0.1f) < 0)
            {
                velocity.y = 0f;
                jumpCancel = false;
            }
        }

        // Mantener pegado al suelo
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -5.8f;
        }

        if (jumpBufferCounter > -1) jumpBufferCounter -= Time.deltaTime; //Comprobacion de seguridad para no desbordar la variable
    }

    void Salto()
    {
        //if (!playerDashScript.isDashJump)
        //{
        isJumping = true;
        jumpsCount--;
        velocity.y = 0f;

        // v = sqrt(2 * h * -g)
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        jumpBufferCounter = 0;
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGrounded && velocity.y > 0f)
        {
            isJumping = false;
            velocity.y = 0f;
        }
    }
}
