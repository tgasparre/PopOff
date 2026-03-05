using System.Collections.Generic;
using UnityEngine;
public class MiniGameState : GameState
{
    private MiniGameInfo _currentMiniGame;
    private static IMiniGameUI GameUI => GameCanvas.MiniGameUI;
    
    private List<int> _playerHealthBeforeMinigame = new List<int>();
    
    public override void EnterState()
    {
        _currentMiniGame = null;
        if (PlayingState.IsStarting) //intro minigame
        {
            //TODO -- play introduction animation
            Loader.LoadStartingMiniGameScene(StartingLoaded);
        } 
        else //all other minigames
        {
            //TODO -- Play little animation
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
        //check scene for minigame object
        _currentMiniGame = MiniGameInfo.Instance;
        if (!_currentMiniGame)
        {
            Debug.LogError("MiniGameInfo not found! Please add a MiniGameInfo object to the scene!");
            return;
        }

        Game.IsPlayersFrozen = true;
        GameUI.SetValues(_currentMiniGame);
        ActivePlayerTracker.SubscribeMiniGameDeath(_currentMiniGame.OnPlayerMiniGameLose);
        _currentMiniGame.Intro(OnIntroFinished, OnGameFinished);
        return;
        
        void OnIntroFinished()
        {
            Game.IsPlayersFrozen = false;
            GameUI.CurrentState = MiniGameUI.UIState.MiniGame;
            _currentMiniGame.Begin(ActivePlayerTracker.GetPlayers());
            
        }
        
        void OnGameFinished()
        {
            Game.IsPlayersFrozen = true;
            GameUI.CurrentState = MiniGameUI.UIState.Finished;
            _currentMiniGame.ShowResults(GameUI.DisableAll, () =>
            {
                _currentMiniGame.End();
            });
            ActivePlayerTracker.SubscribeMiniGameDeath(null);
        }
    }

    public override void ExitState()
    {
        GameUI.DisableAll();
        Game.IsPlayersFrozen = false;
        _currentMiniGame = null;
        
    }

    public override bool IsStateSwitchable(GameStates test)
    {
        throw new System.NotSupportedException();
    }
    
}