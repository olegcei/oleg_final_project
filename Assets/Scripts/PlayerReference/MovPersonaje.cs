using UnityEngine;

public class MovPersonaje : MonoBehaviour
{
    [Header("Referencias")]
    private SaltoyGravedadPlayer _saltoyGravedadPlayer;
    public CharacterController CharacterController;
    private InputsPersonaje _inputControles;

    [Header("Movimiento")]
    private Vector3 _direccionFinal;
    [SerializeField] private float _velocidadBase;
    public float multiplicadorAlCorrer;
    private Vector3 _direccionXZ;
    private float _velocidadFinal;

    private void Awake()
    {
        _saltoyGravedadPlayer = GetComponent<SaltoyGravedadPlayer>();
        CharacterController = GetComponent<CharacterController>();
        _inputControles = GetComponent<InputsPersonaje>();
    }

    private void Start()
    {
        CalcularVelocidad();
    }

    private void Update()
    {
        Movimiento();
    }

    //Calculo para el movimiento del player
    private void Movimiento()
    {
        _direccionXZ = new Vector3(_inputControles.inputMovimiento.x, 0, _inputControles.inputMovimiento.y).normalized;
        _direccionFinal = transform.TransformDirection(_direccionXZ);
        _direccionFinal.y += _saltoyGravedadPlayer.ejeY;

        CharacterController.Move(new Vector3(_direccionFinal.x * _velocidadFinal, _direccionFinal.y, _direccionFinal.z * _velocidadFinal) * Time.deltaTime);
    }

    //Calculo de la variación de velocidad del player
    public void CalcularVelocidad()
    {
        _velocidadFinal = _velocidadBase * multiplicadorAlCorrer;
    }
}
