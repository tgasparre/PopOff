using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class ObjectPlacer : MonoBehaviour
{
    public static bool IsFrozen = false;
    
    [SerializeField] private int _delayUntilStartTime = 5;
    
    [Space]
    [SerializeField] private GameObject _prefab;
    [SerializeField] protected int _maxNumberSpawned = 1;
    
    [Space]
    [SerializeField] private Transform[] _spawnPositions;

    protected bool _canPlace = false;
    protected int _currentNumberSpawned = 0;
    protected Collectable[] _spawned;

    private IEnumerator Start()
    {
        if (IsFrozen) yield break;
        
        _spawned = new Collectable[_maxNumberSpawned];
        yield return new WaitForSeconds(_delayUntilStartTime);
        _canPlace = true;
        StartPlacing();
    }

    private void OnDestroy()
    {
        _canPlace = false;
    }

    protected abstract void StartPlacing();

    protected void Place(float timeToExist = 0f)
    {
        if (_currentNumberSpawned >= _maxNumberSpawned) return;
        
        Vector3 spawnLocation = _spawnPositions[Random.Range(0, _spawnPositions.Length)].position;
        GameObject collect = Instantiate(_prefab, spawnLocation, Quaternion.identity);
        
        Collectable collectable = collect.GetComponent<Collectable>();
        collectable.Spawned(timeToExist);
        collectable.OnDeath += () => { _currentNumberSpawned--; };

        _spawned[_currentNumberSpawned] = collectable;
        
        _currentNumberSpawned++;
    }

    public void StopPlacing()
    {
        _canPlace = false;
        DestroyAll();
    }
    
    protected void DestroyAll()
    {
        foreach (Collectable collectable in _spawned)
        {
            Destroy(collectable.gameObject);
        }
    }
}