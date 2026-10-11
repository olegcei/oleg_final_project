using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] GameObject cutsceneContent;   // dolly cam + spline (disabled by default)
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Collider triggerCollider;
    [SerializeField] ScreenFader fader;
    [SerializeField] CinemachineBrain brain;       // drag your Main Camera here
    [SerializeField] float fadeTime = 0.5f;

    bool played;

    void OnTriggerEnter(Collider other)
    {
        if (played || !other.CompareTag("Player")) return;

        if (fader == null || director == null || cutsceneContent == null || playerInput == null)
        {
            Debug.LogError("CutsceneTrigger: a reference is missing in the Inspector.", this);
            return;
        }

        played = true;
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        // find the Brain before touching anything, so a failure never leaves the player locked
        if (brain == null) brain = FindFirstObjectByType<CinemachineBrain>();
        if (brain == null)
        {
            Debug.LogError("CutsceneTrigger: no CinemachineBrain found in the scene.", this);
            played = false;
            yield break;
        }

        triggerCollider.enabled = false;           // no re-triggering
        playerInput.DeactivateInput();             // lock the player right away

        // --- START: fade to black, then set up the cutscene ---
        yield return fader.FadeOut(fadeTime);

        cutsceneContent.SetActive(true);           // turn the dolly cam ON

        var timeline = (TimelineAsset)director.playableAsset;
        foreach (var track in timeline.GetOutputTracks())
            if (track is CinemachineTrack)
                director.SetGenericBinding(track, brain);

        director.Play();
        yield return null;                         // let the first frame render
        yield return fader.FadeIn(fadeTime);       // reveal the cutscene

        // --- END: fade to black during the last moments of the Timeline ---
        yield return new WaitUntil(() => director.time >= director.duration - fadeTime);
        yield return fader.FadeOut(fadeTime);

        // screen is black: switch off the cutscene and return to the player view
        director.Stop();
        cutsceneContent.SetActive(false);          // turn the dolly cam OFF
        yield return null;
        yield return new WaitUntil(() => !brain.IsBlending);

        yield return fader.FadeIn(fadeTime);
        playerInput.ActivateInput();               // give control back
    }
}