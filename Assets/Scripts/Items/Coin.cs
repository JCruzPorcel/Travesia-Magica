using UnityEngine;

public class Coin : Item
{
    [SerializeField] int score = 10;
    [SerializeField] float stoppingDistance = 1f;
    [SerializeField] Transform playerTransform;

    private bool isMovingTowardsPlayer = false;

    public override void ObjectAtribute()
    {
        ScoreManager.Instance.GetScoreFromAnotherObject(score);
    }

    public override void ItemPick()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        foreach (Collider2D player in hitPlayer)
        {
            if (player.CompareTag("Player") || player.CompareTag("Invulnerable"))
            {
                playerTransform = player.transform;
                isMovingTowardsPlayer = true;
                break;
            }
        }
    }

    public override void ObjectMovement()
    {
        if (isMovingTowardsPlayer && playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            transform.position += (Vector3)direction * speed * Time.deltaTime;

            if (Vector2.Distance(transform.position, playerTransform.position) < stoppingDistance)
            {
                ObjectAtribute();
                isMovingTowardsPlayer = false;
                gameObject.SetActive(false);
            }
        }
        else
        {
            base.ObjectMovement();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
