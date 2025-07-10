using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class TutorialPanel : MonoBehaviour
{
    [SerializeField] AnimalControl animal;
    [SerializeField] Transform interactableObjects;
    [SerializeField] GameObject[] pops;
    [SerializeField] TMP_Text[] tmpTexts;
    public int progress = 0;
    public bool isComplete;
    SFX sfx;
    Tween tweenPop;
    void OnEnable()
    {
        animal.petStateController.UpdateIsInteraction(true);
        animal.gameObject.SetActive(false);
        for (int i = 0; i < interactableObjects.childCount; i++)
        {
            interactableObjects.GetChild(i).gameObject.SetActive(false);
        }
        isComplete = false;
    }
    IEnumerator Start()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        pops[0].SetActive(true);
        tmpTexts[0].gameObject.SetActive(false);
        tweenPop?.Kill();
        pops[0].transform.localScale = 0.85f * Vector3.one;
        tweenPop = pops[0].transform.DOScale(1f, 0.4f).SetEase(Ease.OutBounce);
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        sfx = AudioManager.Instance.PlayEffect("Tuto1", transform.position, 1.0f);
        if (coShowText != null)
        {
            StopCoroutine(coShowText);
        }
        coShowText = StartCoroutine(nameof(ShowText), tmpTexts[0]);
    }
    public void NextButton()
    {
        if (progress >= pops.Length) return;
        sfx?.Stop();
        AudioManager.Instance.PlayEffect("UIClick1", transform.position, 1.0f);
        pops[progress].SetActive(false);
        progress++;
        pops[progress].SetActive(true);
        tmpTexts[progress].gameObject.SetActive(true);
        tweenPop?.Kill();
        pops[progress].transform.localScale = 0.85f * Vector3.one;
        tweenPop = pops[progress].transform.DOScale(1f, 0.4f).SetEase(Ease.OutBounce);
        if (progress == 1)
        {
            sfx = AudioManager.Instance.PlayEffect($"Tuto{progress + 1}", transform.position, 1.0f);
        }
    }
    public void PrevButton()
    {
        if (progress == 0) return;
        sfx?.Stop();
        AudioManager.Instance.PlayEffect("UIClick1", transform.position, 1.0f);
        pops[progress].SetActive(false);
        progress--;
        pops[progress].SetActive(true);
        tweenPop?.Kill();
        pops[progress].transform.localScale = 0.85f * Vector3.one;
        tweenPop = pops[progress].transform.DOScale(1f, 0.4f).SetEase(Ease.OutBounce);
        if (progress == 0)
        {
            sfx = AudioManager.Instance.PlayEffect($"Tuto{progress + 1}", transform.position, 1.0f);
        }
    }
    public void CompleteButton()
    {
        sfx?.Stop();
        AudioManager.Instance.PlayEffect("UIClick1", transform.position, 1.0f);
        isComplete = true;
        pops[progress].SetActive(false);
        // 게임 진행
        animal.petStateController.UpdateIsInteraction(false);
        animal.gameObject.SetActive(true);
        for (int i = 0; i < interactableObjects.childCount; i++)
        {
            interactableObjects.GetChild(i).gameObject.SetActive(true);
        }
    }
    // 임시
    Coroutine coShowText;
    IEnumerator ShowText(TMP_Text tMP_Text)
    {
        tMP_Text.gameObject.SetActive(true);
        string original = tMP_Text.text;
        string sum = "";
        int length = original.Length;
        for (int i = 0; i < length; i++)
        {
            sum += original.Substring(i, 1);
            tMP_Text.text = sum;
            yield return YieldInstructionCache.WaitForSeconds(0.05f);
        }
    }


    
}
