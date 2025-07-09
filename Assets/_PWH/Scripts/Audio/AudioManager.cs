using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : BehaviourSingleton<AudioManager>
{
    protected override bool IsDontDestroy() => true;

    [SerializeField] AudioMixer mixer;

    //배경 음악 
    [Header("Audio Source")]
    [SerializeField] AudioSource masterAudio;
    [SerializeField] AudioSource effectAudio;

    [Header("Init Volume")]
    [Range(-80, 0)] public float init_Master;
    [Range(-80, 0)] public float init_Effect;

    public void PlayMaster(AudioClip clip)
    {
        masterAudio.clip = clip;
    }

    public void PlayEffect(AudioClip clip)
    {
        effectAudio.clip = clip;
    }

    public void SetVolume(SoundType type, float volume)
    {
        mixer.SetFloat(type.ToString(), volume);
    }
}

public enum SoundType {
    BGM, EFFECT,
}