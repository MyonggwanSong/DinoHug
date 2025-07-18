using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioManager : BehaviourSingleton<AudioManager>
{
    protected override bool IsDontDestroy() => false;
    [SerializeField] AudioMixer mixer;
    //배경 음악 
    [Header("Audio Source")]
    [SerializeField] AudioSource bgmAudio;
    //효과 음악
    [SerializeField] List<AudioClip> effectAudio;
    [SerializeField] SFX sfxPrefab;

    [Header("Init Volume")]
    [Range(-80, 0)] public float init_Master;
    [Range(-80, 0)] public float init_Effect;

    public void PlayBGM(AudioClip clip)
    {
        bgmAudio.clip = clip;
        bgmAudio.Play();
    }

    public void StopBGM()
    {
        bgmAudio.Stop();
    }

    public void SetVolume(SoundType type, float volume)
    {
        mixer.SetFloat(type.ToString(), volume);
    }

    // 외부에서 효과음을 재생시킬때 쓰는 메소드. 월드 포지션 벡터 버전
    // clipName : 클립이름, spatialBlend(0~1): 2D~3D사운드정도, fixLength:사운드 길이지정(단위: 초) 기본값인 -1일시 wav,mp3파일의 고유한 길이로 재생
    public SFX PlayEffect(string clipName, Vector3 pos, float spatialBlend = 0.88f, float fixLength = -1)
    {
        int find = effectAudio.FindIndex(x => x.name == clipName);
        if (find == -1)
        {
            Debug.Log($"{clipName} 라는 이름의 효과음은 없습니다.");
            return null;
        }
        if (find == -1) return null;
        PoolBehaviour pb = PoolManager.Instance?.Spawn(sfxPrefab, pos, Quaternion.identity, transform);
        SFX _pb = pb as SFX;
        _pb.transform.position = pos;
        _pb.aus.loop = false;
        _pb.aus.spatialBlend = spatialBlend;
        _pb.Play(effectAudio[find], fixLength);
        return _pb;
    }
    // 외부에서 효과음을 재생시킬때 쓰는 메소드. 위 메소드랑 비슷한데. 타겟 트랜스폼 버전으로 오버로딩 한것 
    // isTracking 이 false일시 효과음소리가 타겟 트랜스폼을 안따라다니고 최초 스폰지점에서 정지, true일경우 재생하는동안 포지션 추적)
    public SFX PlayEffect(string clipName, Transform target, float spatialBlend = 0, bool isTracking = false, float fixLength = -1)
    {
        int find = effectAudio.FindIndex(x => x.name == clipName);
        if (find == -1)
        {
            Debug.Log($"{clipName} 라는 이름의 효과음은 없습니다.");
            return null;
        }
        PoolBehaviour pb = PoolManager.Instance.Spawn(sfxPrefab, target.position, Quaternion.identity, transform);
        SFX _pb = pb as SFX;
        _pb.transform.position = target.position;
        _pb.aus.loop = false;
        _pb.aus.spatialBlend = spatialBlend;
        if (isTracking)
        {
            _pb.Play(effectAudio[find], fixLength, target);
        }
        else
        {
            _pb.Play(effectAudio[find], fixLength);
        }
        return _pb;
    }
    
}

public enum SoundType {
    BGM, EFFECT,
}