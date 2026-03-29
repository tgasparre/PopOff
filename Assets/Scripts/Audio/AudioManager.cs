using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public const int NUM_SFX_SOURCE = 8;
    private const float CROSS_FADE_TIME = 1.25f;
    
    [SerializeField] private GameObject _audioSourcePrefab;
    [SerializeField] private Audio[] _audios;
    
    private AudioPool[] _sources;
    private Queue<AudioSettings> _soundQueue = new Queue<AudioSettings>();
    private Dictionary<AudioTrack, Audio> _audioDictionary = new Dictionary<AudioTrack, Audio>();

    private AudioSource _musicPlayer;
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _gameMusic;
    [SerializeField] private AudioClip _minigameMusic;

    private Coroutine _crossfadeCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _musicPlayer = GetComponent<AudioSource>();

        _sources = new AudioPool[NUM_SFX_SOURCE];
        for (int i = 0; i < NUM_SFX_SOURCE; i++)
        {
            GameObject source = Instantiate(_audioSourcePrefab, transform);
            _sources[i] = source.GetComponent<AudioPool>();
            _sources[i].OnSoundFinish += OnSoundFinish;
        }

        foreach (Audio a in _audios)
        {
            if (!_audioDictionary.TryAdd(a.track, a))
            {
                if (a.track == AudioTrack.Other) continue;
                throw new Exception($"Failed to add type {a.track} to AudioDictionary - check that no type duplicates exist");
            }
        }
    }

    private void OnDestroy()
    {
        foreach (AudioPool source in _sources)
        {
            source.OnSoundFinish -= OnSoundFinish;
        }
    }
    
    #region SFX
    public static void PlaySound(AudioTrack track, float delay = 0f)
    {
        if (!Instance._audioDictionary.TryGetValue(track, out Audio audio))
        {
            Debug.LogWarning($"Audio Type not found {track}, check if it is set up in AudioManager");
            return;
        }

        AudioSettings settings = audio.GetClip();
        settings.delay = delay;
        PlaySound(settings);
    }

    public static void PlaySound(Audio audio, float delay = 0f)
    {
        AudioSettings settings = audio.GetClip();
        settings.delay = delay;
        PlaySound(settings);
    }

    public void PlayUISound()
    {
        PlaySound(AudioTrack.ButtonClick);
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
            
            if (Instance._soundQueue.Count >= 5) Debug.LogWarning("The sound queue exceeds 5 sounds, consider adding more audio sources");
        }
    }
    #endregion
    
    #region Music

    public void SwitchMusic(MusicType type)
    {
        AudioClip song = type switch
        {
            MusicType.Menu => _menuMusic,
            MusicType.Game => _gameMusic,
            MusicType.Minigame => _minigameMusic,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        _crossfadeCoroutine ??= StartCoroutine(CrossFadeMusic(song));
    }

    private IEnumerator CrossFadeMusic(AudioClip newSong)
    {
        float elapsed = 0f;
        float startVolume = _musicPlayer.volume;
        while (elapsed < CROSS_FADE_TIME)
        {
            elapsed += Time.unscaledDeltaTime;
            _musicPlayer.volume = Mathf.Lerp(startVolume, 0f, elapsed / CROSS_FADE_TIME);
            yield return null;
        }

        _musicPlayer.volume = 0f;
        _musicPlayer.clip = newSong;

        elapsed = 0f;
        while (elapsed < CROSS_FADE_TIME)
        {
            elapsed += Time.unscaledDeltaTime;
            _musicPlayer.volume = Mathf.Lerp(0f, startVolume, elapsed / CROSS_FADE_TIME);
            yield return null;
        }
        
        _crossfadeCoroutine = null;
    }
    
    #endregion

    public struct AudioSettings
    {
        public readonly AudioClip clip;
        public readonly float volume;
        public readonly float pitch;
        public float delay;

        public AudioSettings(AudioClip clip, float volume = 1f, float pitch = 1f, float delay = 0f)
        {
            this.clip = clip;
            this.volume = volume;
            this.pitch = pitch;
            this.delay = delay;
        }
    }

    [Serializable]
    public struct Audio
    {
        public string name; //editor use only
        public AudioTrack track;
        [SerializeField] private AudioClip[] _clips;
        [SerializeField] [Range(0f, 1f)] private float _volumeMin;
        [SerializeField] [Range(0f, 1f)] private float _volumeMax;
        [SerializeField] [Range(-3f, 3f)] private float _pitchMin;
        [SerializeField] [Range(-3f, 3f)] private float _pitchMax;
        
        public AudioSettings GetClip()
        {
            AudioClip clip = _clips[Random.Range(0, _clips.Length)];
            float volume = Random.Range(_volumeMin, _volumeMax);
            float pitch = Random.Range(_pitchMin, _pitchMax);
            AudioSettings settings = new AudioSettings(clip, volume, pitch);
            return settings;
        }
    }
}

