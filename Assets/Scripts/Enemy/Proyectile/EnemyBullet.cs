using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int baseDamage;
    public int multiplierCritical = 2;
    public float chanceCritical = 0.1f;

    public float speed;
    public float attackRange;
    public Transform attackPoint;
    public LayerMask playerMask;
    public float knockbackForce = 500f;
    public TrailRenderer trailRender;
    public float shakeIntensity = 5f;
    public CameraShake cameraShake;

    private void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.InGame)
        {
            Movement();
            Impact();

            if (trailRender != null)
                trailRender.enabled = true;
        }
        else
        {
            if (trailRender != null)
                trailRender.enabled = false;
        }
    }

    public virtual void Movement()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }

    public virtual void Impact()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerMask);

        foreach (Collider2D player in hitPlayer)
        {
            var playerClass = player.GetComponentInParent<HealthSystem>();

            int damage = baseDamage;

            if (Random.value <= chanceCritical)
            {
                damage *= multiplierCritical;
            }

            playerClass.Damage(damage);
            cameraShake.ShakeCameraWhenHit(shakeIntensity);

            this.gameObject.SetActive(false);
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