using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private GameManager gm;
    [SerializeField] private GameObject promptUI;
    [SerializeField] private GameObject savedMessageUI;
    [SerializeField] private float messageDuration = 2f;
    [SerializeField] private Transform respawnPoint;

    private bool playerInRange;
    private Coroutine messageRoutine;

    private void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
        if (savedMessageUI != null) savedMessageUI.SetActive(false);
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

            ShowSavedMessage();
        }
    }

    private void ShowSavedMessage()
    {
        if (savedMessageUI == null) return;

        // If the player presses E again, restart the timer instead of stacking timers
        if (messageRoutine != null) StopCoroutine(messageRoutine);
        messageRoutine = StartCoroutine(MessageRoutine());
    }

    private IEnumerator MessageRoutine()
    {
        savedMessageUI.SetActive(true);
        yield return new WaitForSeconds(messageDuration);
        savedMessageUI.SetActive(false);
        messageRoutine = null;
    }
}