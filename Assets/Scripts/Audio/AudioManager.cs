using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public const int NUM_SFX_SOURCE = 4;
    
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
            if (!_audioDictionary.TryAdd(a.type, a))
            {
                throw new Exception($"Failed to add type {a.type} to AudioDictionary - check that no type duplicates exist");
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
    
    public static void PlaySound(AudioType type, float delay = 0f)
    {
        if (!Instance._audioDictionary.TryGetValue(type, out Audio audio))
        {
            throw new Exception($"Audio Type not found {type}, check if it is set up in AudioManager");
        }

        AudioSettings settings = audio.GetClip();
        settings.delay = delay;
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
        public AudioType type;
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

public enum AudioType
{
    Other,
    PlayerHit,
    PlayerMove,
    PlayerJump,
    PlayerDeath,
    PowerupThrow,
    GameStart,
    MinigameStart,
    Transition,
    Countdown,
    ButtonClick
}

#if UNITY_EDITOR
[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private int previousSize = -1;

    private SerializedProperty prefabProp;
    private SerializedProperty arrayProp;

    private void OnEnable()
    {
        prefabProp = serializedObject.FindProperty("_audioSourcePrefab");
        arrayProp = serializedObject.FindProperty("_audios");
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
        SerializedProperty volumeMin = element.FindPropertyRelative("_volumeMin");
        SerializedProperty volumeMax = element.FindPropertyRelative("_volumeMax");
        SerializedProperty pitchMin = element.FindPropertyRelative("_pitchMin");
        SerializedProperty pitchMax = element.FindPropertyRelative("_pitchMax");

        type.enumValueIndex = 0;
        volumeMin.floatValue = 1f;
        volumeMax.floatValue = 1f;
        pitchMin.floatValue = 1f;
        pitchMax.floatValue = 1f;

    }
}
#endif
