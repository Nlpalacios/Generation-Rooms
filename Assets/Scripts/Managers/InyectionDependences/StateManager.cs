using UnityEngine;
//Interface
public interface IStateManagment 
{
    playerState currentState { get; }

    void SetPlayerState(playerState newState);
}

public class StateManager : IStateManagment
{
    private playerState currentGameState;
    public playerState currentState => currentGameState;

    public void SetPlayerState(playerState newState)
    {
        currentGameState = newState;

        switch (currentState)
        {
            case playerState.Default:
                Debug.Log($"Player State: {currentState}");
                break;

            case playerState.Exploration:
                Debug.Log($"Player State: {currentState}");
                Exploration();
                break;

            case playerState.Atack:
                Debug.Log($"Player State: {currentState}");
                break;

            case playerState.OpenCards:
                Debug.Log($"Player State: {currentState}");
                OpenCards();
                break;
        }

        GameManager.Instance.GetCurrentState = currentState;
    }

    #region Exploration
    private void Exploration()
    {
        SetSlowMotion(false);
    }
    #endregion

    #region Attack
    #endregion

    #region OpenCards
    private void OpenCards()
    {
        SetSlowMotion(true);
    }
    #endregion

    private void SetSlowMotion(bool isActive)
    {
        if (isActive) { 
            Time.timeScale = .5f;
        }
        else {
            Time.timeScale = 1;
        }
    }

}
