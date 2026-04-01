using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class MiniGamePlacer : ObjectPlacer
{
    [Space]
    [SerializeField] private float _holdTime = 5f;
    [SerializeField] private float _minTimeToSpawn = 5f;
    [SerializeField] private float _maxTimeToSpawn = 10f;
    
    protected override void StartPlacing()
    {
        StartCoroutine(BeginPlace());
        return;

        IEnumerator BeginPlace()
        {
            while (_canPlace)
            {
                float waitingTime = Random.Range(_minTimeToSpawn, _maxTimeToSpawn);
                yield return new WaitForSeconds(waitingTime);
                if (_canPlace) Place(_holdTime);
            }
        }
    }
}