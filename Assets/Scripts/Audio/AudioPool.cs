using System;
using System.Collections;
using UnityEngine;

public class AudioPool : MonoBehaviour
{
    private AudioSource _source;
    public bool IsPlaying { get; private set; } = false;

    public event Action<AudioPool> OnSoundFinish;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioManager.AudioSettings settings)
    {
        _source.volume = settings.volume;
        _source.pitch = settings.pitch;
        _source.clip = settings.clip;

        float delay = settings.delay;
        if (delay == 0f) _source.Play();
        else _source.PlayDelayed(delay);
        
        StartCoroutine(PlayingDuration(settings.clip.length, delay));
    }

    IEnumerator PlayingDuration(float duration, float delay = 0f)
    {
        IsPlaying = true;
        yield return new WaitForSeconds(duration + delay);
        IsPlaying = false;
        OnSoundFinish?.Invoke(this);
    }
}
