using UnityEngine;
using UnityEngine.UI;

public class TimerManager : Singleton<TimerManager>
{
    private const float TimeThreshold = 1f;

    [SerializeField] private Image playerIcon;
    [SerializeField] private Slider routeSlider;
    [SerializeField] private int goalDistance;

    private readonly float thresholdFraction = 0.2f; // Fracción de camino para cada umbral
    private float lastThreshold = 0f; // Último umbral alcanzado
    private float timer;
    private float impactTimer = 0f;
    private float thrustAmount;
    private bool wasPushed = false;
    private bool timerStopped = false;

    private EnemyPooler enemyPool;
    private ItemPooler itemPool;

    int impactCounter = 0;

    private void Awake()
    {
        enemyPool = FindFirstObjectByType<EnemyPooler>();
        itemPool = FindFirstObjectByType<ItemPooler>();
    }

    private void Start()
    {
        playerIcon.sprite = GameManager.Instance.CharacterSelected.CharacterIcon;
        routeSlider.maxValue = goalDistance;
        routeSlider.value = 0f;

        routeSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.InGame)
        {
            if (!wasPushed)
            {
                if (!timerStopped)
                {
                    if (impactCounter >= 3)
                    {
                        timer += Time.deltaTime * 2.5f;
                    }
                    else
                    {
                        timer += Time.deltaTime;
                    }
                }
                else
                {
                    impactTimer += Time.deltaTime;

                    if (impactTimer > 1f)
                    {
                        timerStopped = false;
                        impactTimer = 0f;
                    }
                }
            }
            else
            {
                timerStopped = true;
                timer -= Time.deltaTime * (3 + thrustAmount);
                impactTimer += Time.deltaTime * (3 + thrustAmount);

                if (impactTimer > thrustAmount)
                {
                    wasPushed = false;
                    impactTimer = 0;
                    thrustAmount = 0;
                }
            }

            if (timer >= TimeThreshold)
            {
                if (timer < 0) timer = 0;
                routeSlider.value = timer;
            }
        }
    }

    public void PushPlayerBack(float amount)
    {
        wasPushed = true;
        thrustAmount += amount;
        impactCounter++;
    }

    void OnSliderValueChanged(float value)
    {
        // Calcular el valor normalizado del progreso actual
        float progress = value / routeSlider.maxValue;

        // Verificar si se alcanzó un nuevo umbral
        if (GameManager.Instance.currentGameFlowState == GameFlowState.Waiting)
        {
            if (progress >= 0.03f && progress <= .04f)
            {
                //Debug.Log("Aproximadamente 1/5 del camino recorrido");
                // TODO: Ejecutar la lógica correspondiente al primer umbral

                GameManager.Instance.NormalGame();
            }
        }
        else if (progress >= lastThreshold + thresholdFraction)
        {
            // Actualizar el umbral
            lastThreshold += thresholdFraction;

            // Ejecutar la lógica correspondiente al umbral alcanzado
            if (lastThreshold >= 0.2f && lastThreshold < 0.4f)
            {
                //Debug.Log("Aproximadamente 2/5 del camino recorrido");
                // TODO: Ejecutar la lógica correspondiente al segundo umbral
            }
            else if (lastThreshold >= 0.4f && lastThreshold < 0.6f)
            {
                //Debug.Log("Aproximadamente 3/5 del camino recorrido");
                // TODO: Ejecutar la lógica correspondiente al tercer umbral


                UpdateObject(enemyPool, "File", 3f, 55f);
                UpdateObject(enemyPool, "Folder", 2f, 55f);
                UpdateObject(itemPool, "Coin", 3f, 55f);
                UpdateObject(itemPool, "Heal", 2.5f, 55f);
            }
            else if (lastThreshold >= 0.6f && lastThreshold < 0.9f)
            {
                //Debug.Log("Aproximadamente 4/5 del camino recorrido");

                // Calcular en qué parte del último cuarto se encuentra el usuario
                if (progress >= 0.6f && progress < 0.7f)
                {
                    //Debug.Log("Entre 60% y 70% del camino recorrido");
                    UpdateObject(enemyPool, "Folder", 0f, 55f);
                    // TODO: Ejecutar la lógica correspondiente a la primera parte del último cuarto
                }
                else if (progress >= 0.8f && progress < 0.9f)
                {
                    //Debug.Log("Entre 80% y 90% del camino recorrido");
                    // TODO: Ejecutar la lógica correspondiente a la tercera parte del último cuarto

                    UpdateObject(enemyPool, "File", 60f, 55f);
                    UpdateObject(enemyPool, "Folder", 60f, 55f);
                    UpdateObject(itemPool, "Coin", 0f, 55f);
                    UpdateObject(itemPool, "Heal", 2.5f, 55f);
                }
            }
        }
        else if (progress >= 1f)
        {
            // Debug.Log("Se alcanzó el final del camino");
            // TODO: Ejecutar la lógica correspondiente al final del camino
            GameManager.Instance.BossBattle();
        }
    }


    void UpdateObject<T>(T pool, string tag, float? newTime = null, float? speed = null) where T : MonoBehaviour
    {
        if (pool is EnemyPooler)
        {
            EnemyPooler enemyPool = (EnemyPooler)(MonoBehaviour)pool;
            if (speed != null)
            {
                enemyPool.UpdateSpeed(tag, speed.Value);
            }
            if (newTime != null)
            {
                enemyPool.UpdateValue(tag, newTime.Value);
            }
        }
        else if (pool is ItemPooler)
        {
            ItemPooler itemPool = (ItemPooler)(MonoBehaviour)pool;
            if (newTime != null)
            {
                itemPool.UpdateValue(tag, newTime.Value);
            }
            if (speed != null)
            {
                itemPool.UpdateSpeed(tag, speed.Value);
            }
        }
        else
        {
            Debug.LogError("UpdateObject() solo admite objetos de tipo EnemyPooler o ItemPooler");
        }
    }
}
