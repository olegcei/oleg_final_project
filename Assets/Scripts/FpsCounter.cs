using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text fpsText;      // Text that displays the FPS
    [SerializeField] private Button toggleButton;   // Button that toggles the counter
    [SerializeField] private TMP_Text buttonLabel;  // Optional: label on the button

    [Header("Settings")]
    [SerializeField] private float updateInterval = 0.5f; // Seconds between text refreshes
    [SerializeField] private bool startActive = true;

    private bool isActive;
    private float accumulatedTime;
    private int frameCount;

    private void Awake()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(Toggle);
    }

    private void Start()
    {
        SetActive(startActive);
    }

    private void OnDestroy()
    {
        if (toggleButton != null)
            toggleButton.onClick.RemoveListener(Toggle);
    }

    private void Update()
    {
        if (!isActive) return;

        // Unscaled so the counter stays accurate when Time.timeScale is 0 or changed
        accumulatedTime += Time.unscaledDeltaTime;
        frameCount++;

        if (accumulatedTime >= updateInterval)
        {
            float fps = frameCount / accumulatedTime;
            float ms = 1000f * accumulatedTime / frameCount;
            fpsText.text = $"{fps:0} FPS\n{ms:0.0} ms";

            accumulatedTime = 0f;
            frameCount = 0;
        }
    }

    public void Toggle()
    {
        SetActive(!isActive);
    }

    public void SetActive(bool value)
    {
        isActive = value;

        if (fpsText != null)
            fpsText.gameObject.SetActive(isActive);

        if (buttonLabel != null)
            buttonLabel.text = isActive ? "FPS: ON" : "FPS: OFF";

        // Reset the averaging window so the first reading after re-enabling is clean
        accumulatedTime = 0f;
        frameCount = 0;
    }
}