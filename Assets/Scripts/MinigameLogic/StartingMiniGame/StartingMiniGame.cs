using System;
using UnityEngine;

public class StartingMiniGame : MiniGameInfo
{
    [Space]
    [SerializeField] private AirFillBoard[] _airFillBoards;
    [SerializeField] private WeightUI[] _weightUIs;
    [SerializeField] private float _fillRate = 0.1f;
    [SerializeField] private float _deflateRate = 0.2f;
    [Space]
    [SerializeField] private PlayerStats _lightClass;
    [SerializeField] private PlayerStats _defaultClass;
    [SerializeField] private PlayerStats _heavyClass;

    private void SetMinigameUI(bool isVisible)
    {
        foreach (AirFillBoard board in _airFillBoards)
        {
            board.IsVisible = isVisible;
            board.SetValues(_fillRate, _deflateRate);
        }

        foreach (WeightUI weightUI in _weightUIs)
        {
            weightUI.IsVisible = isVisible;
        }
    }

    private void Start()
    {
        SetMinigameUI(false);
    }

    protected override void StartMiniGame()
    {
        //assign jump input to fill 
        for (int i = 0; i < _playerControllers.Length; i++)
        {
            _airFillBoards[i].IsVisible = true;
            _playerControllers[i].OnJump = _airFillBoards[i].Fill;

            _weightUIs[i].IsVisible = true;
        }
    }

    protected override void ShowMiniGameResults(Action onFinished, string reward)
    {
        //TODO fun animation
        for (int i = 0; i < _playerControllers.Length; i++)
        {
            _weightUIs[i].IsVisible = false;
            
            PlayerStats weightClass = _lightClass;
            Weight playerWeight = _airFillBoards[i].GetWeight();
            weightClass = playerWeight switch
            {
                Weight.Default => _defaultClass,
                Weight.Heavy => _heavyClass,
                _ => weightClass
            };
            
            _playerControllers[i].CurrentState = PlayerState.Fighting;
            ActivePlayersTracker.LookForPlayerSpawn(_playerControllers[i].ActivePlayer);
            if (_playerControllers[i].ActivePlayer is Player player)
            {
                player.AssignWeightClass(weightClass);
            }
        }
        onFinished.Invoke();
    }
    
    public enum Weight
    {
        Light,
        Default,
        Heavy
    }
}
