using System;
using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public const float VOL = 0.85f;
    public const float FADE_TIME = 0.33f;
    
    private LayeredAudio _layeredAudio;
    public LayeredAudio LayeredMusic
    {
        set
        {
            _layeredAudio = value;
            InitializeLayered();
        }
    }
    private AudioSource _introSource;
    private float _introOverlap;
    private MusicSource[] _sources;

    private void Awake()
    {
        _introSource = gameObject.AddComponent<AudioSource>();
        _introSource.volume = VOL;
        _introSource.playOnAwake = false;
        _introSource.loop = false;
    }

    private void InitializeLayered()
    {
        _introSource.clip = _layeredAudio.Intro;
        _introOverlap = _layeredAudio.IntroOverlap;

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
            source.outputAudioMixerGroup = AudioManager.Instance.MusicMixerGroup;

            _sources[i] = new MusicSource
            {
                order = _layeredAudio.Clips[i].order,
                volume = _layeredAudio.Clips[i].volume,
                source = source,
            };
        }
    }

    public void StopSong()
    {
        FadeOut(() =>
        {
            foreach (MusicSource source in _sources)
            {
                source.source.Stop();
            }
        });
    }

    public void StartSong()
    {
        foreach (MusicSource source in _sources)
        {
            source.source.volume = 0f;
            source.source.Play();
        }
    }

    public void PlaySong(int level, bool skipIntro)
    {
        if (!skipIntro && _introSource.clip != null) StartCoroutine(PlayIntro());
        else PlayLevel(level);
        return;
        
        IEnumerator PlayIntro()
        {
            _introSource.Play();
            yield return new WaitForSecondsRealtime(_introSource.clip.length - _introOverlap);
            PlayLevel(level);
        }
    }

    private void PlayLevel(int level)
    {
        foreach (MusicSource source in _sources)
        {
            float volume = source.order <= level ? source.volume : 0f; 
            Fade(source.source, volume);
        }
    }

    public void FadeOut(Action completed = null)
    {
        if (_sources.Length <= 0) return;

        Fade(_sources[0].source, 0f, completed);
        for (int i = 1; i < _sources.Length; i++)
        {
            Fade(_sources[i].source, 0f);
        }
    }

    private void Fade(AudioSource source, float target, Action completed = null)
    {
        if (source.volume == target) return;
        StartCoroutine(StartFade());
        return;
        
        IEnumerator StartFade()
        {
            float start = source.volume;
            
            float elapsed = 0f;
            while (elapsed <= FADE_TIME)
            {
                elapsed += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(start, target, elapsed / FADE_TIME);
                yield return null;
            }
            completed?.Invoke();
        }
    }

    private struct MusicSource
    {
        public int order;
        public float volume;
        public AudioSource source;
    }
}
