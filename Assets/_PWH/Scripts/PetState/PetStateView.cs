using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PetStateView : MonoBehaviour
{
    [Header("View Component")]
    [Header("Slider")]
    [SerializeField] Slider bar_Bond;
    [SerializeField] Slider bar_Hunger;
    [SerializeField] Slider bar_Thirsty;
    [SerializeField] Slider bar_Bored;

    [Header("TMP")]
    [SerializeField] TextMeshProUGUI percent_Bond;
    [SerializeField] TextMeshProUGUI percent_Hunger;
    [SerializeField] TextMeshProUGUI percent_Thirsty;
    [SerializeField] TextMeshProUGUI percent_Bored;

    public void UpdateBond(float amount)
    {
        bar_Bond.value = amount / 100;
        percent_Bond.text = (bar_Bond.value * 100f).ToString("F0") + "%";
    }

    public void UpdateBored(float amount)
    {
        bar_Bored.value = amount / 100;
        percent_Bored.text = (bar_Bored.value * 100f).ToString("F0") + "%";
    }
    
    public void UpdateHunger(float amount)
    {
        bar_Hunger.value = amount / 100;
        percent_Hunger.text = (bar_Hunger.value * 100f).ToString("F0") + "%";
    }

    public void UpdateThirsty(float amount)
    {
        bar_Thirsty.value = amount / 100;
        percent_Thirsty.text = (bar_Thirsty.value * 100f).ToString("F0") + "%";
    }  
    
}