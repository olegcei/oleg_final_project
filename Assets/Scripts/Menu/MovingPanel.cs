using UnityEngine;

/// <summary>
/// Attach this to a concrete panel/platform. Every few seconds it picks a new
/// random horizontal target position (within set bounds) and moves there smoothly.
/// The player must time a jump onto it correctly to proceed.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class MovingPanel : MonoBehaviour
{
    [Header("Movement Range")]
    [Tooltip("How far left/right (in world units) the panel can move from its starting position.")]
    [SerializeField] private float moveRange = 3f;

    [Header("Timing")]
    [Tooltip("Minimum seconds before picking a new target position.")]
    [SerializeField] private float minInterval = 2f;
    [Tooltip("Maximum seconds before picking a new target position.")]
    [SerializeField] private float maxInterval = 4f;

    [Header("Speed")]
    [Tooltip("How fast the panel moves toward its target position.")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Player Attachment")]
    [Tooltip("If true, the player will move with the panel when standing on it (via parenting).")]
    [SerializeField] private bool carryPlayer = true;
    [Tooltip("Tag used to identify the player.")]
    [SerializeField] private string playerTag = "Player";

    private Rigidbody _rb;
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _timer;
    private float _currentInterval;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true; // moved manually, not via physics forces
        _startPosition = transform.position;
        _targetPosition = _startPosition;
        PickNewInterval();
    }

    private void FixedUpdate()
    {
        _timer += Time.fixedDeltaTime;
        if (_timer >= _currentInterval)
        {
            PickNewTarget();
            _timer = 0f;
            PickNewInterval();
        }

        Vector3 newPos = Vector3.MoveTowards(_rb.position, _targetPosition, moveSpeed * Time.fixedDeltaTime);
        _rb.MovePosition(newPos);
    }

    private void PickNewTarget()
    {
        float offsetX = Random.Range(-moveRange, moveRange);
        _targetPosition = _startPosition + new Vector3(offsetX, 0f, 0f);
    }

    private void PickNewInterval()
    {
        _currentInterval = Random.Range(minInterval, maxInterval);
    }

    // --- Optional: carry the player along with the platform ---

    private void OnCollisionEnter(Collision collision)
    {
        if (!carryPlayer) return;

        if (collision.gameObject.CompareTag(playerTag))
        {
            // Only attach if the player landed on top of the panel
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    collision.transform.SetParent(transform);
                    break;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!carryPlayer) return;

        if (collision.gameObject.CompareTag(playerTag))
        {
            collision.transform.SetParent(null);
        }
    }

    // Visualize the movement range in the editor
    private void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? _startPosition : transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(center + Vector3.left * moveRange, center + Vector3.right * moveRange);
    }
}