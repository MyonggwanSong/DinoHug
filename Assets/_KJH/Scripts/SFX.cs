using System.Collections;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class SFX : PoolBehaviour
{
    AudioSource audioSource;
    public AudioClip clip;
    void Awake()
    {
        TryGetComponent(out audioSource);
    }
    void OnDisable()
    {
        Stop();
    }
    public void Stop()
    {  
        try
        {
            audioSource.Stop();
            StopCoroutine(nameof(AutoDespawn));
            StopCoroutine(nameof(TrackingTarget));
            Despawn();
        }
        catch
        {

        }
    }
    public void Play(Vector3 pos, float spatialBlend, float fixLength = -1)
    {
        audioSource.Stop();
        transform.position = pos;
        audioSource.spatialBlend = spatialBlend;
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();
        StartCoroutine(nameof(AutoDespawn), fixLength);
    }
    public void Play(Transform target, float spatialBlend, bool isTracking = true, float fixLength = -1)
    {
        audioSource.Stop();
        transform.position = target.transform.position;
        audioSource.spatialBlend = spatialBlend;
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();
        StartCoroutine(nameof(AutoDespawn), fixLength);
        if (isTracking)
        {
            StartCoroutine(nameof(TrackingTarget));
        }
    }
    IEnumerator AutoDespawn(float fixLength)
    {
        float length = clip.length + 0.1f;
        if (fixLength != -1)
            length = fixLength;
        yield return new WaitForSeconds(length);
        try
        {
            Despawn();
        }
        catch
        {

        }
    }
    IEnumerator TrackingTarget(Transform target)
    {
        float startTime = Time.time;
        while (Time.time - startTime < clip.length + 0.1f)
        {
            transform.position = target.position;
            yield return null;
        }
    }



}
