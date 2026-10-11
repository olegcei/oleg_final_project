using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] CanvasGroup group;

    public IEnumerator FadeOut(float duration) => Fade(1f, duration); // to black
    public IEnumerator FadeIn(float duration) => Fade(0f, duration); // back to clear

    IEnumerator Fade(float target, float duration)
    {
        float start = group.alpha;
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            group.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        group.alpha = target;
    }
}