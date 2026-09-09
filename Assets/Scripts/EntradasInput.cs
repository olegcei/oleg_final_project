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
}