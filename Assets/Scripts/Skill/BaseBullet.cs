using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    public float baseDamage;
    public float multiplierCritical = 2f;
    public float chanceCritical = 0.1f;

    public float speed;
    public float attackRange;
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public float knockbackForce = 500f;
    public TrailRenderer trailRender;
    public float shakeIntensity = 5f;
    public CameraShake cameraShake;

    private void Awake()
    {
        trailRender = GetComponentInChildren<TrailRenderer>();
        cameraShake = FindFirstObjectByType<CameraShake>();
    }

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
        transform.position += Vector3.right * speed * Time.deltaTime;
    }

    public virtual void Impact()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            var enemyClass = enemy.GetComponentInParent<Enemy>();

            Vector2 knockbackDirection = transform.right;
            enemyClass.ApplyKnockback(knockbackDirection, knockbackForce);

            float damage = baseDamage;

            if (Random.value <= chanceCritical)
            {
                damage *= multiplierCritical;
            }

            enemyClass.TakenDamage(damage);
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
