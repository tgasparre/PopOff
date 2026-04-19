using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "layered audio", menuName = "LayeredAudio")]
public class LayeredAudio : ScriptableObject
{
    [SerializeField] private AudioClip _intro;
    [SerializeField] private float _introOverlap = 0.75f;
    [SerializeField] private AudioLayer[] _clips;

    public AudioClip Intro => _intro;
    public float IntroOverlap => _introOverlap;
    public int ClipLength => _clips.Length;
    public AudioLayer[] Clips => _clips;
}

[System.Serializable]
public struct AudioLayer
{
    public int order;
    [Range(0,1)] public float volume;
    public AudioClip clip;
}
