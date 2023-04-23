using UnityEngine;

public class BulletBase : MonoBehaviour
{
    public float damage;
    public float speed;

    public Transform attackPoint;
    public float attackRange;
    public LayerMask enemyLayer;

    private void Update()
    {
        Movement();
    }

    public virtual void Movement()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log($"We hit {enemy.name}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void OnBecameInvisible()
    {
        this.gameObject.SetActive(false);
    }
}
