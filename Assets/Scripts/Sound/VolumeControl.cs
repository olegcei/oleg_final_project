using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioSource musicSource; // or grab it via MusicPlayer.instance

    void Start()
    {
        // If MusicPlayer is a singleton, get its AudioSource instead of assigning in inspector
        if (musicSource == null)
            musicSource = MusicPlayer.instance.GetComponent<AudioSource>();

        // Sync slider to current volume when the scene opens
        volumeSlider.value = musicSource.volume;

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private void SetVolume(float value)
    {
        musicSource.volume = value;
    }
}