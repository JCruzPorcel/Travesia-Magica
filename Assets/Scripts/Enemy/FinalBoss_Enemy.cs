using UnityEngine;

public class FinalBoss_Enemy : Enemy
{
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
}