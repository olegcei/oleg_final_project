using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fades in a blood splatter UI Image on a Canvas as the player takes damage.
/// Opacity increases each time health drops, and optionally "pulses" briefly
/// on hit before settling to its new baseline.
/// 
/// SETUP:
/// 1. Put your blood splatter Image as a full-screen UI Image on your Canvas
///    (Image type: Simple, color alpha at 0 to start).
/// 2. Drag that Image into the "bloodSplatterImage" field below.
/// 3. Attach this script to any GameObject (e.g. the Canvas itself).
/// 4. Call UpdateHealth(currentHealth, maxHealth) from your PlayerHealth script
///    every time damage is taken (see hook example at bottom of this file).
/// </summary>
public class BloodSplatterHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image bloodSplatterImage;

    [Header("Opacity Settings")]
    [Tooltip("Opacity when at full health (usually 0)")]
    [SerializeField] private float minAlpha = 0f;

    [Tooltip("Opacity when at 0 health (usually near 1)")]
    [SerializeField] private float maxAlpha = 0.85f;

    [Tooltip("Curve controlling how opacity ramps as health drops. " +
             "Default (linear) is fine, but an EaseInOut curve makes low health feel more intense.")]
    [SerializeField] private AnimationCurve opacityCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Hit Pulse (optional)")]
    [Tooltip("Briefly flash extra opacity on the frame damage is taken, then settle down.")]
    [SerializeField] private bool pulseOnHit = true;
    [SerializeField] private float pulseAmount = 0.15f;
    [SerializeField] private float pulseDecaySpeed = 2f;

    private float _baselineAlpha;
    private float _pulseAlpha;
    private float _lastKnownHealth = -1f;

    private void Awake()
    {
        if (bloodSplatterImage == null)
        {
            Debug.LogError("BloodSplatterHealthUI: bloodSplatterImage not assigned.");
            return;
        }

        SetAlpha(minAlpha);
    }

    private void Update()
    {
        if (_pulseAlpha > 0f)
        {
            _pulseAlpha = Mathf.MoveTowards(_pulseAlpha, 0f, pulseDecaySpeed * Time.deltaTime);
            ApplyAlpha();
        }
    }

    /// <summary>
    /// Call this every time health changes (ideally from your damage event).
    /// </summary>
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (bloodSplatterImage == null) return;

        float healthPct = Mathf.Clamp01(currentHealth / maxHealth);
        float damagePct = 1f - healthPct; // 0 = full health, 1 = dead

        float curved = opacityCurve.Evaluate(damagePct);
        _baselineAlpha = Mathf.Lerp(minAlpha, maxAlpha, curved);

        // Trigger a pulse if health actually went down (not on heals)
        if (pulseOnHit && _lastKnownHealth >= 0f && currentHealth < _lastKnownHealth)
        {
            _pulseAlpha = pulseAmount;
        }

        _lastKnownHealth = currentHealth;
        ApplyAlpha();
    }

    private void ApplyAlpha()
    {
        SetAlpha(Mathf.Clamp01(_baselineAlpha + _pulseAlpha));
    }

    private void SetAlpha(float alpha)
    {
        Color c = bloodSplatterImage.color;
        c.a = alpha;
        bloodSplatterImage.color = c;
    }
}

/*
================================
HOOKING THIS UP TO YOUR HEALTH SCRIPT
================================

Option A — If your PlayerHealth script has a method like TakeDamage(int amount):

    public class PlayerHealth : MonoBehaviour
    {
        public int currentHealth = 100;
        public int maxHealth = 100;

        [SerializeField] private BloodSplatterHealthUI bloodUI;

        public void TakeDamage(int amount)
        {
            currentHealth = Mathf.Max(0, currentHealth - amount);
            bloodUI.UpdateHealth(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                // handle death
            }
        }
    }

Option B — If you'd rather not add a direct reference, use a UnityEvent or
C# event instead:

    // In PlayerHealth.cs
    public event System.Action<int, int> OnHealthChanged;

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // In BloodSplatterHealthUI.cs, subscribe in Awake/OnEnable:
    private void OnEnable()
    {
        FindObjectOfType<PlayerHealth>().OnHealthChanged += UpdateHealth;
    }

Option C — Bullet script hitting the player:
Your machine gun bullet script presumably calls something like
playerHealth.TakeDamage(damageAmount) on collision. As long as TakeDamage
calls UpdateHealth (Option A or B), the splatter reacts automatically —
no changes needed on the bullet side.
*/