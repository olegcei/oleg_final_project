using UnityEngine;

public class MovCamara : MonoBehaviour
{
    [Header("Valores cámara")]
    private Camera _camara;
    [SerializeField] private float _sensibilidad;
    [SerializeField] private float _limiteCamara;
    private float _rotacionVertical;

    private EntradasInput _inputRaton;

    private void Awake()
    {
        //Application.targetFrameRate = 60;
        //LimitarCursor();
        _inputRaton = GetComponent<EntradasInput>();
        _camara = FindAnyObjectByType<Camera>();
    }

    private void Update()
    {
        GiroCabeza();
    }

    //Calcula y gira al player y la camara
    private void GiroCabeza()
    {
        // Rotación del player en Y
        float playerXRotation = _inputRaton.inputCamara.x * _sensibilidad * Time.deltaTime;
        transform.Rotate(Vector3.up * playerXRotation);

        // Rotación de la cámara en X
        _rotacionVertical -= _inputRaton.inputCamara.y * _sensibilidad * Time.deltaTime;
        _rotacionVertical = Mathf.Clamp(_rotacionVertical, -_limiteCamara, _limiteCamara);
        _camara.transform.localRotation = Quaternion.Euler(_rotacionVertical, 0, 0);
    }

    //Bloquea y hace invisible el cursur del raton
    private void LimitarCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
