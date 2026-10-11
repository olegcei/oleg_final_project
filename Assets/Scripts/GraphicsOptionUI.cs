using TMPro;
using UnityEngine;

public class GraphicsOptionUI : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropdown;

    void OnEnable()
    {
        var manager = GraphicsSettingsManager.Instance;
        if (manager != null)
            dropdown.SetValueWithoutNotify((int)manager.Current);

        dropdown.onValueChanged.AddListener(OnChanged);
    }

    void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(OnChanged);
    }

    void OnChanged(int index)
    {
        GraphicsSettingsManager.Instance.Apply((GraphicsSettingsManager.Quality)index);
    }
}