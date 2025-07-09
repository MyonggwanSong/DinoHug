using System;
using UnityEngine;
using UnityEngine.UI;

public class SoundView : MonoBehaviour
{
    [SerializeField] private GameObject parentView;

    [SerializeField] private float pre_BGM;
    [SerializeField] private float pre_Effect;

    [Header("Volume Slider")]
    [SerializeField] Slider slider_BGM;
    [SerializeField] Slider slider_Effect;

    void Start()
    {
        slider_BGM.value = AudioManager.Instance.init_Master;
        slider_Effect.value = AudioManager.Instance.init_Effect;

        pre_BGM = slider_BGM.value;
        pre_Effect = slider_Effect.value;
    }

    public void SetParentView(GameObject parentView)
    {
        this.parentView = parentView;
    }

    public void OnUpdateValueMaster()
    {
        if (AudioManager.Instance == null) return;

        AudioManager.Instance.SetVolume(SoundType.BGM, slider_BGM.value);
    }

    public void OnUpdateValueEffect()
    {
        if (AudioManager.Instance == null) return;

        AudioManager.Instance.SetVolume(SoundType.EFFECT, slider_Effect.value);
    }

    public void OnClickApplyButton()
    {
        pre_BGM = slider_BGM.value;
        pre_Effect = slider_Effect.value;

        UpdateVolume();

        parentView.GetComponent<EnvironmentView>().SetActiveView(false);
    }

    public void OnClickBack()
    {
        slider_BGM.value = pre_BGM;
        slider_Effect.value = pre_Effect;

        UpdateVolume();

        parentView.GetComponent<EnvironmentView>().SetActiveView(false);
    }

    private void UpdateVolume()
    {
        AudioManager.Instance.SetVolume(SoundType.BGM, pre_BGM);
        AudioManager.Instance.SetVolume(SoundType.EFFECT, pre_Effect);
    }

    void OnEnable()
    {
        Debug.Log("OnEnable");

        pre_BGM = slider_BGM.value;
        pre_Effect = slider_Effect.value;
    }

    void OnDisable()
    {
        
    }
}
