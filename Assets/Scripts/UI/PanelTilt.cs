using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class BackgroundTilt : MonoBehaviour
{
    public float maxTiltX = 15f;   // deliberately large for testing
    public float maxTiltY = 15f;
    public float smoothing = 8f;
    public bool invert = false;    // tick if the tilt goes the wrong way
    public bool debugLog = true;

    Quaternion startRot;
    Vector2 current;

    void Start()
    {
        startRot = transform.localRotation;
        Debug.Log("BackgroundTilt running on " + name, this);
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current == null) return;
        Vector2 mouse = Mouse.current.position.ReadValue();
#else
        Vector2 mouse = Input.mousePosition;
#endif
        Vector2 offset = new Vector2(
            (mouse.x - Screen.width * 0.5f) / (Screen.width * 0.5f),
            (mouse.y - Screen.height * 0.5f) / (Screen.height * 0.5f));
        offset = Vector2.ClampMagnitude(offset, 1f);
        if (invert) offset = -offset;

        current = Vector2.Lerp(current, offset, 1f - Mathf.Exp(-smoothing * Time.unscaledDeltaTime));
        transform.localRotation = startRot * Quaternion.Euler(-current.y * maxTiltX, current.x * maxTiltY, 0f);

        if (debugLog) Debug.Log($"mouse {mouse} offset {offset}");
    }
}