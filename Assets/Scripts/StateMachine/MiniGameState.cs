using System;
using UnityEngine;
public class MiniGameState : GameState
{
    private MiniGameInfo _currentMiniGame;
    private static IMiniGameUI GameUI => GameCanvas.MiniGameUI;
    
    public override void EnterState()
    {
        _currentMiniGame = null;
        if (PlayingState.IsStarting) //intro minigame
        {
            Loader.LoadStartingMiniGameScene(StartingLoaded);
        } 
        else //all other minigames
        {
            Loader.LoadMiniGameScene(StartMiniGame);
        }

        return;
        
        void StartingLoaded()
        {
            ActivePlayerTracker.SetPlayerStates(PlayerState.StartingMiniGame);
            StartMiniGame();
        }
    }

    public void StartMiniGame()
    {
        ActivePlayerTracker.ResetMinigameDeaths();
        
        //check scene for minigame object
        _currentMiniGame = MiniGameInfo.Instance;
        if (!_currentMiniGame)
        {
            Debug.LogError("MiniGameInfo not found! Please add a MiniGameInfo object to the scene!");
            return;
        }

        Game.IsPlayersFrozen = true;
        GameCanvas.Instance.SetPlayerUIVisibility(false);
        GameUI.SetValues(_currentMiniGame);
        ActivePlayerTracker.OnPlayerFinishMinigame += _currentMiniGame.TriggerEndMiniGame;
        _currentMiniGame.Intro(OnIntroFinished, OnGameFinished, ActivePlayerTracker.GetAlivePlayers());
        return;
        
        void OnIntroFinished()
        {
            Game.IsPlayersFrozen = false;
            GameUI.CurrentState = MiniGameUI.UIState.MiniGame;
            _currentMiniGame.Begin();
            
        }
        
        void OnGameFinished()
        {
            Game.IsFrozen = true;
            GameUI.CurrentState = MiniGameUI.UIState.Finished;
            _currentMiniGame.ShowResults(GameUI.DisableAll, () =>
            {
                _currentMiniGame.End();
                ActivePlayerTracker.OnPlayerFinishMinigame -= _currentMiniGame.TriggerEndMiniGame;
            });
        }
    }

    public override void ExitState(GameStates newState)
    {
        GameCanvas.Instance.SetPlayerUIVisibility(true);
        if (_currentMiniGame != null) _currentMiniGame.ForceEnd();
        
        GameUI.DisableAll();
        Game.IsPlayersFrozen = false;
        
        _currentMiniGame = null;

    }

    public override bool IsStateSwitchable(GameStates test)
    {
        throw new System.NotSupportedException();
    }
    
}