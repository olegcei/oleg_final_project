using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


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

    [SerializeField]
    private string optionsSceneName = "options_pop_up";
    private bool optionsOpen;

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

        HandleInput();
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

    private void HandleInput()
    {
        // ...your existing input code...

        if (Keyboard.current != null && Keyboard.current.nKey.wasPressedThisFrame)
        {
            ToggleOptions();
        }
    }

    private void ToggleOptions()
    {
        if (!optionsOpen)
        {
            SceneManager.LoadScene(optionsSceneName, LoadSceneMode.Additive);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            optionsOpen = true;
        }
        else
        {
            SceneManager.UnloadSceneAsync(optionsSceneName);
            Time.timeScale = 1f;
            // re-lock the cursor here if your game uses a locked cursor
            optionsOpen = false;
        }
    }

    void LateUpdate()
    {
        if (Time.timeScale == 0f)
            Debug.Log($"lock={Cursor.lockState} visible={Cursor.visible}");
    }


}