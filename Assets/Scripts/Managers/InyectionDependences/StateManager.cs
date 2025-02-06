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
                break;

            case playerState.Exploration:
                Exploration();
                break;

            case playerState.Attack:
                break;

            case playerState.OpenCards:
                OpenCards();
                break;

            case playerState.Inspection:
                Inspection();
                break;

        }

        Debug.LogWarning($"NEW: Player State - {currentState}");
    }

    #region Exploration
    private void Exploration()
    {
        EventManager.Instance.TriggerEvent(PlayerEvents.OnStopMovement, false);
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
        if (!isActive && Time.timeScale == 1 || 
             isActive && Time.timeScale != 1) return;

        if (isActive) { 
            Time.timeScale = .5f;
        }
        else {
            Time.timeScale = 1;
        }
    }

    private void Inspection()
    {
        EventManager.Instance.TriggerEvent(PlayerEvents.OnStopMovement, true);
    }
}
