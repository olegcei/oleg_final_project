using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    [SerializeField] string mainMenuScene = "MainMenu";

    void Start()
    {
        Time.timeScale = 1f;                    // in case the level paused it
        Cursor.lockState = CursorLockMode.None; // first-person scripts usually lock it
        Cursor.visible = true;
    }

}