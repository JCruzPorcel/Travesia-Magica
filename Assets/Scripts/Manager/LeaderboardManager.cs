using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.TimeZoneInfo;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] string playerName;
    const string mainMenuLevel = "MainMenu";
    [SerializeField] int score;

    [SerializeField] TextMeshProUGUI[] leaderboardTxt;
    [SerializeField] TMP_InputField inputField;

    [SerializeField] Button localButton;
    [SerializeField] Button globalButton;
    [SerializeField] Button mainMenu;

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
        mainMenu.onClick.AddListener(MainMenu);
    }

    private void MainMenu()
    {
        fadeInOut.FadeIn();
        gameManager.LoadLevelAsync(1f, mainMenuLevel);
        gameManager.MainMenu(1f);
        leaderboardGo.SetActive(false);
    }

    private async void SaveName(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        
        playerName = text;

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
        int count = 0;

        for (int i = 0; i < leaderboardTxt.Length; i++)
        {
            leaderboardTxt[i].text = "---";
        }

        foreach (var score in topScores)
        {
            leaderboardTxt[count].text = ($"{score.playerName}              {score.playerScore}");
            count++;
        }
    }
}
