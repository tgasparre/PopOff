using System;
using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    public bool isLeftBound;
    
    [SerializeField] private Layout _layout = Layout.Horizontal;
    [SerializeField] private bool _extend = true;
    
    [SerializeField] private ParticleSystem _deathParticlesPrefab;
    private ParticleSystem _deathParticles;

    private void Awake()
    {
       AlterLayout();

       if (_extend)
       {
           Vector2 scale = transform.localScale;
           if (_layout == Layout.Horizontal)
           {
               scale.x *= 10f;
           }
           else scale.y *= 10f;
           transform.localScale = scale;
       }

       GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ContactPoint2D contact = collision.contacts[0];
            SpawnDeathParticles(contact.point);
            
            collision.gameObject.GetComponentInChildren<AttackHurtbox>().InstantDeath();
        }
    }

    private void AlterLayout()
    {
        Vector2 scale = transform.localScale;
        if (_layout == Layout.Horizontal)
        {
            scale.x = Mathf.Max(transform.localScale.x, transform.localScale.y);
            scale.y = Mathf.Min(transform.localScale.x, transform.localScale.y);
        }
        else
        {
            scale.x = Mathf.Min(transform.localScale.x, transform.localScale.y);
            scale.y = Mathf.Max(transform.localScale.x, transform.localScale.y);
        }
        transform.localScale = scale;
    }

    private void SpawnDeathParticles(Vector2 spawnPositon)
    {
        switch (_layout)
        {
            case Layout.Vertical:
                if (isLeftBound)
                    _deathParticles = Instantiate(_deathParticlesPrefab, spawnPositon, Quaternion.Euler(-15f,90f,-90f));
                else
                {
                    _deathParticles = Instantiate(_deathParticlesPrefab, spawnPositon, Quaternion.Euler(-165f,-90f,90f));
                }
                break;
            
            case Layout.Horizontal:
                _deathParticles = Instantiate(_deathParticlesPrefab, spawnPositon, Quaternion.Euler(-90f,0,0));
                break;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        AlterLayout();
    }
    #endif

    public enum Layout
    {
        Horizontal,
        Vertical
    }
}
