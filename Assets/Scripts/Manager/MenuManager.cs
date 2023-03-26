using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuGo;
    [Space(5)]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;

    [Space(10)]

    [Header("Character Selection")]
    [SerializeField] private GameObject charSelectionGo;

    [Space(5)]

    [Header("Character One")]
    [SerializeField] private Button characterOneButton;
    [SerializeField] private Image characterOneImage;
    [SerializeField] private GameObject CharOne_Panel;
    [SerializeField] private CharacterData characterOne;

    [Space(10)]

    [Header("Character Two")]
    [SerializeField] private Button characterTwoButton;
    [SerializeField] private Image characterTwoImage;
    [SerializeField] private GameObject CharTwo_Panel;
    [SerializeField] private CharacterData characterTwo;

    [Space(10)]

    [Header("Confirm Character")]
    [SerializeField] private GameObject confirmCharacterGo;
    [SerializeField] private Button confirmCharacterButton;
    [Space(5)] 
    [SerializeField] private string confirmText;

    [Space(10)]

    [Header("Options")]
    [SerializeField] private GameObject optionGo;
    [Space(5)]
    [SerializeField] private Button backToMenuButton;

    [Space(10)]

    [Header("Load Level")]
    [SerializeField] private string levelName = string.Empty;
    public string LevelName { get { return levelName; } }


    [SerializeField] private float loadingTime = 0f;

    [SerializeField] FadeInOut fadeInOut;
    [SerializeField] private float transitionTime = 1f;
    GameManager gameManager;


    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager.currentGameState == GameState.MainMenu)
            CloseWindows();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        characterOneImage.sprite = characterOne.CharacterImage;
        characterTwoImage.sprite = characterTwo.CharacterImage;

        if (confirmCharacterGo != null)
        {
            confirmCharacterGo.SetActive(false);
        }

        if (CharOne_Panel != null && CharTwo_Panel != null)
        {
            CharOne_Panel.SetActive(false);
            CharTwo_Panel.SetActive(false);
        }

        #region Button Listener

        playButton.onClick.AddListener(() =>
        {
            StartCoroutine(MenuTransition(charSelectionGo, mainMenuGo, false));
        });

        optionButton.onClick.AddListener(() =>
        {
            StartCoroutine(MenuTransition(optionGo, mainMenuGo, false));
        });

        exitButton.onClick.AddListener(() =>
        {
            DisableButtonInteraction();
            gameManager.ExitGame(transitionTime);
        });

        backToMenuButton.onClick.AddListener(() =>
        {
            StartCoroutine(MenuTransition(mainMenuGo, charSelectionGo, false));
        });

        characterOneButton.onClick.AddListener(() =>
        {
            CharacterSelect(characterOne, CharOne_Panel);
        });

        characterTwoButton.onClick.AddListener(() =>
        {
            CharacterSelect(characterTwo, CharTwo_Panel);
        });

        confirmCharacterButton.onClick.AddListener(() =>
        {
            StartCoroutine(MenuTransition(null, null, true));
            gameManager.StartGame(loadingTime);
        });

        #endregion
    }


    private void WindowManagement(GameObject openMenu, GameObject closeMenu)
    {
        if (openMenu != null && closeMenu != null)
        {
            openMenu.SetActive(true);
            closeMenu.SetActive(false);
        }

        if (CharOne_Panel != null && CharTwo_Panel != null)
        {
            CharOne_Panel.SetActive(false);
            CharTwo_Panel.SetActive(false);
        }
    }

    private void CharacterSelect(CharacterData character, GameObject panel)
    {

        if (character != null)
        {
            gameManager.CharacterSelected = null;
            gameManager.CharacterSelected = character;
        }

        if (confirmCharacterGo != null)
        {
            confirmCharacterGo.SetActive(true);
            confirmCharacterGo.GetComponentInChildren<TMP_Text>().text = confirmText;
        }

        if (CharOne_Panel != null && CharTwo_Panel != null)
        {
            CharOne_Panel.SetActive(false);
            CharTwo_Panel.SetActive(false);
        }

        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    IEnumerator MenuTransition(GameObject openMenu, GameObject closeMenu, bool isLoading)
    {
        fadeInOut.FadeIn();
        fadeInOut.IsLoading = isLoading;
        DisableButtonInteraction();

        yield return new WaitForSeconds(transitionTime);

        if (isLoading)
        {
            gameManager.CurrentLevelName = levelName;
            gameManager.LoadLevelAsync(loadingTime, levelName);
        }
        else
        {
            if (openMenu != null && closeMenu != null)
            {
                WindowManagement(openMenu, closeMenu);
                fadeInOut.FadeOut();
            }
        }

        if (confirmCharacterGo != null && confirmCharacterGo.activeInHierarchy)
        {
            confirmCharacterGo.SetActive(false);
        }

        EnableButtonInteraction();
    }

    private void CloseWindows()
    {
        mainMenuGo.SetActive(true);
        optionGo.SetActive(false);
        charSelectionGo.SetActive(false);
    }

    private void DisableButtonInteraction()
    {
        backToMenuButton.interactable = false;
        characterOneButton.interactable = false;
        playButton.interactable = false;
        optionButton.interactable = false;
        exitButton.interactable = false;
        confirmCharacterButton.interactable = false;
        characterTwoButton.interactable = false;
        characterOneButton.interactable = false;
    }

    private void EnableButtonInteraction()
    {
        backToMenuButton.interactable = true;
        characterOneButton.interactable = true;
        playButton.interactable = true;
        optionButton.interactable = true;
        exitButton.interactable = true;
        confirmCharacterButton.interactable = true;
        characterTwoButton.interactable = true;
        characterOneButton.interactable = true;
    }
}
