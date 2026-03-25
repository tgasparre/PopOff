using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private const int NUM_SFX_SOURCE = 4;
    
    [SerializeField] private GameObject _audioSourcePrefab;
    [SerializeField] private Audio[] _audios;
    
    private AudioPool[] _sources;
    private Queue<AudioSettings> _soundQueue = new Queue<AudioSettings>();
    private Dictionary<AudioType, Audio> _audioDictionary = new Dictionary<AudioType, Audio>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _sources = new AudioPool[NUM_SFX_SOURCE];
        for (int i = 0; i < NUM_SFX_SOURCE; i++)
        {
            GameObject source = Instantiate(_audioSourcePrefab, transform);
            _sources[i] = source.GetComponent<AudioPool>();
            _sources[i].OnSoundFinish += OnSoundFinish;
        }

        foreach (Audio a in _audios)
        {
            _audioDictionary.TryAdd(a.type, a);
        }
    }

    private void OnDestroy()
    {
        foreach (AudioPool source in _sources)
        {
            source.OnSoundFinish -= OnSoundFinish;
        }
    }
    
    public static void PlaySound(AudioType type)
    {
        AudioSettings settings = Instance._audioDictionary[type].GetClip();
        PlaySound(settings);
    }
    
    public static void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f, float delay = 0f)
    {
        AudioSettings settings = new AudioSettings(clip, volume, pitch, delay);
        PlaySound(settings);
    }

    private static void PlaySound(AudioSettings settings)
    {
        foreach (AudioPool source in Instance._sources)
        {
            if (!source.IsPlaying)
            {
                source.PlaySound(settings);
                return;
            }
        }
        
        Instance._soundQueue.Enqueue(settings);
    }

    private void OnSoundFinish(AudioPool finished)
    {
        if (_soundQueue.Count > 0)
        {
            AudioSettings settings = _soundQueue.Dequeue();
            finished.PlaySound(settings);
        }
    }

    public struct AudioSettings
    {
        public readonly AudioClip clip;
        public readonly float volume;
        public readonly float pitch;
        public readonly float delay;

        public AudioSettings(AudioClip clip, float volume = 1f, float pitch = 1f, float delay = 0f)
        {
            this.clip = clip;
            this.volume = volume;
            this.pitch = pitch;
            this.delay = delay;
        }
    }

    [System.Serializable]
    public class Audio
    {
        public AudioType type = AudioType.Other;
        [SerializeField] private AudioClip[] _clips;
        [SerializeField] [Range(0f, 1f)] private float _volumeMin = 1f;
        [SerializeField] [Range(0f, 1f)] private float _volumeMax = 1f;
        [SerializeField] [Range(-3f, 3f)] private float _pitchMin = 1f;
        [SerializeField] [Range(-3f, 3f)] private float _pitchMax = 1f;

        public AudioSettings GetClip()
        {
            //TODO 
            // make the random clips not be the same ones - avoid repeats up to two
            
            AudioClip clip = _clips[Random.Range(0, _clips.Length)];
            float volume = Random.Range(_volumeMin, _volumeMax);
            float pitch = Random.Range(_pitchMin, _pitchMax);
            AudioSettings settings = new AudioSettings(clip, volume, pitch);
            return settings;
        }
    }
}

public enum AudioType
{
    PlayerHit,
    PlayerMove,
    Transition,
    Countdown,
    ButtonClick,
    PlayerDeath,
    PlayerJump,
    PowerupThrow,
    GameStart,
    MinigameStart,
    Other
}
