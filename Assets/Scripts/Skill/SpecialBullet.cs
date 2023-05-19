using UnityEngine;

public class SpecialBullet : BaseBullet
{
    [SerializeField] float rotationSpeed = 2f;

    private void Awake()
    {
        shakeIntensity = 7f;
        cameraShake = FindFirstObjectByType<CameraShake>();
    }

    public override void Impact()
    {
        transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);

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
        }
    }
}
