using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private GameManager gm;
    [SerializeField] private GameObject promptUI;
    [SerializeField] private GameObject savedMessageUI;
    [SerializeField] private GameObject savedPlane;
    [SerializeField] private float messageDuration = 5f;
    [SerializeField] private Transform respawnPoint;

    private bool playerInRange;
    private Coroutine savedRoutine;

    private void Awake()
    {
        if (promptUI != null) promptUI.SetActive(false);
        SetSavedFeedback(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (promptUI != null) promptUI.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (promptUI != null) promptUI.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange) return;

        Keyboard kb = Keyboard.current;
        if (kb != null && kb.eKey.wasPressedThisFrame)
        {
            if (respawnPoint != null)
                gm.Save(respawnPoint.position);
            else
                gm.Save();

            ShowSavedFeedback();
        }
    }

    private void ShowSavedFeedback()
    {
        // Pressing E again restarts the timer instead of stacking timers
        if (savedRoutine != null) StopCoroutine(savedRoutine);
        savedRoutine = StartCoroutine(SavedFeedbackRoutine());
    }

    private IEnumerator SavedFeedbackRoutine()
    {
        SetSavedFeedback(true);
        yield return new WaitForSeconds(messageDuration);
        SetSavedFeedback(false);
        savedRoutine = null;
    }

    private void SetSavedFeedback(bool active)
    {
        if (savedMessageUI != null) savedMessageUI.SetActive(active);
        if (savedPlane != null) savedPlane.SetActive(active);
    }
}