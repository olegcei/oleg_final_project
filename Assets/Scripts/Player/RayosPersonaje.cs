using UnityEngine;

public class RayosPersonaje : MonoBehaviour
{
    CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    //Comprueba si el player toca suelo o no
    public bool DetectarSuelo()
    {
        return _characterController.isGrounded;
    }

    //Detectar y emparentar plataforma
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Plataforma")
        {
            this.transform.parent = hit.transform;
        }
        else this.transform.parent = null;
    }
}