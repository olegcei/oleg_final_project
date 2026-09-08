using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class EntradasInput : MonoBehaviour
{
    public event Action OnHookshotPressed;
    public event Action OnHookshotReleased;

    public void Hookshot(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            OnHookshotPressed?.Invoke();
        else if (ctx.canceled)
            OnHookshotReleased?.Invoke();
    }
}