public enum AudioTrack
{
    Other,
    PlayerHit,
    PlayerMove,
    PlayerJump,
    PlayerDeath,
    PlayerUltimate,
    PlayerAppear,
    PowerupThrow,
    PowerupThud,
    PowerupExplode,
    GameStart,
    MinigameStart,
    MinigameEnd,
    MinigameTransition,
    MinigameWhistle,
    Transition,
    Countdown,
    ButtonClick,
}

public enum MusicType
{
    Menu,
    Game,
    Minigame
}

#if UNITY_EDITOR
[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private int previousSize = -1;

    private SerializedProperty prefabProp;
    private SerializedProperty arrayProp;

    private SerializedProperty menuMusic;
    private SerializedProperty gameMusic;
    private SerializedProperty minigameMusic;

    private void OnEnable()
    {
        prefabProp = serializedObject.FindProperty("_audioSourcePrefab");
        arrayProp = serializedObject.FindProperty("_audios");
        
        menuMusic = serializedObject.FindProperty("_menuMusic");
        gameMusic = serializedObject.FindProperty("_gameMusic");
        minigameMusic = serializedObject.FindProperty("_minigameMusic");
    }

    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((AudioManager)target), typeof(AudioManager), false);
        EditorGUILayout.FloatField(label: "Total SFX Sources", AudioManager.NUM_SFX_SOURCE);
        GUI.enabled = true;
        
        EditorGUILayout.Space(10f);
        
        serializedObject.Update();

        EditorGUILayout.PropertyField(prefabProp);
        
        EditorGUILayout.Space(10f);
        EditorGUILayout.PropertyField(menuMusic);
        EditorGUILayout.PropertyField(gameMusic);
        EditorGUILayout.PropertyField(minigameMusic);
        EditorGUILayout.Space(10f);
        
        if (previousSize == -1)
        {
            previousSize = arrayProp.arraySize;
        }

        EditorGUI.BeginChangeCheck();
        
        EditorGUILayout.PropertyField(arrayProp, true);

        if (EditorGUI.EndChangeCheck())
        {
            if (arrayProp.arraySize > previousSize)
            {
                for (int i = previousSize; i < arrayProp.arraySize; i++)
                {
                    SerializedProperty element = arrayProp.GetArrayElementAtIndex(i);
                    Initialize(element);
                }
            }
            previousSize = arrayProp.arraySize;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private static void Initialize(SerializedProperty element)
    {
        SerializedProperty type = element.FindPropertyRelative("type");
        SerializedProperty array = element.FindPropertyRelative("_clips");
        SerializedProperty volumeMin = element.FindPropertyRelative("_volumeMin");
        SerializedProperty volumeMax = element.FindPropertyRelative("_volumeMax");
        SerializedProperty pitchMin = element.FindPropertyRelative("_pitchMin");
        SerializedProperty pitchMax = element.FindPropertyRelative("_pitchMax");

        type.enumValueIndex = 0;
        array.arraySize = 0;
        volumeMin.floatValue = 1f;
        volumeMax.floatValue = 1f;
        pitchMin.floatValue = 1f;
        pitchMax.floatValue = 1f;

    }
}
#endif
