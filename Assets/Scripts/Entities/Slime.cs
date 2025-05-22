using System;
using UnityEngine;

public class Slime : Entity
{
    protected override void OnAttack()
    {
        aiBase.canMove = false;
        Vector2 targetPoint = destination.target.gameObject.GetComponent<Collider2D>().ClosestPoint(transform.position);
        Vector2 direction = (targetPoint - (Vector2)transform.position).normalized;
        rb.AddForce(direction * 3f, ForceMode2D.Impulse);
        animator.SetFloat("X", rb.linearVelocity.x);
        animator.SetFloat("Y", rb.linearVelocity.y);
        animator.SetTrigger("attack");
    }

    protected override void OnReload()
    {
        if (rb.angularVelocity > 0)
        {
            animator.SetFloat("X", rb.linearVelocity.x);
            animator.SetFloat("Y", rb.linearVelocity.y);
        }
            
        if (attackDelay == 0)
            aiBase.canMove = true;
    }
    
    public void OnCollisionEnter2D(Collision2D  other)
    {
        if (other.collider.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().Damage(damage);
        }
        else if (other.collider.name.Equals("DungeonHeart"))
        {
            other.gameObject.GetComponent<DungeonHeart>().Damage(damage);
        }
    }
}