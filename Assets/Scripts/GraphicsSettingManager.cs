using System;
using UnityEngine;
using UnityEngine.Rendering;

public class GraphicsSettingsManager : MonoBehaviour
{
    public enum Quality { Low = 0, High = 1 }

    public static GraphicsSettingsManager Instance { get; private set; }
    public static event Action<Quality> OnQualityChanged;

    [SerializeField] Volume globalVolume;
    [SerializeField] VolumeProfile lowProfile;
    [SerializeField] VolumeProfile highProfile;
    [SerializeField] Quality defaultQuality = Quality.High;

    const string PrefKey = "GraphicsQuality";

    public Quality Current { get; private set; }

    // Makes sure the manager exists no matter which scene you press Play from
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (Instance != null) return;

        var prefab = Resources.Load<GraphicsSettingsManager>("GraphicsSettingsManager");
        if (prefab == null)
        {
            Debug.LogError("GraphicsSettingsManager prefab not found in a Resources folder.");
            return;
        }
        Instantiate(prefab);
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Apply((Quality)PlayerPrefs.GetInt(PrefKey, (int)defaultQuality), false);
    }

    public void Apply(Quality quality, bool save = true)
    {
        Current = quality;

        int level = Mathf.Clamp((int)quality, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(level);

        if (globalVolume != null)
            globalVolume.sharedProfile = quality == Quality.High ? highProfile : lowProfile;

        if (save)
        {
            PlayerPrefs.SetInt(PrefKey, (int)quality);
            PlayerPrefs.Save();
        }

        OnQualityChanged?.Invoke(quality);
    }
}