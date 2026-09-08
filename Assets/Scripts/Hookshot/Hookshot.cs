using UnityEngine;

public class Hookshot : MonoBehaviour
{
    [Header("References")]
    public Transform gunTip;        // where the rope visually starts
    public Transform camTransform;  // player camera
    public LineRenderer lineRenderer;
    public Rigidbody playerRb;
    public EntradasInput input;     // NEW - reference to your input script

    [Header("Settings")]
    public float maxHookDistance = 25f;
    public float pullForce = 30f;
    public float maxPullSpeed = 20f;
    public float minDistanceToDetach = 1.5f;
    public string hookableTag = "Hookable";
    public LayerMask hookableMask; // optional, use instead of/with tag

    private Vector3 hookPoint;
    private bool isHooking = false;

    void Awake()
    {
        // Prevents the spinning issue from earlier
        playerRb.freezeRotation = true;
        playerRb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void OnEnable()
    {
        input.OnHookshotPressed += TryStartHook;
        input.OnHookshotReleased += StopHook;
    }

    void OnDisable()
    {
        input.OnHookshotPressed -= TryStartHook;
        input.OnHookshotReleased -= StopHook;
    }

    void Update()
    {
        // Auto-detach when close enough, still needs to run every frame
        if (isHooking && Vector3.Distance(transform.position, hookPoint) < minDistanceToDetach)
        {
            StopHook();
        }

        if (isHooking)
        {
            DrawRope();
        }
    }

    void FixedUpdate()
    {
        if (isHooking)
        {
            PullPlayer();
        }
    }

    void TryStartHook()
    {
        RaycastHit hit;
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, maxHookDistance, hookableMask))
        {
            if (hit.collider.CompareTag(hookableTag))
            {
                hookPoint = hit.point;
                isHooking = true;
                lineRenderer.enabled = true;
            }
        }
    }

    void PullPlayer()
    {
        Vector3 direction = (hookPoint - playerRb.position).normalized;

        // Add force towards hook point
        playerRb.AddForce(direction * pullForce, ForceMode.Acceleration);

        // Clamp speed so it doesn't get absurd
        if (playerRb.linearVelocity.magnitude > maxPullSpeed)
        {
            playerRb.linearVelocity = playerRb.linearVelocity.normalized * maxPullSpeed;
        }
    }

    void StopHook()
    {
        isHooking = false;
        lineRenderer.enabled = false;
    }

    void DrawRope()
    {
        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, hookPoint);
    }
}