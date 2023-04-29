using UnityEngine;

public class Folder_Enemy : Enemy
{

    [SerializeField] float width = 2f;

    public override void Attack()
    {
        Vector2 size = new Vector2(attackRange * width, attackRange);

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

        Vector3 size = new Vector3(attackRange * width, attackRange, 0f);
        Gizmos.DrawWireCube(attackPoint.position, size);

    }
}
