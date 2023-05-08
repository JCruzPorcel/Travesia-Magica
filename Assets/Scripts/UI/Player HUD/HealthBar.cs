using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private const float DAMAGED_HEALTH_TIMER_MAX = 1f;

    [SerializeField] Image barImage;
    [SerializeField] Image damagedBarImage;
    private Color damagedColor;
    private float damagedHealthTimer;

    HealthSystem healthSystem;

    [SerializeField] private bool healthShrinkBar = true;


    private void Awake()
    {
        healthSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthSystem>();

        if (!healthShrinkBar)
        {
            damagedColor = damagedBarImage.color;
            damagedColor.a = 0f;
            damagedBarImage.color = damagedColor;
        }
    }

    private void Start()
    {
        SetHealth(healthSystem.GetHealthNormalized());
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        healthSystem.OnHealed += HealthSystem_OnHealed;

        if (healthShrinkBar)
        {
            damagedBarImage.fillAmount = barImage.fillAmount;
        }
    }

    private void Update()
    {
        if (healthShrinkBar)
        {
            damagedHealthTimer -= Time.deltaTime;
            if (damagedHealthTimer < 0)
            {
                if (barImage.fillAmount < damagedBarImage.fillAmount)
                {
                    float shrinkSpeed = 1f;
                    damagedBarImage.fillAmount -= shrinkSpeed * Time.deltaTime;
                }
            }
        }
        else
        {
            if (damagedColor.a > 0)
            {
                damagedHealthTimer -= Time.deltaTime;
                if (damagedHealthTimer < 0)
                {
                    float fadeAmount = 5f;
                    damagedColor.a -= fadeAmount * Time.deltaTime;
                    damagedBarImage.color = damagedColor;
                }
            }
        }
    }

    private void HealthSystem_OnHealed(object senser, System.EventArgs e)
    {
        SetHealth(healthSystem.GetHealthNormalized());

        if (healthShrinkBar)
        {
            damagedBarImage.fillAmount = barImage.fillAmount;
        }
    }

    private void HealthSystem_OnDamaged(object senser, System.EventArgs e)
    {
        if (healthShrinkBar)
        {
            damagedHealthTimer = DAMAGED_HEALTH_TIMER_MAX;
        }
        else
        {
            if (damagedColor.a <= 0)
            {
                damagedBarImage.fillAmount = barImage.fillAmount;
            }

            damagedColor.a = 1;
            damagedBarImage.color = damagedColor;
            damagedHealthTimer = DAMAGED_HEALTH_TIMER_MAX;
        }


        SetHealth(healthSystem.GetHealthNormalized());
    }

    private void SetHealth(float healthNormalized)
    {
        barImage.fillAmount = healthNormalized;
    }
}
