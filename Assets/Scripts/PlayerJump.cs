using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] float gravity;      // negative value
    [SerializeField] float jumpHeight;
    [SerializeField] EntradasInput input; // drag in the same GameObject's EntradasInput

    public Vector3 velocity;

    float coyoteTime = 0.1f;
    float coyoteTimeCounter;
    public float jumpBufferTime = 0.1f;
    float jumpBufferCounter;

    int jumpsMax = 1;
    int jumpsCount;
    public bool isJumping;
    bool isGrounded;

    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = characterController.isGrounded;

        // Buffer the jump the moment the button is pressed
        if (input.salto)
        {
            jumpBufferCounter = jumpBufferTime;
            input.salto = false; // consume it so it only buffers once per press
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Gravity
        if (velocity.y > -50f) velocity.y += gravity * Time.deltaTime;

        if (isGrounded && velocity.y < 0f)
        {
            coyoteTimeCounter = coyoteTime;
            jumpsCount = jumpsMax;
            isJumping = false;
            velocity.y = -5.8f; // stick to ground
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump resolution
        if (jumpBufferCounter > 0f)
        {
            if (!isJumping && isGrounded)
                Salto();
            else if (!isJumping && !isGrounded && coyoteTimeCounter > 0f)
                Salto();
            else if (isJumping && jumpsCount > 0)
                Salto();
        }

        // Apply movement — this was missing before
        characterController.Move(velocity * Time.deltaTime);
    }

    void Salto()
    {
        isJumping = true;
        jumpsCount--;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        jumpBufferCounter = 0f;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y > 0.5f && velocity.y <= 0f)
        {
            isJumping = false;
        }
    }
}