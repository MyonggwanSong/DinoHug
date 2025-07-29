using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;
public class TutorialPanel : MonoBehaviour
{
    AnimalControl animal;
    [SerializeField] Transform interactableObjects;
    [SerializeField] GameObject[] pops;
    Text[] texts;
    string[] originalTexts;
    public int progress = 0;
    public bool isComplete;
    SFX sfx;
    Tween tweenPop;
    public VideoClip[] videoClips;
    public RenderTexture[] renderTextures;
    VideoPlayer videoPlayer;
    void Awake()
    {
        TryGetComponent(out videoPlayer);
        animal = FindAnyObjectByType<AnimalControl>();
        texts = new Text[pops.Length];
        originalTexts = new string[pops.Length];
        for (int i = 0; i < pops.Length; i++)
        {
            texts[i] = pops[i].transform.Find("Content").GetComponent<Text>();
            originalTexts[i] = texts[i].text;
        }
    }
    IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);

        if (!GameManager.Instance.isFirst)
        {
            yield break;
        }

        texts[0].gameObject.SetActive(false);
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        animal.petStateController.UpdateIsInteraction(true);
        animal.gameObject.SetActive(false);
        for (int i = 0; i < interactableObjects.childCount; i++)
        {
            interactableObjects.GetChild(i).gameObject.SetActive(false);
        }
        isComplete = false;
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        pops[0].SetActive(true);
        tweenPop?.Kill();
        pops[0].transform.localScale = 0.85f * Vector3.one;
        tweenPop = pops[0].transform.DOScale(1f, 0.4f).SetEase(Ease.OutBounce);
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        sfx = AudioManager.Instance.PlayEffect("TutorialPop1", transform.position);
        texts[0].gameObject.SetActive(true);
        if (coShowText != null)
        {
            StopCoroutine(coShowText);
            coShowText = null;
        }
        coShowText = StartCoroutine(ShowText(0));
    }
    public void NextButton()
    {
        if (progress >= pops.Length) return;
        sfx?.Stop();
        AudioManager.Instance.PlayEffect("UIClick1", transform.position);
        pops[progress].SetActive(false);
        progress++;
        pops[progress].SetActive(true);
        tweenPop?.Kill();
        pops[progress].transform.localScale = 0.85f * Vector3.one;
        tweenPop = pops[progress].transform.DOScale(1f, 0.4f).SetEase(Ease.OutBounce);
        sfx = AudioManager.Instance.PlayEffect($"TutorialPop{progress + 1}", transform.position);
        if (coShowText != null)
        {
            StopCoroutine(coShowText);
            coShowText = null;
        }
        coShowText = StartCoroutine(ShowText(progress));
        videoPlayer.Stop();
        switch (progress)
        {
            case 2:
                videoPlayer.clip = videoClips[0];
                videoPlayer.targetTexture = renderTextures[0];
                videoPlayer.Play();
                break;
            case 3:
                videoPlayer.clip = videoClips[1];
                videoPlayer.targetTexture = renderTextures[1];
                videoPlayer.Play();
                break;
            case 4:
                videoPlayer.clip = videoClips[2];
                videoPlayer.targetTexture = renderTextures[2];
                videoPlayer.Play();
                break;
        }
    }
    public void PrevButton()
    {
        if (progress == 0) return;
        sfx?.Stop();
        AudioManager.Instance.PlayEffect("UIClick1", transform.position);
        pops[progress].SetActive(false);
        progress--;
        pops[progress].SetActive(true);
        tweenPop?.Kill();
        pops[progress].transform.localScale = 0.85f * Vector3.one;
        tweenPop = pops[progress].transform.DOScale(1f, 0.4f).SetEase(Ease.OutBounce);
        sfx = AudioManager.Instance.PlayEffect($"TutorialPop{progress + 1}", transform.position);
        if (coShowText != null)
        {
            StopCoroutine(coShowText);
            coShowText = null;
        }
        coShowText = StartCoroutine(ShowText(progress));
        videoPlayer.Stop();
        switch (progress)
        {
            case 2:
                videoPlayer.clip = videoClips[0];
                videoPlayer.targetTexture = renderTextures[0];
                videoPlayer.Play();
                break;
            case 3:
                videoPlayer.clip = videoClips[1];
                videoPlayer.targetTexture = renderTextures[1];
                videoPlayer.Play();
                break;
            case 4:
                videoPlayer.clip = videoClips[2];
                videoPlayer.targetTexture = renderTextures[2];
                videoPlayer.Play();
                break;
        }
    }
    public void CompleteButton()
    {
        sfx?.Stop();
        AudioManager.Instance.PlayEffect("UIClick1", transform.position);
        isComplete = true;
        pops[progress].SetActive(false);
        // 게임 진행
        animal.petStateController.UpdateIsInteraction(false);
        animal.gameObject.SetActive(true);
        for (int i = 0; i < interactableObjects.childCount; i++)
        {
            interactableObjects.GetChild(i).gameObject.SetActive(true);
        }
        animal.ChangeState(AnimalControl.State.Idle);
        if (coShowText != null)
        {
            StopCoroutine(coShowText);
            coShowText = null;
        }
        videoPlayer.Stop();

        GameManager.Instance.isFirst = false;
    }
    // 임시
    Coroutine coShowText;
    IEnumerator ShowText(int index)
    {
        string original = originalTexts[index];
        string sum = "";
        int length = original.Length;
        for (int i = 0; i < length; i++)
        {
            sum += original.Substring(i, 1);
            texts[index].text = sum;
            yield return YieldInstructionCache.WaitForSeconds(0.03f);
        }
    }
    



}
