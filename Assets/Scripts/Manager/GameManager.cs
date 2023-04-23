using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public enum GameState
{
    MainMenu,
    InGame,
    OnPause,
}

public class GameManager : SingletonPersistent<GameManager>
{
    public GameState currentGameState = GameState.MainMenu;

    [SerializeField] private CharacterData characterSelected;
    public CharacterData CharacterSelected { get => characterSelected; set => characterSelected = value; }

    [SerializeField] private PetData petSelected;
    public PetData PetSelected { get => petSelected; set => petSelected = value; }

    private string currentLevelName = string.Empty;
    public string CurrentLevelName { get => currentLevelName; set => currentLevelName = value; }

    // Delegado y evento para notificar cambios de estado
    public delegate void GameStateChangedHandler(GameState newGameState);
    public static event GameStateChangedHandler OnGameStateChanged;

    private void Start()
    {
        //currentGameState = GameState.MainMenu;
        Debug.Log($"Current Game State: {currentGameState}");
    }

    public void StartGame(float time)
    {
        StartCoroutine(NewGameSate(GameState.InGame, time));
    }

    public void PauseGame()
    {
        SetGameState(GameState.OnPause);
    }

    public void ResumeGame()
    {
        SetGameState(GameState.InGame);
    }

    public void MainMenu(float time)
    {
        StartCoroutine(NewGameSate(GameState.MainMenu, time));
    }

    // Método para cambiar el estado del juego y notificar cambios
    private void SetGameState(GameState newGameState)
    {
        this.currentGameState = newGameState;

        // Notificar a otros objetos que el estado del juego ha cambiado
        OnGameStateChanged?.Invoke(newGameState);


        switch (newGameState)
        {
            case GameState.MainMenu:
                // TODO: Implementar código para el estado MainMenu
                break;

            case GameState.InGame:
                // TODO: Implementar código para el estado InGame
                break;

            case GameState.OnPause:
                // TODO: Implementar código para el estado OnPause
                OptionsManager optionsManager = FindFirstObjectByType<OptionsManager>();
                optionsManager.OptionsMenu.SetActive(true);
                break;
        }
    }

    public void LoadLevelAsync(float transitionTime, string levelName)
    {
        StartCoroutine(LoadInGameLevelAsync(transitionTime, levelName));
    }

    IEnumerator LoadInGameLevelAsync(float transitionTime, string levelName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName);
        asyncLoad.allowSceneActivation = false;

        yield return new WaitForSeconds(transitionTime);

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    IEnumerator NewGameSate(GameState newGameState, float transitionTime)
    {

        yield return new WaitForSeconds(transitionTime);

        SetGameState(newGameState);

    }

    public void ExitGame(float transitionTime)
    {
        StartCoroutine(ApplicationQuit(transitionTime));
    }

    IEnumerator ApplicationQuit(float transitionTime)
    {
        FadeInOut fadeInOut = FindObjectOfType<FadeInOut>();

        fadeInOut.FadeIn();

        yield return new WaitForSeconds(transitionTime);


#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
}
