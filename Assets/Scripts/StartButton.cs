using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] private GameObject _joinText;
    [SerializeField] private CountdownUI _startGameCountdown;
    [SerializeField] private int _defaultStartTime = 3;


    private void Awake()
    {
        _startGameCountdown.gameObject.SetActive(false);
        ResetCoroutine();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_startGameCountdown.isRunning || (Game.PlayerCount <= 1 && !Game.Instance.bypassOnePlayerBlock)) return;
        _joinText.SetActive(false);
        _startGameCountdown.gameObject.SetActive(true);
        _startGameCountdown.StartCountdown(StartGame, 0.5f);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        ResetCoroutine();
    }
    
    private void ResetCoroutine()
    {
        if (_startGameCountdown == null) return;
        _joinText.SetActive(true);
        _startGameCountdown.StopCountdownNoTrigger();
        _startGameCountdown.InitializeCountdown(_defaultStartTime);
        _startGameCountdown.gameObject.SetActive(false);
    }

    private void StartGame()
    {
        Game.currentState = GameStates.Playing;
    }
}
