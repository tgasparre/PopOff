using System;
using System.Collections;
using UnityEngine;

public class ExplodeObject : MonoBehaviour
{
    private float _growRate = 5f;
    private float _growDuration = 0.2f;

    [SerializeField] private bool _grow = false;

    public void StartDie(Explode explosion)
    {
        Collider2D[] colliers = Physics2D.OverlapCircleAll(transform.position, explosion.Radius);
        foreach (Collider2D collier in colliers)
        {
            if (collier.gameObject.CompareTag("Player"))
            {
                Player hitPlayer = collier.gameObject.GetComponentInParent<Player>();
                float distanceFromCenter = Vector2.Distance(transform.position, hitPlayer.transform.position);
                hitPlayer.TakeDamage(Mathf.RoundToInt(distanceFromCenter / explosion.Radius * explosion.Damage));

                Rigidbody2D rb = hitPlayer.GetComponent<Rigidbody2D>();
                Vector2 explosionDir = rb.position - (Vector2)transform.position;
                float explosionDistance = explosionDir.magnitude;
                explosionDir /= explosionDistance;
                rb.AddForce(explosionDir * explosion.Force);
            }
        }

        if (_grow) StartCoroutine(GrowBeforeDeath());
        else Destroy(gameObject, 0.5f);
    }

    private IEnumerator GrowBeforeDeath()
    {
        float elapsed = 0;
        while (elapsed <= _growDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale += Vector3.one * _growRate * Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.blueViolet;
    //     Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    // }
}
