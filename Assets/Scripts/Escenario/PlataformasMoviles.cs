using UnityEngine;

public class PlataformasMoviles : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float _Velocidad;
    [SerializeField] private Transform[] _Posiciones;
    private int _Index = 0;

    private void Awake()
    {
        transform.position = _Posiciones[0].position;
    }

    void Update()
    {
        CompruebaWaypoints();        
    }

    void FixedUpdate()
    {
        Mov();
    }

    void Mov()
    {
        //Movimiento plataforma lineal
        //transform.position = Vector3.MoveTowards(transform.position, _Posiciones[_Index].position, _Velocidad * Time.fixedDeltaTime);

        //Movimiento plataforma con aceleracion y tiempo de parada
        transform.position = Vector3.Lerp(transform.position, _Posiciones[_Index].position, _Velocidad * Time.fixedDeltaTime);
    }

    //Comprobamos cuando la plataforma llega al siguiente punto y pasa a otro
    void CompruebaWaypoints()
    {
        if (Vector3.Distance(transform.position, _Posiciones[_Index].position) < 1f)
        {
            _Index++;
        }

        if (_Index >= _Posiciones.Length) _Index = 0;
    }
}

