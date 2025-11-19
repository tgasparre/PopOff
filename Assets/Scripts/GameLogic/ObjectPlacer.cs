using System.Collections;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public GameObject ObjectPrefab;

    public float minimumSecondsToCreate = 0;
    public float maximumSecondsToCreate = 0;
    public string TagToClean = "";

    private bool isWaitingToCreate = false;
    private Coroutine CountdownCoroutine;
    private bool isGamePlaying = false;
    public void Update()
    {
        if (isGamePlaying)
        {
            if (!isWaitingToCreate)
            {
                CountdownCoroutine = StartCoroutine(CountdownUntilCreation());
            }
        }
    }

    public void StartPlacing()
    {
        isGamePlaying = true;
        Debug.Log("Set startplacing to true");
    }

    public void StopPlacing()
    {
        isGamePlaying = false;
        if (CountdownCoroutine != null)
        {
            StopCoroutine(CountdownCoroutine);
        }
    }

    IEnumerator CountdownUntilCreation()
    {
        isWaitingToCreate = true;
        float secondsToWait = Random.Range(minimumSecondsToCreate,
            maximumSecondsToCreate);
        yield return new WaitForSeconds(secondsToWait);
        Place();
        isWaitingToCreate = false;
    }
    
    public virtual void Place()
    {
        //pick place
        //instantiate
        Debug.Log("placed object");
        Vector3 position = SpriteTools.RandomLocationWorldSpace();
        Instantiate(ObjectPrefab, position, Quaternion.identity);
    }

    public void Reset()
    {
        foreach (GameObject placedObject in GameObject.FindGameObjectsWithTag(TagToClean))
        {
            Destroy(placedObject);
        }
        if (CountdownCoroutine != null)
        {
            StopCoroutine(CountdownCoroutine);
        }
        isWaitingToCreate = false;
    }
}

