using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerupSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] powerups;
    private List<GameObject> notUsed = new List<GameObject>();

    private void Start()
    {
        DisableAll();
        InvokeRepeating(nameof(SpawnPowerup), 3f, 8f);
    }

    private void SpawnPowerup()
    {
        if (notUsed.Count == 0) DisableAll();
        int random = Random.Range(0, notUsed.Count);
        notUsed[random].SetActive(true);
        notUsed.Remove(notUsed[random]);
    }

    private void DisableAll()
    {
        notUsed = powerups.ToList();
        powerups.ToList().ForEach(t => t.SetActive(false));
    }
}
