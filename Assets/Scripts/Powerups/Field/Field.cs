using System;
using UnityEngine;

public class Field : MonoBehaviour
{
    public float debug_force = 100f;
    private float _fieldForce;
    
    public void StartField(float force)
    {
        _fieldForce = force;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("enter: " + other.name);
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("found player");
            Rigidbody2D otherRB = other.transform.parent.GetComponent<Rigidbody2D>();
            otherRB.AddForce(otherRB.linearVelocity * -1 * _fieldForce * debug_force);
        }
    }
}
