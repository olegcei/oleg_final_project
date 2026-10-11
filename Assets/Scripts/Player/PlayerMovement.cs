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

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _clipsPasos;
    [SerializeField, Range(0f, 1f)] private float _volumenPasos = 0.6f;
    [SerializeField] private float _distanciaPorPaso = 2f;
    private float _distanciaAcumulada;

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
        ReproducirPasos();

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

    private void ReproducirPasos()
    {
        bool seMueve = _inputControles.inputMovimiento.sqrMagnitude > 0.01f;

        if (!CharacterController.isGrounded || !seMueve)
        {
            // Al parar, el siguiente paso suena casi al instante cuando vuelves a caminar
            _distanciaAcumulada = _distanciaPorPaso;
            return;
        }

        _distanciaAcumulada += _velocidadFinal * Time.deltaTime;

        if (_distanciaAcumulada >= _distanciaPorPaso)
        {
            _distanciaAcumulada = 0f;

            if (_audioSource != null && _clipsPasos != null && _clipsPasos.Length > 0)
            {
                AudioClip clip = _clipsPasos[Random.Range(0, _clipsPasos.Length)];
                _audioSource.PlayOneShot(clip, _volumenPasos);
            }
        }
    }

}
