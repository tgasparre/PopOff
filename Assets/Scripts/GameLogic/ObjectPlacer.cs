using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class ObjectPlacer : MonoBehaviour
{
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
    
    protected void DestroyAll()
    {
        foreach (Collectable collectable in _spawned)
        {
            Destroy(collectable.gameObject);
        }
    }
}

// public GameObject ObjectPrefab;
//
// public float minimumSecondsToCreate = 0;
// public float maximumSecondsToCreate = 0;
// public string TagToClean = "";
//
// private bool isWaitingToCreate = false;
// private Coroutine CountdownCoroutine;
// private bool isGamePlaying = false;
// public void Update()
// {
//     if (isGamePlaying)
//     {
//         if (!isWaitingToCreate)
//         {
//             CountdownCoroutine = StartCoroutine(CountdownUntilCreation());
//         }
//     }
// }
//
// public void StartPlacing()
// {
//     isGamePlaying = true;
//     Debug.Log("Set startplacing to true");
// }
//
// public void StopPlacing()
// {
//     isGamePlaying = false;
//     if (CountdownCoroutine != null)
//     {
//         StopCoroutine(CountdownCoroutine);
//     }
// }
//
// IEnumerator CountdownUntilCreation()
// {
//     isWaitingToCreate = true;
//     float secondsToWait = Random.Range(minimumSecondsToCreate,
//         maximumSecondsToCreate);
//     yield return new WaitForSeconds(secondsToWait);
//     Place();
//     isWaitingToCreate = false;
// }
//     
// public virtual void Place()
// {
//     //pick place
//     //instantiate
//     Debug.Log("placed object");
//     Vector3 position = SpriteTools.RandomLocationWorldSpace();
//     Instantiate(ObjectPrefab, position, Quaternion.identity);
// }
//
// public void Reset()
// {
//     foreach (GameObject placedObject in GameObject.FindGameObjectsWithTag(TagToClean))
//     {
//         Destroy(placedObject);
//     }
//     if (CountdownCoroutine != null)
//     {
//         StopCoroutine(CountdownCoroutine);
//     }
//     isWaitingToCreate = false;
// }