using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class DartDodgeMiniGame : MiniGameInfo
{
    [Header("References")]
    [SerializeField] private FallingDart[] _dartPrefabPool;
    [SerializeField] private FallingDart[] _hiddenDartPrefab;
    [Space]
    [SerializeField] private GameObject[] _spawnPoints;
    [SerializeField] private GameObject[] _hiddenSpawnPoints;
    [Tooltip("How often should a new dart spawn")] [SerializeField] private float _spawnRate = 0.25f;
    [Tooltip("How much should the spawn rate decrease when the difficulty is raised")] [SerializeField] private float _decreaseRate = 0.05f;
    [Tooltip("How often should the difficulty be raised")] [SerializeField] private float _difficultyRate = 2f;
    [Tooltip("How fast should the darts fall")] [SerializeField] private float _dartFallRate = .8f;
    [Tooltip("How much should the dart fall speed increase when the difficulty is raised")] [SerializeField] private float _dartFallIncreaseRate = 0.1f;
    
    private const float SpawnRateLimit = 0.025f;
    private const float DartFallLimit = 1.5f;

    private int _dartPoolIndex = 0;
    
    protected override void StartMiniGame()
    {
        StartCoroutine(SpawnDarts());
        StartCoroutine(SpawnHiddenDarts());
        StartCoroutine(Difficulty());
    }

    private IEnumerator SpawnDarts()
    {
        while (_isPlayingMiniGame)
        {
            yield return new WaitForSeconds(_spawnRate);
            int randomSpawn = GetRandomSpawn();
            Vector2 pos = _spawnPoints[randomSpawn].transform.position;
            _dartPrefabPool[_dartPoolIndex%_dartPrefabPool.Length].Spawn(pos, _dartFallRate);
            _dartPoolIndex++;
            yield return null;
        }
    }

    private IEnumerator SpawnHiddenDarts()
    {
        while (_isPlayingMiniGame)
        {
            yield return new WaitForSeconds(1f);
            int hiddenIndex = 0;
            foreach (GameObject point in _hiddenSpawnPoints)
            {
                _hiddenDartPrefab[hiddenIndex++].Spawn(point.transform.position, _dartFallRate);
            }
            yield return null;
        }
    }

    private IEnumerator Difficulty()
    {
        while (_isPlayingMiniGame)
        {
            yield return new WaitForSeconds(_difficultyRate);
            _spawnRate = Mathf.Max(SpawnRateLimit, _spawnRate - _decreaseRate);
            _dartFallRate = Mathf.Min(_dartFallRate + _dartFallIncreaseRate, DartFallLimit);
            yield return null;
        }
    }

    protected override void OnEndMiniGame()
    {
        foreach (FallingDart dart in _dartPrefabPool)
        {
            dart.ReturnToPool();
        }
    }

    private int GetRandomSpawn()
    {
        return Random.Range(0, _spawnPoints.Length);
    }
}
