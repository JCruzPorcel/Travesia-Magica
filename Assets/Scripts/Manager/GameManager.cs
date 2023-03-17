using UnityEngine;

[System.Serializable]
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


    private void Start()
    {
        currentGameState = GameState.MainMenu;
    }
}
