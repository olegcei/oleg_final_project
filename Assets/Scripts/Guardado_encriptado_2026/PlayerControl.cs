using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Vector3 playerPosition;
    public int vida;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(transform.position);
        Debug.Log(vida);
    }
}
