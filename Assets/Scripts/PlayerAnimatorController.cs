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
        var kb = Keyboard.current;
        if (kb == null) return;

        bool isRunning = kb.shiftKey.isPressed;
        animator.SetBool("isRunning", isRunning);

        bool isWalking = kb.wKey.isPressed || kb.aKey.isPressed ||
                         kb.sKey.isPressed || kb.dKey.isPressed;
        animator.SetBool("isWalking", isWalking);

        bool isJumping = kb.spaceKey.isPressed;
        animator.SetBool("isJumping", isJumping);

        bool isThrowing = kb.gKey.isPressed;
        animator.SetBool("isThrowing", isThrowing);

        bool isSwinging = hookshot.IsHooking; // reflects actual hook state, not just input
        animator.SetBool("isSwinging", isSwinging);
    }
}