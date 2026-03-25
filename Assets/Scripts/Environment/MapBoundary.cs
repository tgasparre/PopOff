using System;
using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    [SerializeField] private Layout _layout = Layout.Horizontal;
    [SerializeField] private bool _extend = true;

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
