using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VfxMixerControl: MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixer SFX;                 // drag your Mixer asset here
    [SerializeField] private string exposedParameter = "SfxVolume"; // name of the exposed parameter

    void Start()
    {
        // Sync slider to the mixer's current volume when the scene opens
        if (SFX.GetFloat(exposedParameter, out float db))
            volumeSlider.value = Mathf.Pow(10f, db / 20f);

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    void OnDestroy()
    {
        volumeSlider.onValueChanged.RemoveListener(SetVolume);
    }

    private void SetVolume(float value)
    {
        // Slider 0..1 -> decibels (-80 dB to 0 dB)
        float db = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        SFX.SetFloat(exposedParameter, db);
    }
}