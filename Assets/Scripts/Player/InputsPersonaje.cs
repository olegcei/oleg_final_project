using UnityEngine;
using UnityEngine.InputSystem;

public class InputsPersonaje : MonoBehaviour
{
    private PlayerInput _inputJugador;
    public Vector2 inputMovimiento;
    public Vector2 inputCamara;

    private MovPersonaje _movimientoPersonajeCha;
    private SaltoyGravedadPlayer _saltoyGravedadPlayer;


    private void Awake()
    {
        _inputJugador = GetComponent<PlayerInput>();
        _movimientoPersonajeCha = GetComponent<MovPersonaje>();
        _saltoyGravedadPlayer = GetComponent<SaltoyGravedadPlayer>();
    }

    private void Update()
    {
        //Lectura de entrada de los input para mover al player y girar la camara
        inputMovimiento = _inputJugador.actions["Mover"].ReadValue<Vector2>();
        inputCamara = _inputJugador.actions["Girar"].ReadValue<Vector2>();
    }

    //Accion imput salto
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

    //Accion input correr
    public void InputCorrer(InputAction.CallbackContext callbackContext)
    {
        switch (callbackContext.phase)
        {
            case InputActionPhase.Started:
                _movimientoPersonajeCha.multiplicadorAlCorrer = 2f;
                _movimientoPersonajeCha.CalcularVelocidad();
                break;
            case InputActionPhase.Canceled:
                _movimientoPersonajeCha.multiplicadorAlCorrer = 1f;
                _movimientoPersonajeCha.CalcularVelocidad();
                break;
        }
    }
}