using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public int playLevel = 1;

    //Se crean instancias de los scritps para poder actualizar sus datos
    GameManager gm;
    [SerializeField] PlayerHealth ph;
    [SerializeField] PlayerPosition pp;
    

    void Awake()
    {
        //Inicializamos las instancias para poder actualizar sus datos
        gm = GetComponent<GameManager>();
  

        string dataPath = Application.persistentDataPath + "/savedata.save";

        //Comprobacion de exixtencia de archivo
        if (File.Exists(dataPath))
        {
            //Llamada a funcion de carga de datos guardados
            Load();
        }

    }

    //Funcion de carga de datos guardados
    void Load()
    {
        //Se accede al metodo "LoadAllData()" de la clase "SaveLoadMethods" para cargar los datos guardados en una variable
        SaveData saveData = SaveLoadMethods.LoadAllData();
        //Se actualiza la variable con los datos guardados
        ph.currentHealth = saveData.vida;

        pp.playerPos = new Vector3(saveData.positionPlayer[0], saveData.positionPlayer[1], saveData.positionPlayer[2]);

        Debug.Log("Datos cargados");
        Debug.Log("La vida actual es: " + ph.currentHealth);
        Debug.Log("La posicíon del player es: " + pp.playerPos);
        Debug.Log("Level: " + pp.level);
    }


    //public void Save(Vector3 respawnPosition)
    //{
    //    pp.playerPos = respawnPosition;
    //    Save();
    //}

    [ContextMenu("Delete save file")]
    void DeleteSaveFile()
    {
        SaveLoadMethods.DeleteSaveData();
    }
}
