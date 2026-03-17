using System;
using System.Collections;
using UnityEngine;

public class ExplodeObject : MonoBehaviour
{
    [SerializeField] private float _explosionRadius = 3f;

    private float _growRate = 5f;
    private float _growDuration = 0.2f;

    private const float ExplosionDamage = 20f;
    private const float ExplosionForce = 1000f;
    
    private void Awake()
    {
        Die();
    }

    private void Die()
    {
        Collider2D[] colliers = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);
        foreach (Collider2D collier in colliers)
        {
            if (collier.gameObject.CompareTag("Player"))
            {
                Player hitPlayer = collier.gameObject.GetComponentInParent<Player>();
                float distanceFromCenter = Vector2.Distance(transform.position, hitPlayer.transform.position);
                hitPlayer.TakeDamage(Mathf.Round(distanceFromCenter/_explosionRadius * ExplosionDamage));
                
                Rigidbody2D rb = hitPlayer.GetComponent<Rigidbody2D>();
                Vector2 explosionDir = rb.position - (Vector2)transform.position;
                float explosionDistance = explosionDir.magnitude;
                explosionDir /= explosionDistance;
                rb.AddForce(explosionDir * ExplosionForce);
            }
        }

        StartCoroutine(GrowBeforeDeath());
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blueViolet;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
