using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private float timeThreshold = 1f; // cantidad de segundos que deben pasar para agregar score
    private float timer = 0f; // contador de tiempo
    private float timerText = 0f; // contador de tiempo del texto
    [SerializeField] float timeLimit = 5f; // tiempo límite en segundos para acumular puntuación

    [SerializeField] private TMPro.TextMeshProUGUI scoreText;

    public List<GameObject> scoreList = new();
    readonly Queue<GameObject> scoreQueue = new();

    private long playerScore = 0; // puntuación actual del jugador
    private string ScoreFormat => playerScore.ToString().PadLeft(5, '0');
    private long currentScore = 0;
    [SerializeField] private int scorePerTime;

    public long PlayerScore { get { return playerScore; } }

    private void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.InGame)
        {
            timer += Time.deltaTime;

            AddToQueue();

            // Verificar si ha pasado el tiempo de umbral y agregar score
            if (timer >= timeThreshold)
            {
                AddScore(scorePerTime);
                timer = 0f;
            }
        }
    }

    public void AddScore(long score)
    {
        playerScore += score;
        scoreText.text = $"Score: {ScoreFormat}";
    }

    public void GetScoreFromAnotherObject(int score)
    {
        currentScore += score;
        timerText = timeLimit;

        try
        {
            StartCoroutine(UpdateScoreText());
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Se produjo un error al actualizar el texto del puntaje: " + ex.Message);
        }
    }

    private IEnumerator UpdateScoreText()
    {
        GameObject go = SpawnText();

        if (go != null)
        {
            var text = go.GetComponent<TMPro.TextMeshProUGUI>();
            var animator = go.GetComponent<Animator>();

            if (text != null)
            {
                text.text = string.Empty;
            }

            while (timerText > 0f)
            {
                if (currentScore > 0 && text != null)
                {
                    text.text = $"+{currentScore}";
                }

                timerText -= Time.deltaTime;
                yield return null;
            }

            currentScore = 0;

            if (animator != null)
            {
                animator.SetTrigger("HideAddScore");
            }
        }
    }


    private void AddToQueue()
    {
        if (scoreList.Count > 0)
        {
            foreach (GameObject go in scoreList)
            {
                if (!scoreQueue.Contains(go))
                {
                    scoreQueue.Enqueue(go);
                }
            }
        }
    }

    private GameObject SpawnText()
    {
        if (scoreQueue.Count > 0)
        {
            GameObject go = scoreQueue.Dequeue();
            return go;
        }
        else
        {
            GameObject poolParent = GameObject.Find($"Environment Floating Text Pool");

            if (poolParent == null)
            {
                poolParent = new GameObject($"Environment Floating Text Pool");
            }

            GameObject prefab = ObjectPooler.Instance.GetPrefab(ObjectType.Environment, "Floating Text");
            GameObject go = Instantiate(prefab, poolParent.transform);
            go.name = "Floating Text";
            scoreList.Add(go);
            return go;
        }
    }
}