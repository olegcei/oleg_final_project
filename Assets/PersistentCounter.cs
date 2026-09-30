using UnityEngine;

public class PersistentText : MonoBehaviour
{
    public static PersistentText Instance { get; private set; }

    [SerializeField] private GameObject textObject; // the TMP text child
    private const string Key = "ShowText";

    public static bool IsVisible => PlayerPrefs.GetInt(Key, 0) == 1;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        textObject.SetActive(IsVisible);
    }

    public void SetVisible(bool value)
    {
        textObject.SetActive(value);
        PlayerPrefs.SetInt(Key, value ? 1 : 0);
        PlayerPrefs.Save();
    }
}