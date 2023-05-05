using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [Header("Navigation Buttons")]
    [SerializeField] Button backButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button exitButton;

    public Button BackButton { get { return backButton; } }

    [Header("Menu Windows")]
    [SerializeField] GameObject optionMenu;
    [SerializeField] GameObject mainMenu;

    public GameObject OptionsMenu { get { return optionMenu; } }

    const string mainMenuLevel = "MainMenu";

    bool inOptionMenu;
    public bool InOptionMenu { get { return inOptionMenu; } set { inOptionMenu = value; } }

    FadeInOut fadeInOut;
    readonly float transitionTime = 1f;
    GameManager gameManager;
    PlayerControls playerInputs;


    private void Awake()
    {
        playerInputs = new PlayerControls();
        playerInputs.Player.Enable();
        playerInputs.Player.Options.performed += ToggleOptionMenu;
    }

    void Start()
    {
        gameManager = GameManager.Instance;
        fadeInOut = FindFirstObjectByType<FadeInOut>();

        backButton.onClick.AddListener(OpenMenu);


        if (gameManager.currentGameState == GameState.MainMenu) return;


        restartButton.onClick.AddListener(() =>
        {
            fadeInOut.FadeIn();
            string levelName = gameManager.CurrentLevelName;
            gameManager.LoadLevelAsync(transitionTime, levelName);
            gameManager.StartGame(transitionTime);
        });

        mainMenuButton.onClick.AddListener(() =>
         {
             fadeInOut.FadeIn();
             gameManager.LoadLevelAsync(transitionTime, mainMenuLevel);
             gameManager.MainMenu(transitionTime);
         });

        exitButton.onClick.AddListener(() =>
        {
            fadeInOut.FadeIn();
            gameManager.ExitGame(transitionTime);
        });
    }

    public void OpenMenu()
    {
        if (gameManager.currentGameState == GameState.MainMenu)
        {
            StartCoroutine(MenuTransition(optionMenu, mainMenu, inOptionMenu));

            Button button = FindFirstObjectByType<MenuManager>().PlayButton;
            gameManager.GetComponent<DeviceManager>().GetCurrentMenuButton(button);

        }
        else if (gameManager.currentGameState == GameState.InGame ||
                 gameManager.currentGameState == GameState.OnPause)
        {
            inOptionMenu = !inOptionMenu;
            optionMenu.SetActive(inOptionMenu);
            gameManager.ResumeGame();
        }
    }

    IEnumerator MenuTransition(GameObject optionMenu, GameObject otherMenu, bool inOptionMenu)
    {
        fadeInOut.FadeIn();
        DisableButtonInteraction();

        inOptionMenu = !inOptionMenu;

        yield return new WaitForSeconds(transitionTime);

        if (otherMenu != null)
        {
            otherMenu.SetActive(inOptionMenu);
        }

        optionMenu.SetActive(!inOptionMenu);

        fadeInOut.FadeOut();
        EnableButtonInteraction();
    }

    private void ToggleOptionMenu(InputAction.CallbackContext context)
    {
        if (gameManager.currentGameState == GameState.InGame || gameManager.currentGameState == GameState.OnPause)
        {
            inOptionMenu = !inOptionMenu;

            if (optionMenu != null)
            {
                optionMenu.SetActive(inOptionMenu);
            }

            if (inOptionMenu)
            {
                gameManager.PauseGame();
            }
            else
            {
                gameManager.ResumeGame();
            }
        }
    }

    private void DisableButtonInteraction()
    {
        backButton.interactable = false;

        if (gameManager.currentGameState == GameState.MainMenu) return;

        restartButton.interactable = false;
        mainMenuButton.interactable = false;
        exitButton.interactable = false;
    }

    private void EnableButtonInteraction()
    {
        backButton.interactable = true;

        if (gameManager.currentGameState == GameState.MainMenu) return;

        restartButton.interactable = true;
        mainMenuButton.interactable = true;
        exitButton.interactable = true;
    }
}
