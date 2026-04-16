using System;
using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public const float VOL = 0.9f;
    public const float FADE_TIME = 0.2f;
    
    private LayeredAudio _layeredAudio;
    public LayeredAudio LayeredMusic
    {
        set
        {
            _layeredAudio = value;
            Initialize();
        }
    }

    private AudioSource _introSource;
    private MusicSource[] _sources;
    
    // private float _introDelay;
    // private AudioClip _intro;

    private void Awake()
    {
        _introSource = gameObject.AddComponent<AudioSource>();
        _introSource.volume = VOL;
        _introSource.playOnAwake = false;
        _introSource.loop = false;
    }

    private void Initialize()
    {
        // (_intro, _introDelay) = _layeredAudio.GetIntro();
        // _introSource.clip = _intro;

        _sources = new MusicSource[_layeredAudio.ClipLength];
        for (int i = 0; i < _sources.Length; i++)
        {
            GameObject sourceObject = new GameObject("musicSource");
            sourceObject.transform.parent = transform;
            AudioSource source = sourceObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            source.clip = _layeredAudio.Clips[i].clip;
            source.volume = 0f;

            _sources[i] = new MusicSource
            {
                order = _layeredAudio.Clips[i].order,
                source = source,
            };
        }

        foreach (MusicSource source in _sources)
        {
            source.source.Play();
        }
    }

    public void PlayAll()
    {
        ChangeLevel(100);
    }

    public void ChangeLevel(int level)
    {
        foreach (MusicSource source in _sources)
        {
            float volume = source.order <= level ? VOL : 0f; 
            Fade(source.source, volume);
        }
    }

    public void FadeOut()
    {
        foreach (MusicSource source in _sources)
        {
            Fade(source.source, 0f);
        }
    }

    private void Fade(AudioSource source, float target)
    {
        if (source.volume == target) return;
        StartCoroutine(StartFade(source, target));
        return;
        
        static IEnumerator StartFade(AudioSource source, float target)
        {
            float start = source.volume;
            
            float elapsed = 0f;
            while (elapsed <= FADE_TIME)
            {
                elapsed += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(start, target, elapsed / FADE_TIME);
                yield return null;
            }
        }
    }

    private struct MusicSource
    {
        public int order;
        public AudioSource source;
    }
}
