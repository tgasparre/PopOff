using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    private bool hasBeenSteppedOn = false;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!hasBeenSteppedOn && other.gameObject.CompareTag("Player"))
        {
            hasBeenSteppedOn = true;
            StartCoroutine(FallAfterTime(2f));
        }
    }

    IEnumerator FallAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        rb.bodyType = RigidbodyType2D.Dynamic;
        
        //slight delay so player doesn't fall through platform
        yield return new WaitForSeconds(0.2f); 
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
