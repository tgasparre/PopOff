using System;
using UnityEngine;

public class Trailer_Tilt : MonoBehaviour
{
    [SerializeField] private float _amp;
    [SerializeField] private float _freq;
    private float _startTilt;

    private void Awake()
    {
        _startTilt = transform.eulerAngles.z;
    }

    private void Update()
    {
        Vector3 rot = transform.eulerAngles;
        rot.z = _startTilt + _amp * Mathf.Sin(_freq * Time.time);
        transform.eulerAngles = rot;
    }
}
