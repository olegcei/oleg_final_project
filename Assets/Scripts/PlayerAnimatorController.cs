using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimatorController : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool isRunning = Keyboard.current.shiftKey.isPressed;
        animator.SetBool("isRunning", isRunning);

        bool isWalking = Keyboard.current.wKey.isPressed;
        animator.SetBool("isWalking", isWalking);

        bool isJumping = Keyboard.current.spaceKey.isPressed;
        animator.SetBool("isJumping", isJumping);

        bool isThrowing = Keyboard.current.gKey.isPressed;
        animator.SetBool("isThrowing", isThrowing);
    }


}