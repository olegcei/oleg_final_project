using UnityEngine;

public class SaltoyGravedadPlayer : MonoBehaviour
{
    private RayosPersonaje _rayosPersonaje;

    [Header("Salto")]
    [SerializeField] private int _saltosMaximos;
    private int _saltosActuales;
    private float _fuerzaSalto = 8f;

    [Header("Gravedad")]
    [SerializeField] private float _gravedad;
    [SerializeField] private float _limiteEjeY;
    public float ejeY;

    private float _coyoteTime = 0.1f;
    public float coyoteTimeCounter;
    public float jumpBufferTime;
    public float jumpBufferCounter;
    public bool isJumping;
    public bool jumpCancel;

    private void Awake()
    {
        _rayosPersonaje = GetComponent<RayosPersonaje>();
        if (_saltosMaximos < 1) _saltosMaximos = 1;
    }

    private void Update()
    {
        CalcularGravedad();
        ComprobarSaltos();
    }

    //Calculo y bloqueo de la gravedad
    private void CalcularGravedad()
    {
        if (_rayosPersonaje.DetectarSuelo() && ejeY <= 0)
        {
            ejeY = _limiteEjeY;
        }
        else
        if (ejeY > _limiteEjeY)
        {
            ejeY -= _gravedad * Time.deltaTime;
        }
    }

    //Calcula si salta o no
    private void ComprobarSaltos()
    {
        //Comprobar si salta 
        if (jumpBufferCounter > 0f) // El buffer del salto controla si se salta o no
        {
            if (!isJumping && _rayosPersonaje.DetectarSuelo()) // Primer salto desde el suelo
            {
                Salto();
            }
            else if (!isJumping && !_rayosPersonaje.DetectarSuelo()) // Salto inicial cayendo desde el precipicio dentro del tiempo coyote 
            {
                if (coyoteTimeCounter > 0f)
                {
                    Salto();
                    _saltosActuales--; // Salto inicial cayendo desde precipicio se le resta un salto al salto inicial para que cuente todos los saltos
                }
            }
            else if (isJumping && (_saltosActuales < _saltosMaximos - 1)) // Saltos a partir del primero
            {
                Salto();
            }
        }

        // Cancela el salto al soltar la tecla o botón
        if (jumpCancel && !_rayosPersonaje.DetectarSuelo())
        {
            if (ejeY > 0f && (jumpBufferCounter + 0.1f) < 0f) //El +0.1 es el salto minimo antes de poder ser cancelado
            {
                ejeY = 0f;
                jumpCancel = false;
            }
        }

        //Si el player toca el suelo reestablece valores
        if (_rayosPersonaje.DetectarSuelo())
        {
            coyoteTimeCounter = _coyoteTime;
            _saltosActuales = 0;
            if (jumpBufferCounter <= -0.5f) isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Comprobación de seguridad para no desbordar la variable jumpBufferCounter
        if (jumpBufferCounter > -1f) jumpBufferCounter -= Time.deltaTime;
    }

    //Establece valores si salta
    private void Salto()
    {
        isJumping = true;
        _saltosActuales++;
        jumpBufferCounter = 0f;
        ejeY = _fuerzaSalto;
    }
}
