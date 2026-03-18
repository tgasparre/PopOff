using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private GameObject _playerUIPrefab;
    [SerializeField] private Transform _playerUITransform;
    private List<PlayerUIDisplayer> ui = new List<PlayerUIDisplayer>();
    
    public PlayerUIDisplayer CreatePlayerUI(PlayerController player)
    {
        GameObject uiInstance = Instantiate(_playerUIPrefab, _playerUITransform);
        PlayerUIDisplayer playerUIDisplayer = uiInstance.GetComponent<PlayerUIDisplayer>();
        
        playerUIDisplayer.InitializePlayerUI(player.ActivePlayer as Player);
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
