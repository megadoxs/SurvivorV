using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Vector2 AttackDirection = Vector2.down;
    Vector2 Size;
    Vector2 Offset;

    BoxCollider2D SwordCollider;

    void Start()
    {
        SwordCollider = GetComponent<BoxCollider2D>();
        Size = SwordCollider.size;
        Offset = SwordCollider.offset;
    }

    public void Attack()
    {
        SwordCollider.enabled = true;
        if (AttackDirection == Vector2.left)
            AttackLeft();
        else if (AttackDirection == Vector2.right)
            AttackRight();
        else if (AttackDirection == Vector2.up)
            AttackUp();
        else if (AttackDirection == Vector2.down)
            AttackDown();
    }

    private void AttackRight()
    {
        SwordCollider.size = Size;
        SwordCollider.offset = Offset;
    }

    private void AttackLeft()
    {
        SwordCollider.size = Size;
        SwordCollider.offset = new Vector2(-Offset.x, Offset.y);
    }

    private void AttackUp()
    {
        SwordCollider.size = new Vector2(Size.y, Size.x);
        SwordCollider.offset = new Vector2(Offset.y, Offset.x);
    }

    private void AttackDown()
    {
        SwordCollider.size = new Vector2(Size.y, Size.x);
        SwordCollider.offset = new Vector2(Offset.y, -Offset.x);
    }

    public void StopAttack()
    {
        SwordCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
            collider.GetComponent<Enemy>().Damage(1f);
    }
}
