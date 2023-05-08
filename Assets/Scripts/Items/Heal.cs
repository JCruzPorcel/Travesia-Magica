using System.Drawing;
using UnityEngine;

public class Heal : Item
{
    public int healRecovery;

    public override void ObjectAtribute()
    {
        FindFirstObjectByType<HealthSystem>().GetComponent<HealthSystem>().Heal(healRecovery);
    }

    public override void ItemPick()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapBoxAll(attackPoint.position, size, playerLayer);

        foreach (Collider2D player in hitPlayer)
        {
            if (player.CompareTag("Player") || player.CompareTag("Invulnerable"))
            {
                ObjectAtribute();
                this.gameObject.SetActive(false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireCube(attackPoint.position, size);
    }
}
