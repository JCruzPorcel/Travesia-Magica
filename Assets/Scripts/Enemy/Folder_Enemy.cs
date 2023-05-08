using UnityEngine;

public class Folder_Enemy : Enemy
{
    public override void Impact()
    {      
        Collider2D[] hitPlayer = Physics2D.OverlapBoxAll(attackPoint.position, size, playerLayer);

        foreach (Collider2D player in hitPlayer)
        {
            if (player.CompareTag("Player"))
            {
                player.GetComponent<HealthSystem>().Damage(damage);
                TimerManager.Instance.PushPlayerBack(amountOfImpact);
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
