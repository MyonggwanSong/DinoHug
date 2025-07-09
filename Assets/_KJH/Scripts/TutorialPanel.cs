using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] GameObject animal;
    [SerializeField] Transform interactableObjects;
    [SerializeField] GameObject[] pops;
    [SerializeField] PetStateController petStateController;
    public int progress = 0;
    public bool isComplete;
    void Awake()
    {
        petStateController.UpdateIsInteraction(true);
        animal.SetActive(false);
        for (int i = 0; i < interactableObjects.childCount; i++)
        {
            interactableObjects.GetChild(i).gameObject.SetActive(false);
        }
        isComplete = false;
        pops[0].SetActive(true);
    }
    public void NextButton()
    {
        if (progress >= pops.Length) return;
        pops[progress].SetActive(false);
        progress++;
        pops[progress].SetActive(true);
    }
    public void PrevButton()
    {
        if (progress == 0) return;
        pops[progress].SetActive(false);
        progress--;
        pops[progress].SetActive(true);
    }
    public void CompleteButton()
    {
        isComplete = true;
        pops[progress].SetActive(false);
        // 게임 진행
        petStateController.UpdateIsInteraction(false);
        animal.SetActive(true);
        for (int i = 0; i < interactableObjects.childCount; i++)
        {
            interactableObjects.GetChild(i).gameObject.SetActive(true);
        }
    }


    
}
