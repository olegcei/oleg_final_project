using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class EntradasInput : MonoBehaviour
{
    [Header("References")]
    public GameObject grenadePrefab;
    public Transform throwPoint;

    [Header("Throw Settings")]
    public float forwardForce = 15f;
    public float upwardForce = 3f;

    public event Action OnHookshotPressed;
    public event Action OnHookshotReleased;

    public bool salto;

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
        GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        Vector3 throwDirection = throwPoint.forward * forwardForce + throwPoint.up * upwardForce;
        rb.AddForce(throwDirection, ForceMode.VelocityChange);
    }

    public void Salto(InputAction.CallbackContext callbackContext)
    {
        //Si el callbackContext es performed, almacenamos el valor de salto como true
        if (callbackContext.performed)
        {
            //Almacenamos el valor de salto como true
            salto = true;
        }
        else
            //Si el callbackContext es canceled, almacenamos el valor de salto como false
            if (callbackContext.canceled)
            {
                //Almacenamos el valor de salto como false
                salto = false;
            }
    }

}