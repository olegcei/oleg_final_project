using UnityEngine;

public class PlayerPosition : MonoBehaviour
{

    public Vector3 playerPos;

    public int level;

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = playerPos;

        transform.position = new Vector3(28, -1, 35);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
