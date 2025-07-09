using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : SingletonBehaviour<AudioManager>
{
    protected override bool IsDontDestroy() => true;

    [SerializeField] AudioMixer mixer;

    //배경 음악 
    [Header("Audio Source")]
    [SerializeField] AudioSource masterAudio;
    [SerializeField] List<AudioClip> sfxClips = new List<AudioClip>();
    [SerializeField] 

    [Header("Init Volume")]
    [Range(-80, 0)] public float init_Master;
    [Range(-80, 0)] public float init_SFX;

    public void PlayMaster(AudioClip clip)
    {
        masterAudio.clip = clip;
    }

    public void PlaySFX(string clipName, Vector3 pos)
    {
        int find = sfxClips.FindIndex(x => x.name == clipName);
        if (find == -1)
        {
            Debug.Log($"{clipName} 라는 이름의 효과음은 없습니다.");
            return;
        }
        AudioClip clip = sfxClips[find];

        //effectAudio.clip = clip;
    }
    public void PlaySFX(string clipName, Transform transform, bool isTracking)
    {
        int find = sfxClips.FindIndex(x => x.name == clipName);
        if (find == -1)
        {
            Debug.Log($"{clipName} 라는 이름의 효과음은 없습니다.");
            return;
        }
        AudioClip clip = sfxClips[find];
    }

    public void SetVolume(SoundType type, float volume)
    {
        mixer.SetFloat(type.ToString(), volume);
    }
}

public enum SoundType {
    BGM, EFFECT,
}