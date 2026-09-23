using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public int playLevel;

    //Se crean instancias de los scritps para poder actualizar sus datos
    GameManager gm;
    PlayerControl pc;

    void Awake()
    {
        //Inicializamos las instancias para poder actualizar sus datos
        gm = GetComponent<GameManager>();
        pc = FindAnyObjectByType<PlayerControl>();

        string dataPath = Application.persistentDataPath + "/savedata.save";

        //Comprobacion de exixtencia de archivo
        if (File.Exists(dataPath))
        {
            //Llamada a funcion de carga de datos guardados
            Load();
        }
        else playLevel = 1;
    }

    void Update()
    {
        //Guardar datos
        if (Input.GetKeyDown(KeyCode.G))
        {
            Save();
        }

        //Cargar datos
        if (Input.GetKeyDown(KeyCode.C))
        {
            Load();
        }

        //Reset pos player por si falla el guardado o la carga
        if (Input.GetKeyDown(KeyCode.R))
        {
            pc.playerPosition.x = 5;
            pc.playerPosition.y = 5;
            pc.playerPosition.z = 5;
            pc.vida = 50;
            playLevel = 8;
        }
    }

    //Funcion de carga de datos guardados
    void Load()
    {
        //Se accede al metodo "LoadAllData()" de la clase "SaveLoadMethods" para cargar los datos guardados en una variable
        SaveData saveData = SaveLoadMethods.LoadAllData();
        //Se actualiza la variable con los datos guardados
        gm.playLevel = saveData.playLevel;
        pc.vida = saveData.vida;
        pc.transform.position = new Vector3(saveData.positionPlayer[0], saveData.positionPlayer[1], saveData.positionPlayer[2]);


        Debug.Log("Datos cargados");
        Debug.Log("El nivel actual es: " + gm.playLevel);
        Debug.Log("La posicíon del player es: " + pc.playerPosition);
    }
    void Save()
    {
        //Se accede al metodo "SaveAllData()" de la clase "SaveLoadMethods" para guardar los datos de juego
        SaveLoadMethods.SaveAllData(gm, pc);
        Debug.Log("Datos guardados");
    }
}
