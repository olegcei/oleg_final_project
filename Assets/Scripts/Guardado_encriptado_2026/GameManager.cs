using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int level;

    //Se crean instancias de los scritps para poder actualizar sus datos

    [SerializeField] PlayerHealth ph;
    [SerializeField] PlayerPosition pp;
    GameManager gameManager;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();

        string dataPath = Application.persistentDataPath + "/savedata.save";

        // Only load when the player pressed Continue, not on every scene start
        if (GameSession.LoadSaveOnStart)
        {
            GameSession.LoadSaveOnStart = false;

            if (File.Exists(dataPath))
                Load();
        }
    }

    public static string SceneForLevel(int level)
    {
        switch (level)
        {
            case 1:
                return "MineShaft";
            case 2:
                return "Turret_sewer";
            default:
                return "MineShaft";
        }
    }
    //Funcion de carga de datos guardados
    void Load()
    {
        //Se accede al metodo "LoadAllData()" de la clase "SaveLoadMethods" para cargar los datos guardados en una variable
        SaveData saveData = SaveLoadMethods.LoadAllData();
        if (saveData == null) return;
        //Se actualiza la variable con los datos guardados
        ph.currentHealth = saveData.vida;

        Vector3 savedPos = new Vector3(saveData.positionPlayer[0], saveData.positionPlayer[1], saveData.positionPlayer[2]);

        ph.maxHealth = saveData.vida;
        pp.playerPos = savedPos;
        pp.transform.position = savedPos;
        gameManager.level = saveData.level;

        Debug.Log("Datos cargados");
        Debug.Log("La vida actual es: " + ph.currentHealth);
        Debug.Log("La posicíon del player es: " + pp.playerPos);
        Debug.Log("Level: " + gameManager.level);
    }

    public void Save()
    {
        Save(pp.transform.position);
    }

    public void Save(Vector3 respawnPosition)
    {
        //pp.playerPos = respawnPosition;
        //Save();
        pp.playerPos = respawnPosition;

        SaveLoadMethods.SaveAllData(pp, ph, gameManager);
        Debug.Log("Datos guardados en: " + pp.playerPos);
    }

    [ContextMenu("Delete save file")]
    void DeleteSaveFile()
    {
        SaveLoadMethods.DeleteSaveData();
    }
}
