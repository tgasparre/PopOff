using System.Collections;
using UnityEngine;

public class Trailer_PlayerChanger : MiniGameInfo
{
    [SerializeField] private GameObject _particles;


    protected override void StartMiniGame()
    {
        StartCoroutine(StartTheChange());
    }

    IEnumerator StartTheChange()
    {
        yield return new WaitForSeconds(2f);
        foreach (PlayerController controller in _playerControllers)
        {
            controller.CurrentState = PlayerState.Fighting;
            ActivePlayersTracker.SpawnSinglePlayer(controller.ActivePlayer);
            Instantiate(_particles, controller.ActivePlayer.transform.position, Quaternion.identity);
        }
    }
}
