using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections;


public class EntradasInput : MonoBehaviour
{
    [Header("References")]
    public GameObject grenadePrefab;
    public Transform throwPoint;

    private Animator animator;


    private PlayerInput playerInput;
    public Vector2 inputMovimiento;
    public Vector2 inputCamara;
    private PlayerMovement _movimientoPersonaje;
    private SaltoyGravedadPlayer _saltoyGravedadPlayer;

    [Header("Throw Settings")]
    public float forwardForce = 15f;
    public float upwardForce = 3f;

    public event UnityAction OnHookshotPressed;
    public event UnityAction OnHookshotReleased;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        _movimientoPersonaje = GetComponent<PlayerMovement>();
        _saltoyGravedadPlayer = GetComponent<SaltoyGravedadPlayer>();
        
    }

    private void Update()
    {
        //Lectura de entrada de los input para mover al player y girar la camara
        inputMovimiento = playerInput.actions["Move"].ReadValue<Vector2>();
        inputCamara = playerInput.actions["Look"].ReadValue<Vector2>();
    }


    public void Hookshot(InputAction.CallbackContext ctx)
    {
        Debug.Log("Hookshot input received: " + ctx.phase);

        if (ctx.started)
            OnHookshotPressed?.Invoke();
        else if (ctx.canceled)
            OnHookshotReleased?.Invoke();
    }

    public void Grenade(InputAction.CallbackContext ctx)
    {
        Debug.Log("Grenade input received: " + ctx.phase);

        if (ctx.started)
            ThrowGrenade();
    }

    public void ThrowGrenade()
    {
        StartCoroutine(ThrowGrenadeAfterDelay(1f));
    }

    private IEnumerator ThrowGrenadeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        Vector3 throwDirection = throwPoint.forward * forwardForce + throwPoint.up * upwardForce;
        rb.AddForce(throwDirection, ForceMode.VelocityChange);
    }

    public void InputCorrer(InputAction.CallbackContext callbackContext)
    {
        switch (callbackContext.phase)
        {
            case InputActionPhase.Started:
                _movimientoPersonaje.multiplicadorAlCorrer = 2f;
                _movimientoPersonaje.CalcularVelocidad();
                break;
            case InputActionPhase.Canceled:
                _movimientoPersonaje.multiplicadorAlCorrer = 1f;
                _movimientoPersonaje.CalcularVelocidad();
                break;

        }
    }

    public void InputSalto(InputAction.CallbackContext callbackContext)
    {
        switch (callbackContext.phase)
        {
            case InputActionPhase.Started:
                _saltoyGravedadPlayer.jumpBufferCounter = _saltoyGravedadPlayer.jumpBufferTime;
                _saltoyGravedadPlayer.jumpCancel = false;
                break;
            case InputActionPhase.Canceled:
                _saltoyGravedadPlayer.jumpCancel = true;
                _saltoyGravedadPlayer.coyoteTimeCounter = 0f;
                break;
        }
    }

}