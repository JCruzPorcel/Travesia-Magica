using UnityEngine;

public class File_Enemy : Enemy
{
    [SerializeField] float height = 2;

    public override void Attack()
    {
        Vector2 size = new Vector2(attackRange, attackRange * height);

        Collider2D[] hitPlayer = Physics2D.OverlapBoxAll(attackPoint.position, size, playerLayer);

        foreach (Collider2D player in hitPlayer)
        {
            if (player.CompareTag("Player"))
            {
                player.GetComponent<HealthSystem>().Damage(damage);
                this.gameObject.SetActive(false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Vector3 size = new Vector3(attackRange, attackRange * height, 0f);
        Gizmos.DrawWireCube(attackPoint.position, size);

    }
}
