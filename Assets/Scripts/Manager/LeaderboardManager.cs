using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    private const int MAX_TABLE_OF_PLAYERS = 8;
    const string mainMenuLevel = "MainMenu";

    private string playerName;
    private int score;

    [SerializeField] TextMeshProUGUI[] leaderboardPlayerName_Text;
    [SerializeField] TextMeshProUGUI[] leaderboardPlayerScore_Text;
    [SerializeField] TMP_InputField inputField;

    [SerializeField] Button localButton;
    [SerializeField] Button globalButton;
    [SerializeField] Button backToMainMenuButton;

    [SerializeField] GameObject leaderboardGo;
    [SerializeField] GameObject gameOverGo;

    public GameObject GameOverGo { get { return gameOverGo; } }

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
        score = (int)ScoreManager.Instance.PlayerScore;

        await FirebaseManager.SaveScore(playerName, score);

        gameOverGo.SetActive(false);
        leaderboardGo.SetActive(true);
        ShowLocalLeaderboard();
    }

    public async void ShowLocalLeaderboard()
    {
        List<PlayerScore> topScores = await FirebaseManager.LoadLocalScores();
        PlayerList(topScores);
    }

    public async void ShowGlobalLeaderboard()
    {
        List<PlayerScore> topScores = await FirebaseManager.LoadGlobalScores();
        PlayerList(topScores);
    }

    private void PlayerList(List<PlayerScore> topScores)
    {
        if (topScores == null) return;

        for (int i = 0; i < MAX_TABLE_OF_PLAYERS; i++)
        {
            leaderboardPlayerName_Text[i].text = "---";
            leaderboardPlayerScore_Text[i].text = "---";
        }

        for (int i = 0; i < topScores.Count; i++)
        {
            leaderboardPlayerName_Text[i].text = topScores[i].playerName;
            leaderboardPlayerScore_Text[i].text = topScores[i].playerScore.ToString();
        }

    }

    private void DisableButtonInteraction()
    {
        backToMainMenuButton.interactable = false;
        localButton.interactable = false;
        globalButton.interactable = false;
    }
}
