using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public abstract class Entity : MonoBehaviour
{
    protected EntitySO entity;
    
    protected HostilityType hostility;
    
    //properties
    protected float health;
    protected float maxHealth;
    protected float speed;
    protected float damage;
    protected float range = 1.5f;
    protected float attackRange;
    protected float attackSpeed = 0.5f; //for testing

    protected ItemStack[] drops;
    
    protected Rigidbody2D rb;
    protected Animator animator;
    protected AIPath aiPath;
    protected AIBase aiBase;
    protected AIDestinationSetter destination;
    protected FloatingHealthBar healthBar;
    protected Coroutine hideHealthBar;

    protected float attackDelay;

    [SerializeField] 
    private GameObject itemPrefab;

    public void SetEntityStats(EntitySO entity)
    {
        //initializing properties
        this.entity = entity;
        maxHealth = entity.maxHealth;
        health = maxHealth;
        damage = entity.damage;
        attackRange = entity.attackRange;
        drops = entity.drops;
    }
    
    public void LoadEntity(EntityData entityData)
    {
        var entity = entityData.GetEntity();
        
        this.entity = entity;
        maxHealth = entity.maxHealth;
        health = entityData.health;
        damage = entity.damage;
        attackRange = entity.attackRange;
        drops = entity.drops;
    }

    public EntitySO GetEntity()
    {
        return entity;
    }

    public float GetHealth()
    {
        return health;   
    }
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthBar = GetComponentInChildren<FloatingHealthBar>(true);
        aiPath = GetComponent<AIPath>();
        aiBase = GetComponent<AIBase>();
        destination = GetComponent<AIDestinationSetter>();
        aiPath.endReachedDistance = attackRange;
    }
    
    
    private void FixedUpdate()
    {
        if (aiPath.lastDeltaPosition.magnitude > 0)
        {
            animator.SetFloat("X", aiPath.lastDeltaPosition.x);
            animator.SetFloat("Y", aiPath.lastDeltaPosition.y);
            animator.SetBool("isMoving", true);
            GetComponent<SpriteRenderer>().flipX = aiPath.lastDeltaPosition.x < 0;
            return;
        }
        
        if (destination.target != null && aiPath.reachedEndOfPath && attackDelay == 0)
        {
            OnAttack();
            attackDelay = 1f / attackSpeed;
        }
        else if (attackDelay != 0)
        {
            attackDelay = Mathf.Max(attackDelay - Time.deltaTime, 0);
            OnReload();
        }
        animator.SetBool("isMoving", false);
    }

    protected abstract void OnAttack();
    protected abstract void OnReload();
    
    private void Update() //TODO care about hostility
    {
        if (destination.target == null)
        {
            GameObject closestTarget = null;
            var closestDistance = float.MaxValue;
            
            foreach (var target in GameObject.FindGameObjectsWithTag("Player")) //TODO use a MonsterTarget tag instead
            {
                if (!target.GetComponent<PlayerController>().alive)
                    continue;
                
                var distance = Vector3.Distance(transform.position, target.transform.position);
    
                if (distance < closestDistance && distance <= range)
                {
                    closestDistance = distance;
                    closestTarget = target;
                }
            }
            
            if(closestTarget != null)
                destination.target = closestTarget.transform;
            else if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                var dungeonHeart = GameObject.Find("DungeonHeart");
                if (dungeonHeart != null)
                    destination.target = dungeonHeart.transform;
            } 
                
        }
        else if (destination.target != null && destination.target.CompareTag("Player") && (Vector3.Distance(transform.position, destination.target.position) > range * 2 || !destination.target.gameObject.GetComponent<PlayerController>().alive)) //TODO use a MonsterTarget tag instead
        {
            var dungeonHeart = GameObject.Find("DungeonHeart");
            if (dungeonHeart != null)
                destination.target = dungeonHeart.transform;
            else
                destination.target = null;
        }
    }

    float Health
    {
        set
        {
            health = value;
            healthBar.UpdateHealth(value, maxHealth);

            if (health <= 0)
            {
                foreach (var drop in drops)
                {
                    var item = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                    item.GetComponent<PickupItem>().SetItemStack(drop.Clone());
                }
                Destroy(gameObject);
            }
        }
        get => health;
    }

    public void Damage(float damage, Transform attacker)
    {
        destination.target = attacker; //could have an abstract OnDamage(attacker)
        Damage(damage);
    }
    
    public void Damage(float damage)
    {
        healthBar.gameObject.SetActive(true);
        if (hideHealthBar != null)
            StopCoroutine(hideHealthBar);
        Health = math.max(Health - damage, 0);
        hideHealthBar = StartCoroutine(hideHealth(3f));
    }

    public void Knockback(Transform source, float knockbackForce) //care about resistances
    {
        rb.AddForce((transform.position - source.position).normalized * knockbackForce, ForceMode2D.Impulse);
        GetComponent<AIBase>().canMove = false;
        StartCoroutine(WaitForKnockbackToEnd());
    }
    
    private IEnumerator WaitForKnockbackToEnd()
    {
        yield return new WaitUntil(() => rb.linearVelocity.magnitude < 0.1f);
        GetComponent<AIBase>().canMove = true;
    }

    public void Heal(float heal)
    {
        Health = math.min(Health + heal, maxHealth);
    }

    private IEnumerator hideHealth(float time)
    {
        yield return new WaitForSeconds(time);
        if (healthBar.gameObject != null)
            healthBar.gameObject.SetActive(false);
    }
}
