using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IPlayerDataSaver
{
    [SerializeField] 
    private TMP_Text hpText;
    
    public float moveSpeed = 25f;
    public float maxSpeed = 1f;
    public float maxHealth = 10;
    public SwordAttack swordAttack;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private new Collider2D collider;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool canMove = true;
    private float health = 10;
    public bool alive = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
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

    private void OnMove(InputValue value)
    {
        if(!PauseMenu.isPaused && alive && !GameOver.gameOver)
            moveInput = value.Get<Vector2>();
    }

    private void OnAttack(){
        if (!PauseMenu.isPaused && alive && !GameOver.gameOver)
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
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }

    private async void OnTransform()
    {
        if (canMove && alive && !PauseMenu.isPaused && !GameOver.gameOver)
        {
            bool transform = !animator.GetBool("isBat");
            canMove = false;
            
            if (transform)
                maxSpeed = 1.5f;
            else 
                maxSpeed = 1;
        
            animator.SetBool("isBat", transform);
            animator.SetBool("isMoving", false);
            
            await Task.Delay(500);
            canMove = true;
        }
    }

    public void LoadData(PlayerData data)
    {
        gameObject.transform.position = data.position;
        health = data.health;
    }

    public void SaveData(ref PlayerData data)
    {
        data.position = gameObject.transform.position;
        data.health = health;
    }

    public void Damage(float damage)
    {
        health -= damage;
        hpText.text = health + "/" + maxHealth;
        if (health <= 0)
        {
            canMove = false;
            alive = false;
            collider.enabled = false;
            spriteRenderer.enabled = false;
            moveInput = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            TextManager.instance.ShowBody("Respawning in: ", 10);
            StartCoroutine(Revive(10));
        }
    }

    private IEnumerator Revive(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        alive = true;
        canMove = true;
        spriteRenderer.enabled = true;
        collider.enabled = true;
        health = maxHealth;
        gameObject.transform.position = new Vector3(0.72f, 0.24f);
        hpText.text = health + "/" + maxHealth;
    }
}
