using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class TextToggle : MonoBehaviour
{
    private void Start()
    {
        var toggle = GetComponent<Toggle>();
        toggle.SetIsOnWithoutNotify(PersistentText.IsVisible);
        toggle.onValueChanged.AddListener(v =>
        {
            if (PersistentText.Instance != null)
                PersistentText.Instance.SetVisible(v);
        });
    }
}