using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class FullScreenEffectController : MonoBehaviour
{
    public static FullScreenEffectController instance;

    [SerializeField] private ScriptableRendererData rendererData;
    [SerializeField] private string featureName = "FullScreenPassRendererFeature";
    [SerializeField] private string[] scenesWithEffect = { "MenuPrincipal", "Options"};

    private ScriptableRendererFeature feature;

    void Awake()
    {
        

        if (instance != null)
        {
            Debug.Log("[FSEC] Duplicate, destroying");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        feature = rendererData.rendererFeatures.Find(f => f is FullScreenPassRendererFeature);
        if (feature == null)
        {
            Debug.LogError($"[FSEC] Feature '{featureName}' NOT found in {rendererData.name}");
            return;
        }

        
        SceneManager.sceneLoaded += OnSceneLoaded;
        Apply(SceneManager.GetActiveScene());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Apply(scene);
    }

    void Apply(Scene scene)
    {
        bool enable = System.Array.IndexOf(scenesWithEffect, scene.name) >= 0;
        feature.SetActive(enable);
        
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnApplicationQuit()
    {
        // The feature is an asset, so its state would otherwise persist in the editor
        if (feature != null) feature.SetActive(true);
    }
}