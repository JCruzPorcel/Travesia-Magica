using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{

    public event EventHandler OnDamaged;
    public event EventHandler OnHealed;

    private int healthAmount;
    [SerializeField] private int healthAmountMax;
    private CameraShake cameraShake;

    Animator anim;

    public void Awake()
    {
        this.healthAmount = healthAmountMax;
        anim = FindFirstObjectByType<HealthBar>().GetComponent<Animator>();
        cameraShake = FindFirstObjectByType<CameraShake>();
    }

    public void Damage(int amount)
    {
        healthAmount -= amount;
        anim.SetTrigger("Shake");
        cameraShake.ShakeCamera();
        if (healthAmount < 0)
        {
            healthAmount = 0;
        }
        if (OnDamaged != null) OnDamaged(this, EventArgs.Empty);
    }

    public void Heal(int amoount)
    {
        healthAmount += amoount;

        if (healthAmount > healthAmountMax)
        {
            healthAmount = healthAmountMax;
        }
        if (OnHealed != null) OnHealed(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return (float)healthAmount / healthAmountMax;
    }

}
