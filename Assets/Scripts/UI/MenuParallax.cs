using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    [System.Serializable]
    public class Layer
    {
        public Transform transform;
        public float moveAmount = 20f;   // units (pixels on a screen-space canvas, meters in world space)
        public float tiltAmount = 3f;    // degrees
        [HideInInspector] public Vector3 startPos;
        [HideInInspector] public Quaternion startRot;
    }

    public Layer[] layers;
    public float smoothing = 6f;
    public bool invert = false;   // flip to make layers move away from the cursor

    Vector2 current;

    void Start()
    {
        foreach (var l in layers)
        {
            l.startPos = l.transform.localPosition;
            l.startRot = l.transform.localRotation;
        }
    }

    void Update()
    {
        // Old Input Manager:
        Vector2 mouse = Input.mousePosition;
        // New Input System instead: Vector2 mouse = UnityEngine.InputSystem.Mouse.current.position.ReadValue();

        Vector2 target = new Vector2(
            (mouse.x / Screen.width - 0.5f) * 2f,
            (mouse.y / Screen.height - 0.5f) * 2f);
        target = Vector2.ClampMagnitude(target, 1f);
        if (invert) target = -target;

        // Frame-rate independent smoothing; unscaled so it works while paused
        current = Vector2.Lerp(current, target, 1f - Mathf.Exp(-smoothing * Time.unscaledDeltaTime));

        foreach (var l in layers)
        {
            l.transform.localPosition = l.startPos + new Vector3(current.x, current.y, 0f) * l.moveAmount;

            // Mouse right -> rotate around Y, mouse up -> rotate around X (negated so it leans toward the cursor)
            Quaternion tilt = Quaternion.Euler(-current.y * l.tiltAmount, current.x * l.tiltAmount, 0f);
            l.transform.localRotation = l.startRot * tilt;
        }
    }
}