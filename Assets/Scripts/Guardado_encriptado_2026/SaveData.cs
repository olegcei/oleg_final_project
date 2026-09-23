[System.Serializable]
public class SaveData
{
    //Este script es nuestro contenedor de datos, son los datos que vamos a guardar cuando lo deseemos
    //Variables genericas del juego 
    public int playLevel;

    //PlayerControl
    public float[] positionPlayer = new float[3];
    public int vida;

    //Contructor desde donde llamaremos para pasar los datos que se guarden
    public SaveData(GameManager gm, PlayerControl pc)
    {
        playLevel = gm.playLevel;

        positionPlayer[0] = pc.playerPosition.x;
        positionPlayer[1] = pc.playerPosition.y;
        positionPlayer[2] = pc.playerPosition.z;

        vida = pc.vida;
    }
}
