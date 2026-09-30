using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    private const string PrefKey = "FpsCounterEnabled";
    private static FpsCounter instance;

    [Header("UI References")]
    [SerializeField] private Canvas canvas;         // The canvas this script sits on
    [SerializeField] private TMP_Text fpsText;
    [SerializeField] private Toggle toggle;
    [SerializeField] private TMP_Text toggleLabel;  // Optional

    [Header("Settings")]
    [SerializeField] private float updateInterval = 0.5f;

    [Header("World Space canvas only")]
    [SerializeField] private bool followCamera = false;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 0f, 1f);

    private bool isActive;
    private float accumulatedTime;
    private int frameCount;

    private void Awake()
    {
        // If a copy already exists (e.g. the prefab is also placed in another scene), remove this one
        if (instance != null && instance != this)
        {
            Destroy(transform.root.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(transform.root.gameObject); // must be a root object

        if (canvas == null) canvas = GetComponentInParent<Canvas>();

        if (toggle != null)
            toggle.onValueChanged.AddListener(OnToggleChanged);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (instance != this) return;

        bool defaultState = toggle != null && toggle.isOn;
        bool saved = PlayerPrefs.GetInt(PrefKey, defaultState ? 1 : 0) == 1;

        if (toggle != null)
            toggle.SetIsOnWithoutNotify(saved);

        Apply(saved);
        RefreshCamera();
    }

    private void OnDestroy()
    {
        if (instance != this) return;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (toggle != null)
            toggle.onValueChanged.RemoveListener(OnToggleChanged);
        instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshCamera();

        if (EventSystem.current == null)
            Debug.LogWarning("FpsCounter: no EventSystem in this scene, the toggle won't respond to clicks.");
    }

    private void RefreshCamera()
    {
        if (canvas == null) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        // Only needed for World Space / Screen Space - Camera canvases
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            canvas.worldCamera = cam;
    }

    private void LateUpdate()
    {
        if (instance != this) return;

        if (followCamera && canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            Camera cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
            if (cam != null)
                transform.root.SetPositionAndRotation(
                    cam.transform.TransformPoint(cameraOffset),
                    cam.transform.rotation);
        }
    }

    private void Update()
    {
        if (instance != this || !isActive) return;

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

    private void OnToggleChanged(bool value)
    {
        PlayerPrefs.SetInt(PrefKey, value ? 1 : 0);
        Apply(value);
    }

    private void Apply(bool value)
    {
        isActive = value;

        if (fpsText != null)
            fpsText.gameObject.SetActive(isActive);

        if (toggleLabel != null)
            toggleLabel.text = isActive ? "FPS: ON" : "FPS: OFF";

        accumulatedTime = 0f;
        frameCount = 0;
    }
}