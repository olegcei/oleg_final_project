using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class CutsceneAutoSkip : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("Seconds before the cutscene auto-skips. Use 0 to wait for the Timeline to finish on its own.")]
    public float skipAfterSeconds = 10f;

    [Header("Next scene")]
    public string gameplaySceneName = "Gameplay";

    [Header("Optional")]
    public PlayableDirector director;
    public bool allowManualSkip = true;

    bool loading;

    void Start()
    {
        if (skipAfterSeconds > 0f)
            StartCoroutine(SkipTimer());
        else if (director != null)
            director.stopped += _ => LoadGameplay();
    }

    IEnumerator SkipTimer()
    {
        // Realtime so it still works if timeScale is changed
        yield return new WaitForSecondsRealtime(skipAfterSeconds);
        LoadGameplay();
    }

    void Update()
    {
        if (!allowManualSkip || loading) return;

        if ((Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) ||
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame))
        {
            LoadGameplay();
        }
    }

    void LoadGameplay()
    {
        if (loading) return;   // prevents double loading (timer + director + input)
        loading = true;

        if (director != null) director.Stop();
        SceneManager.LoadScene(gameplaySceneName);
    }
}