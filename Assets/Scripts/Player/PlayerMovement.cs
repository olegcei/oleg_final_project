using UnityEngine;

// Obliga a que el objeto tenga un CharacterController (Unity lo añade solo).
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Referencias")]
    private SaltoyGravedadPlayer _saltoyGravedadPlayer;
    public CharacterController CharacterController;
    private EntradasInput _inputControles;

    [Header("Movimiento")]
    private Vector3 _direccionFinal;
    [SerializeField] private float _velocidadBase;
    public float multiplicadorAlCorrer;
    private Vector3 _direccionXZ;
    private float _velocidadFinal;

    private void Awake()
    {
        // Guardamos el CharacterController una sola vez para no buscarlo cada frame.
        CharacterController = GetComponent<CharacterController>();
        _saltoyGravedadPlayer = GetComponent<SaltoyGravedadPlayer>();
        _inputControles = GetComponent<EntradasInput>();
    }

    // Update se ejecuta una vez por frame: aqui leemos el mando/teclado y movemos.
    private void Start()
    {
        CalcularVelocidad();
    }

    private void Update()
    {
        Movimiento();
    }

    private void Movimiento()
    {
        _direccionXZ = new Vector3(_inputControles.inputMovimiento.x, 0, _inputControles.inputMovimiento.y).normalized;
        _direccionFinal = transform.TransformDirection(_direccionXZ);
        _direccionFinal.y += _saltoyGravedadPlayer.ejeY;

        CharacterController.Move(new Vector3(_direccionFinal.x * _velocidadFinal, _direccionFinal.y, _direccionFinal.z * _velocidadFinal) * Time.deltaTime);
    }

    public void CalcularVelocidad()
    {
        _velocidadFinal = _velocidadBase * multiplicadorAlCorrer;
    }

}
