using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class FpsCounter : MonoBehaviour
{
    [SerializeField] private float updateInterval = 0.5f;

    private TMP_Text label;
    private float elapsed;
    private int frames;

    private void Awake() => label = GetComponent<TMP_Text>();

    private void Update()
    {
        frames++;
        elapsed += Time.unscaledDeltaTime;

        if (elapsed >= updateInterval)
        {
            label.text = Mathf.RoundToInt(frames / elapsed) + " FPS";
            frames = 0;
            elapsed = 0f;
        }
    }
}