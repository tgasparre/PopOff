using System;
using TMPro;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _winnerNameText;

    public CanvasGroup canvasGroup { get; private set; }
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetWinnerName()
    {
        int winningIndex = Game.WinningIndex;
        if (winningIndex == -1) Debug.LogError("Error! Winning Index = -1, shouldn't happen!");
        _winnerNameText.text = GameUtils.PlayerNames[winningIndex];
    }
    
    public void RestartGame()
    {
        //TODO
        //play the game from the beginning without the need to have players join again or go to the main menu
    }

    public void ReturnToMenu()
    {
        Game.currentState = GameStates.Menu;
    }
}

