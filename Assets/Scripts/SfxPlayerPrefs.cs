using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SfxControl : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;   // leave empty on scenes with no slider
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string exposedParameter = "SfxVolume";
    [SerializeField] private string prefKey = "SfxVolume";

    void Start()
    {
        float saved = PlayerPrefs.GetFloat(prefKey, 0.75f);
        ApplyVolume(saved);

        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(saved);
            volumeSlider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    void OnDestroy()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        ApplyVolume(value);
        PlayerPrefs.SetFloat(prefKey, value);
    }

    private void ApplyVolume(float value)
    {
        float db = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        if (!audioMixer.SetFloat(exposedParameter, db))
            Debug.LogWarning("Parameter '" + exposedParameter + "' not found.");
    }
}