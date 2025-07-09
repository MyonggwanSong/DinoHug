using System;
using UnityEngine;

public class AudioManager : BehaviourSingleton<AudioManager>
{
    protected override bool IsDontDestroy() => false;

    //배경 음악 
    [Header("Audio Source")]
    [SerializeField] AudioSource masterAudio;
    [SerializeField] AudioSource effectAudio;

    [Header("Volume Test")]
    [Range(0, 1)] public float master_Volume;
    [Range(0, 1)] public float effect_Volume;

    [Header("Effect volume Test Clip")]
    AudioClip effectTester;

    void Start()
    {
        masterAudio.volume = master_Volume;
        effectAudio.volume = effect_Volume;
    }

    void Update()
    {
        masterAudio.volume = master_Volume;
        effectAudio.volume = effect_Volume;
    }
}