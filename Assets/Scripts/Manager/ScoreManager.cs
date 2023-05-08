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

        StartCoroutine(UpdateScoreText());
    }

    private IEnumerator UpdateScoreText()
    {
        GameObject go = SpawnText();

        var text = go.GetComponent<TMPro.TextMeshProUGUI>();
        var animator = go.GetComponent<Animator>();

        text.text = string.Empty;

        while (timerText > 0f)
        {
            if (currentScore > 0)
            {
                text.text = $"+{currentScore}";
            }

            timerText -= Time.deltaTime;
            yield return null;
        }
        currentScore = 0;
        animator.SetTrigger("HideAddScore");
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
            Debug.Log("No more score text objects in queue.");
        }
        return null;
    }

}