using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private string gameSceneName = "GameScene";

    private void Start()
    {
        // Greyed out when there is nothing to continue
        //continueButton.interactable = SaveLoadMethods.SaveAllData();
    }

    public void OnContinuePressed()
    {
        // Save file exists, so GameManager.Awake loads it automatically
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnNewGamePressed()
    {
        // Must delete first, otherwise GameManager would load the old save
        SaveLoadMethods.DeleteSaveData();
        SceneManager.LoadScene(gameSceneName);
    }
}