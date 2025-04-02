using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IPlayerDataSaver
{
    public float moveSpeed = 25f;
    public float maxSpeed = 1f;
    public SwordAttack swordAttack;
    Vector2 moveInput;
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    bool canMove = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (canMove && moveInput != Vector2.zero)
        {
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity + moveSpeed * Time.deltaTime * moveInput, maxSpeed);

            if (moveInput.x != 0)
            {
                spriteRenderer.flipX = moveInput.x < 0;
                swordAttack.AttackDirection = Vector2.right * Mathf.Sign(moveInput.x);
            }
            else if (moveInput.y != 0)
            {
                swordAttack.AttackDirection = Vector2.up * Mathf.Sign(moveInput.y);
            }

            animator.SetFloat("X", moveInput.x);
            animator.SetFloat("Y", moveInput.y);
            animator.SetBool("isMoving", true);
        }
        else
            animator.SetBool("isMoving", false);
    }

    void OnMove(InputValue value)
    {
        if(!PauseMenu.isPaused)
            moveInput = value.Get<Vector2>();
    }

    void OnAttack(){
        if (!PauseMenu.isPaused)
            animator.SetTrigger("swordAttack");
    }

    public void SwordAttack()
    {
        LockMovement();
        swordAttack.Attack();
    }

    public void SwordAttackEnd()
    {
        UnlockMovement();
        swordAttack.StopAttack();
    }

    public void LockMovement()
    {
        moveInput = Vector2.zero;
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }

    public void LoadData(PlayerData data)
    {
        gameObject.transform.position = data.position;
    }

    public void SaveData(ref PlayerData data)
    {
        data.position = gameObject.transform.position;
    }
}
