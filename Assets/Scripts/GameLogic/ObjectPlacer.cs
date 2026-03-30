using System;
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

    // [SerializeField, Range(0,1)] private float _precentToSpawn = 0.5f;
    // [SerializeField] private Vector2 _spawnInterval = new Vector2(1, 2);
    
    private GameObject[] _spawned;

    private void Start()
    {
        Invoke(nameof(StartPlacing), _delayUntilStartTime);
    }

    protected abstract void StartPlacing();

    protected void Place()
    {
        Vector3 spawnLocation = _spawnPositions[Random.Range(0, _spawnPositions.Length)].position;
        Instantiate(_prefab, spawnLocation, Quaternion.identity);
    }

    protected void EndPlacing()
    {
        foreach (GameObject spawnedObject in _spawned)
        {
            Destroy(spawnedObject);
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