using System.Collections;
using UnityEngine;

public class SubtitlePlayer : MonoBehaviour
{
    [SerializeField] private GameObject[] subtitlePanels; // drag all 3 here, in order
    [SerializeField] private float[] durations;           // seconds each one stays up
    [SerializeField] private float gapBetween = 0.2f;
    [SerializeField] private bool playOnStart = true;

    void Start()
    {
        HideAll();
        if (playOnStart) Play();
    }

    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        for (int i = 0; i < subtitlePanels.Length; i++)
        {
            HideAll();
            subtitlePanels[i].SetActive(true);

            float time = i < durations.Length ? durations[i] : 2f;
            yield return new WaitForSeconds(time);

            subtitlePanels[i].SetActive(false);
            yield return new WaitForSeconds(gapBetween);
        }
    }

    void HideAll()
    {
        foreach (var p in subtitlePanels) p.SetActive(false);
    }
}