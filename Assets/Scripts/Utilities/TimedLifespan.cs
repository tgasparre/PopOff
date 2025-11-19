using System.Collections;
using UnityEngine;

public class TimedLifespan : MonoBehaviour
{
    public float secondsOnScreen;
    public virtual void Start()
    {
        StartCoroutine(CountDownTilDeath());
    }

    private IEnumerator CountDownTilDeath()
    {
        yield return new WaitForSeconds(secondsOnScreen);
        Destroy(gameObject);
    }
}
