using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private bool _randomizeSpawns = true;
    [SerializeField] private bool _showIndicators = true;
    private PlayerSpawn[] _spawns;
    private List<PlayerSpawn> _spawnList;

    private void Awake()
    {
        _spawns = GetComponentsInChildren<PlayerSpawn>();
    }

    public void SpawnPlayers(PlayerBase[] players)
    {
        if (_randomizeSpawns)
        {
            _spawnList = new List<PlayerSpawn>(_spawns);
            foreach (PlayerBase player in players)
            {
                PlayerSpawn randomSpawn = _spawnList[Random.Range(0, _spawnList.Count)];
                randomSpawn.Spawn(player, _showIndicators);
                _spawnList.Remove(randomSpawn);
            }
        }
        else
        {
            foreach (PlayerBase player in players)
            {
                SpawnPlayer(player);
            }
        }
    }

    public void SpawnPlayer(PlayerBase player)
    {
        foreach (PlayerSpawn spawn in _spawns)
        {
            if (player.PlayerIndex == (int)spawn.Type)
            {
                spawn.Spawn(player, _showIndicators);
            }
        }
    }
}
