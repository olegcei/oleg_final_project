using UnityEngine;

public class GameManager1 : MonoBehaviour
{
    [SerializeField] GameObject player;
    GameObject playerInstancia;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerInstancia = Instantiate(player, new Vector3(0,10,0), Quaternion.identity);
    }
}
