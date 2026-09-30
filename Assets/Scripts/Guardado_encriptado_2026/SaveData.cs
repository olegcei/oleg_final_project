[System.Serializable]
public class SaveData
{
    //Este script es nuestro contenedor de datos, son los datos que vamos a guardar cuando lo deseemos
    //Variables genericas del juego 


    //PlayerControl
    public float[] positionPlayer = new float[3];
    public float vida;
    public int level;


    //Contructor desde donde llamaremos para pasar los datos que se guarden
    public SaveData(PlayerPosition pp, PlayerHealth ph)
    {

        positionPlayer[0] = pp.playerPos.x;
        positionPlayer[1] = pp.playerPos.y;
        positionPlayer[2] = pp.playerPos.z;    

        vida = ph.currentHealth;

        level = pp.level;
    }
}