using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "layered audio", menuName = "LayeredAudio")]
public class LayeredAudio : ScriptableObject
{
    // [SerializeField] private AudioClip _intro;
    // [SerializeField] [Tooltip("Time for the intro to fade out and the looped music to start")] private float _introFadeTime = 1f;
    [SerializeField] private AudioLayer[] _clips;

    public int ClipLength => _clips.Length;
    public AudioLayer[] Clips => _clips;

    // public (AudioClip, float) GetIntro()
    // {
    //     return (_intro, _introFadeTime);
    // }
}

[System.Serializable]
public struct AudioLayer
{
    public int order;
    public AudioClip clip;
}
