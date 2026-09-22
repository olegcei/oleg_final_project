using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{

    public Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        animator.SetBool("isRunning", isRunning);
    }
}
