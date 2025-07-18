using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using DG.Tweening;

public class PetEffectViewer : MonoBehaviour
{
    [Header("Animal Reference")]
    [SerializeField] AnimalControl animal;

    [Header("Animal Imoticon")]
    [SerializeField] Sprite img_Lonely;
    [SerializeField] Sprite img_Hunger;
    [SerializeField] Sprite img_Thirsty;
    [SerializeField] Sprite img_Bored;


    [Header("EmoticonPanel")]
    [SerializeField] GameObject panel;
    [SerializeField] Image emoticon;
    [SerializeField] List<AnimalControl.Effect> effects;
    Dictionary<AnimalControl.Effect, Sprite> emoticonDic;

    void Start()
    {
        if (animal == null)
        {
            animal = GetComponentInParent<AnimalControl>();
        }
        animal.OnUpdateEffect += UpdateImoticon;

        InitEmoticonDic();
    }

    void InitEmoticonDic() {
        emoticonDic = new();

        emoticonDic[AnimalControl.Effect.Lonely] = img_Lonely;
        emoticonDic[AnimalControl.Effect.Hungry] = img_Hunger;
        emoticonDic[AnimalControl.Effect.Thirsty] = img_Thirsty;
        emoticonDic[AnimalControl.Effect.Bored] = img_Bored;
    }

    void Update()
    {

    }

    void OnEnable()
    {   
        if (animal == null)
        {
            animal = GetComponentInParent<AnimalControl>();
        }
        animal.OnUpdateEffect += UpdateImoticon;
    }

    void OnDisable()
    {
        animal.OnUpdateEffect -= UpdateImoticon;
    }

    public void UpdateImoticon(AnimalControl.Effect effect)
    {
        if (effect.Equals(AnimalControl.Effect.None))
        {
            panel.SetActive(false);
            return;
        }

        effects = GetActiveEffects(effect);
        ShowEmoticons();       
    }

    private void ShowEmoticons()
    {
        //만약 꺼져있다면 켜기
        if (!panel.activeSelf)
        {
            panel.SetActive(true);
            AudioManager.Instance.PlayEffect("Alert", transform.position, 0.4f);
        }

        if (effects.Count <= 1)
        {
            DOTween.Kill("EmoticonShowLoop");
            emoticon.sprite = emoticonDic[effects[0]];
        }
        else
        {
            LoopShowPerform(effects);
        }
    }

    void LoopShowPerform(List<AnimalControl.Effect> effects)
    {
        if (effects.Count <= 0) return;

        int count = effects.Count;

        int i = 0;

        DOTween.Kill("EmoticonShowLoop");

        Sequence seq = DOTween.Sequence().SetId("EmoticonShowLoop");

        seq.AppendCallback(() =>
        {
            //Debug.Log($"Effect : {effects[i]}");
            Sprite sp = emoticonDic[effects[i]];

            emoticon.sprite = sp;
            i = (i + 1) % count;
        });

        seq.AppendInterval(1f);
        seq.SetLoops(-1);
    }

    // Utility로 빼기
    public List<AnimalControl.Effect> GetActiveEffects(AnimalControl.Effect effect)
    {
        List<AnimalControl.Effect> res = new();

        foreach (AnimalControl.Effect e in Enum.GetValues(typeof(AnimalControl.Effect)))
        {
            if (e.Equals(AnimalControl.Effect.None)) continue;

            if ((effect & e) == e)
            {
                res.Add(e);
            }
        }

        return res;
    }
}