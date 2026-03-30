using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private GameObject _playerUIPrefab;
    [SerializeField] private Transform _playerUITransform;
    private List<PlayerUIDisplayer> ui = new List<PlayerUIDisplayer>();
    
    [Space]
    [SerializeField] private Sprite _mousePlayerSprite;
    [SerializeField] private Sprite _dogPlayerSprite;
    
    public PlayerUIDisplayer CreatePlayerUI(PlayerController player)
    {
        GameObject uiInstance = Instantiate(_playerUIPrefab, _playerUITransform);
        PlayerUIDisplayer playerUIDisplayer = uiInstance.GetComponent<PlayerUIDisplayer>();
        Player activePlayer = player.ActivePlayer as Player;
        Sprite playerSprite = Game.Instance.PlayerTypes[activePlayer.PlayerIndex] switch
        {
            PlayerType.Mouse => _mousePlayerSprite,
            PlayerType.Dog => _dogPlayerSprite,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        playerUIDisplayer.InitializePlayerUI(activePlayer, playerSprite);
        ui.Add(playerUIDisplayer);
        return playerUIDisplayer;
    }

    public void DestroyUI()
    {
        foreach (PlayerUIDisplayer displayer in ui)
        {
            Destroy(displayer.gameObject);
        }
        ui.Clear();
    }
}
