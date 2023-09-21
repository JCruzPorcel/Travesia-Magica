using UnityEngine;

public class FinalBoss_Enemy : Enemy
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackInterval = 2f; // Intervalo de tiempo entre ataques

    private float timeSinceLastAttack = 0f;

    public override void Update()
    {
        base.Update();
        AttackCooldown();
    }

    public override void ApplyKnockback(Vector2 direction, float force)
    {

    }

    public override void Movement()
    {
        // Calcula el desplazamiento en el eje Y
        float verticalMovement = speed * Time.deltaTime;

        // Actualiza la posición del objeto
        transform.Translate(0f, -verticalMovement, 0f);

        // Verifica si alcanza los límites de movimiento
        if (transform.position.y <= -19f)
        {
            // Si alcanza el límite inferior, cambia la dirección de movimiento
            speed *= -1f;
        }
        else if (transform.position.y >= 15f)
        {
            // Si alcanza el límite superior, cambia la dirección de movimiento
            speed *= -1f;
        }
    }

    public override void Impact()
    {

    }

    public override void DespawnDistance()
    {

    }

    private void AttackCooldown()
    {
        // Actualiza el tiempo transcurrido desde el último ataque
        timeSinceLastAttack += Time.deltaTime;

        // Verifica si es hora de atacar nuevamente
        if (timeSinceLastAttack >= attackInterval)
        {
            Attack();
            timeSinceLastAttack = 0f; // Reinicia el tiempo desde el último ataque
        }
    }

    private void Attack()
    {
        // Crea un proyectil si el proyectilPrefab está asignado
        if (projectilePrefab != null)
        {
            // Aquí puedes instanciar un proyectil en la posición del enemigo
            // Puedes ajustar la lógica para determinar la posición y dirección del proyectil
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            // Agrega lógica adicional para configurar el movimiento y comportamiento del proyectil
        }
    }
}