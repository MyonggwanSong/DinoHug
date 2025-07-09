using System.Collections;
using UnityEngine;
using DG.Tweening;
public class TutorialPanel : MonoBehaviour
{
    [SerializeField] AnimalControl animal;
    [SerializeField] Transform interactableObjects;
    [SerializeField] GameObject[] pops;
    public int progress = 0;
    public bool isComplete;
    SFX sfx;
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
        yield return new WaitForSeconds(0.5f); 
        pops[0].SetActive(true);
        yield return new WaitForSeconds(0.5f); 
        sfx = AudioManager.Instance.PlayEffect("Tuto1", transform.position, 0.65f);
    }
    public void NextButton()
    {
        if (progress >= pops.Length) return;
        sfx?.Stop();
        AudioManager.Instance.PlayEffect("UIClick1", transform.position, 0.65f);
        pops[progress].SetActive(false);
        progress++;
        pops[progress].SetActive(true);
        if (progress == 1)
        {
            sfx = AudioManager.Instance.PlayEffect($"Tuto{progress+1}", transform.position, 0.65f);
        }
    }
    public void PrevButton()
    {
        if (progress == 0) return;
        sfx?.Stop();
        AudioManager.Instance.PlayEffect("UIClick1", transform.position, 0.65f);
        pops[progress].SetActive(false);
        progress--;
        pops[progress].SetActive(true);
        if (progress == 0)
        {
            sfx = AudioManager.Instance.PlayEffect($"Tuto{progress+1}", transform.position, 0.65f);
        }
    }
    public void CompleteButton()
    {
        sfx?.Stop();
        AudioManager.Instance.PlayEffect("UIClick1", transform.position, 0.65f);
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


    
}
