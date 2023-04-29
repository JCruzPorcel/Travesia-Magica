using UnityEngine;

public class Heal : ItemBase
{
    public int healRecovery;

    public override void ObjectAtribute()
    {
        FindFirstObjectByType<HealthSystem>().GetComponent<HealthSystem>().Heal(healRecovery);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
