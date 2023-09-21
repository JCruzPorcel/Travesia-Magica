using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    private const int MAX_TABLE_OF_PLAYERS = 5;
    const string mainMenuLevel = "MainMenu";

    private string playerName;
    private int playerScore;

    [Space(10)]
    [SerializeField] TextMeshProUGUI[] playerName_Text;
    [Space(5)]
    [SerializeField] TextMeshProUGUI[] playerScore_Text;
    [Space(5)]
    [SerializeField] TextMeshProUGUI[] playerNumberPositio_Text;
    [Space(10)]
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI scoreText;

    [SerializeField] Button localButton;
    [SerializeField] Button globalButton;
    [SerializeField] Button backToMainMenuButton;

    [SerializeField] GameObject leaderboardGo;
    [SerializeField] GameObject leaderboard_EntryName_Go;
    [SerializeField] GameObject divisorGo;

    [SerializeField] Color normalColor;
    [SerializeField] Color highlightColor;

    FadeInOut fadeInOut;
    GameManager gameManager;


    private void Start()
    {
        fadeInOut = FindFirstObjectByType<FadeInOut>();
        gameManager = GameManager.Instance;

        inputField.onEndEdit.AddListener(SaveName);

        globalButton.onClick.AddListener(ShowGlobalLeaderboard);
        localButton.onClick.AddListener(ShowLocalLeaderboard);
        backToMainMenuButton.onClick.AddListener(MainMenu);
    }

    private void MainMenu()
    {
        fadeInOut.FadeIn();
        gameManager.LoadLevelAsync(1f, mainMenuLevel);
        gameManager.MainMenu(1f);
        DisableButtonInteraction();
    }

    private async void SaveName(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        playerName = text;
        playerScore = (int)ScoreManager.Instance.PlayerScore;

        await FirebaseManager.SaveScore(playerName, playerScore);

        leaderboard_EntryName_Go.SetActive(false);
        leaderboardGo.SetActive(true);
        ShowLocalLeaderboard();
    }

    public async void ShowLocalLeaderboard()
    {
        List<PlayerScore> topScores = await FirebaseManager.LoadLocalScores();
        UpdateLeaderboard(topScores);
    }

    public async void ShowGlobalLeaderboard()
    {
        List<PlayerScore> topScores = await FirebaseManager.LoadGlobalScores();
        UpdateLeaderboard(topScores);
    }

    private void UpdateLeaderboard(List<PlayerScore> topScores)
    {
        for (int i = 0; i < topScores.Count; i++)
        {
            if (i < MAX_TABLE_OF_PLAYERS)
            {
                playerName_Text[i].text = topScores[i].playerName;
                playerScore_Text[i].text = topScores[i].playerScore.ToString("D5");
                playerNumberPositio_Text[i].text = $"{i + 1}.";

                playerScore_Text[i].color = normalColor;
                playerName_Text[i].color = normalColor;

                if (playerName == topScores[i].playerName && playerScore == topScores[i].playerScore)
                {
                    playerScore_Text[i].color = highlightColor;
                    playerName_Text[i].color = highlightColor;
                }
                divisorGo.SetActive(false);
            }
            else
            {
                if (playerName == topScores[i].playerName && playerScore == topScores[i].playerScore)
                {
                    playerName_Text[6].text = topScores[i].playerName;
                    playerScore_Text[6].text = topScores[i].playerScore.ToString();
                    playerNumberPositio_Text[6].text = $"{i + 1}.";

                    playerScore_Text[6].color = highlightColor;
                    playerName_Text[6].color = highlightColor;

                    /*********************************************/

                    if (i - 1 >= 0)
                    {
                        playerName_Text[5].text = topScores[i - 1].playerName;
                        playerScore_Text[5].text = topScores[i - 1].playerScore.ToString("D5");
                        playerNumberPositio_Text[5].text = $"{i}.";

                        playerScore_Text[5].color = normalColor;
                        playerName_Text[5].color = normalColor;
                    }

                    /*********************************************/

                    if (i + 1 < topScores.Count)
                    {
                        playerName_Text[7].text = topScores[i + 1].playerName;
                        playerScore_Text[7].text = topScores[i + 1].playerScore.ToString("D5");
                        playerNumberPositio_Text[7].text = $"{i + 2}.";

                        playerScore_Text[7].color = normalColor;
                        playerName_Text[7].color = normalColor;
                    }

                    divisorGo.SetActive(true);

                }
            }
        }
    }

    public void ShowLeaderboard()
    {
        leaderboard_EntryName_Go.SetActive(true);
        scoreText.text = $"Score alcanzado: <color=yellow><i>{ScoreManager.Instance.PlayerScore.ToString("D5")}</i></color>!!!";
        inputField.Select();
    }

    private void DisableButtonInteraction()
    {
        backToMainMenuButton.interactable = false;
        localButton.interactable = false;
        globalButton.interactable = false;
    }
}