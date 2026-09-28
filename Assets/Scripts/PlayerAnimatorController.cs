using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimatorController : MonoBehaviour
{
    public Animator animator;
    public Hookshot hookshot;
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

        bool isSwinging = hookshot.IsHooking; // CHANGED - reflects actual hook state, not just input
        animator.SetBool("isSwinging", isSwinging);
    }


}