using UnityEngine;

public class Hookshot : MonoBehaviour
{
    [Header("References")]
    public Transform gunTip;
    public Transform camTransform;
    public LineRenderer lineRenderer;
    public CharacterController controller; // CHANGED - no more Rigidbody
    public PlayerMovement playerMovement;  // NEW - to disable it while hooking
    public EntradasInput input;

    [Header("Settings")]
    public float maxHookDistance = 25f;
    public float pullSpeed = 15f;
    public float minDistanceToDetach = 1.5f;
    public string hookableTag = "Hookable";
    public LayerMask hookableMask;

    private Vector3 hookPoint;
    private bool isHooking = false;

    void OnEnable()
    {
        input.OnHookshotPressed += TryStartHook;
        input.OnHookshotReleased += StopHook;
        Debug.Log("Hookshot subscribed to input events");
    }

    void OnDisable()
    {
        input.OnHookshotPressed -= TryStartHook;
        input.OnHookshotReleased -= StopHook;
    }

    void Update()
    {
        if (isHooking)
        {
            DrawRope();

            float dist = Vector3.Distance(transform.position, hookPoint);
            if (dist < minDistanceToDetach)
            {
                StopHook();
                return;
            }

            Vector3 direction = (hookPoint - transform.position).normalized;
            controller.Move(direction * pullSpeed * Time.deltaTime);
        }
    }

    void TryStartHook()
    {
        Debug.Log("TryStartHook called");
        RaycastHit hit;
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, maxHookDistance, hookableMask))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);
            if (hit.collider.CompareTag(hookableTag))
            {
                Debug.Log("Hook successful!");
                hookPoint = hit.point;
                isHooking = true;
                lineRenderer.enabled = true;
                playerMovement.enabled = false; // NEW - stop normal movement fighting the pull
            }
        }
    }

    void StopHook()
    {
        isHooking = false;
        lineRenderer.enabled = false;
        playerMovement.enabled = true; // NEW - give control back
    }

    void DrawRope()
    {
        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, hookPoint);
    }
}