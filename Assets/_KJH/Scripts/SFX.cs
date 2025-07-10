using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
[RequireComponent(typeof(AudioSource))]
public class SFX : PoolBehaviour
{
    #region UniTask Setting
    CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        Application.quitting += UniTaskCancel;
    }
    void OnDisable() { UniTaskCancel(); }
    void OnDestroy() { UniTaskCancel(); }
    void UniTaskCancel()
    {
        try
        {
            cts?.Cancel();
            cts?.Dispose();
        }
        catch (System.Exception e)
        {

            Debug.Log(e);
        }
        cts = null;
    }
    #endregion
    public AudioSource aus;
    void Awake()
    {
        TryGetComponent(out aus);
    }
    public void Play(AudioClip clip, float fixLength)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        AutoDespawn(clip, fixLength, cts.Token).Forget();
    }
    public void Play(AudioClip clip, float fixLength, Transform trackingTarget)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        AutoDespawn(clip, fixLength, cts.Token).Forget();
        TrackingTarget(trackingTarget, cts.Token).Forget();
    }
    async UniTask AutoDespawn(AudioClip clip, float fixLength, CancellationToken token)
    {
        if (fixLength == -1) fixLength = clip.length;
        await UniTask.Delay(1, ignoreTimeScale: true, cancellationToken: token);
        aus.loop = false;
        aus.clip = clip;
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
        if (!aus.enabled) aus.enabled = true;
        await UniTask.Delay(1, ignoreTimeScale: true, cancellationToken: token);
        aus.Play();
        await UniTask.Delay((int)(1000f * (fixLength + 0.15f)), ignoreTimeScale: true, cancellationToken: token);
        try
        {
            Despawn();
        }
        catch
        {
            gameObject.SetActive(false);
        }
    }
    async UniTask TrackingTarget(Transform trackingTarget, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.Delay(1, ignoreTimeScale: true, cancellationToken: token);
        }
    }
    public void Stop()
    {
        aus.Stop();
        UniTaskCancel();
        try
        {
            Despawn();
        }
        catch
        {
            gameObject.SetActive(false);
        }
    }


}
