using System;
using UnityEngine;

public class StartingMiniGame : MiniGameInfo
{
    [Space]
    [SerializeField] private AirFillBoard[] _airFillBoards;
    [SerializeField] private float _fillRate = 0.1f;
    [SerializeField] private float _deflateRate = 0.2f;
    [Space]
    [SerializeField] private PlayerStats _lightClass;
    [SerializeField] private PlayerStats _defaultClass;
    [SerializeField] private PlayerStats _heavyClass;

    private void SetFillBoards(bool isVisible)
    {
        foreach (AirFillBoard board in _airFillBoards)
        {
            board.IsVisible = isVisible;
            board.SetValues(_fillRate, _deflateRate);
        }
    }

    private void Start()
    {
        SetFillBoards(false);
    }

    protected override void StartMiniGame()
    {
        //assign jump input to fill 
        for (int i = 0; i < _playerControllers.Length; i++)
        {
            _airFillBoards[i].IsVisible = true;
            _playerControllers[i].OnJump += _airFillBoards[i].Fill;
        }
    }

    protected override void ShowMiniGameResults(Action onFinished, string reward)
    {
        //TODO fun animation
        for (int i = 0; i < _playerControllers.Length; i++)
        {
            float fillAmount = _airFillBoards[i].FillAmount;
            PlayerStats weightClass = _lightClass;
            if (fillAmount < 0.66f) weightClass = _defaultClass;
            if (fillAmount < 0.33f) weightClass = _heavyClass;
            _playerControllers[i].CurrentState = PlayerState.Fighting;
            ActivePlayersTracker.LookForPlayerSpawn(_playerControllers[i].ActivePlayer);
            _playerControllers[i].AssignWeightClass(weightClass);
        }
        onFinished.Invoke();
    }
}
