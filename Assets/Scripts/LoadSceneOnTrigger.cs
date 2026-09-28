using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnTrigger : MonoBehaviour
{
    [Tooltip("Name of the scene to load (must be added to Build Settings)")]
    [SerializeField] private string sceneToLoad;

    [Tooltip("Tag the player object must have to trigger the load")]
    [SerializeField] private string playerTag = "Player";

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag(playerTag))
        {
            hasTriggered = true;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}