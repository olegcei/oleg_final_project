using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Image))]
public class PanelHoverHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private float fadeDuration = 0.15f;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private float movementThreshold = 3f;
    [SerializeField] private string sceneToLoad;

    private static bool isFirstSceneLoad = true;

    private CanvasGroup canvasGroup;
    private Coroutine fadeRoutine;
    private Vector2 initialMousePos;
    private bool suppressUntilMoved;
    private bool hasMouseMoved = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        suppressUntilMoved = isFirstSceneLoad;

        if (suppressUntilMoved && Mouse.current != null)
            initialMousePos = Mouse.current.position.ReadValue();
    }

    private void Start()
    {
        isFirstSceneLoad = false;
    }

    private void Update()
    {
        if (!suppressUntilMoved || hasMouseMoved || Mouse.current == null) return;

        Vector2 currentPos = Mouse.current.position.ReadValue();

        if (Vector2.Distance(currentPos, initialMousePos) > movementThreshold)
            hasMouseMoved = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (suppressUntilMoved && !hasMouseMoved) return;
        StartFade(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (suppressUntilMoved && !hasMouseMoved) return;
        StartFade(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SfxPlayer.instance != null)
            SfxPlayer.instance.PlayOneShot(clickSound);

        if (!string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadScene(sceneToLoad);
    }

    private void StartFade(bool show)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);

        if (show && SfxPlayer.instance != null)
            SfxPlayer.instance.PlayOneShot(hoverSound);

        fadeRoutine = StartCoroutine(FadeRoutine(show));
    }

    private IEnumerator FadeRoutine(bool show)
    {
        float start = canvasGroup.alpha;
        float target = show ? 1f : 0f;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;
    }
}