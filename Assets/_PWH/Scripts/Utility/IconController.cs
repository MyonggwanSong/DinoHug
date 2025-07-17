using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class IconController : MonoBehaviour
{
    [Header("Scale")]
    [SerializeField] Vector3 initScale;
    [SerializeField] Vector3 targetScale1;
    [SerializeField] Vector3 targetScale2;
    [SerializeField] Vector3 targetScale3;

    [Header("Duration")]
    [SerializeField] float duration1;
    [SerializeField] float duration2;
    
    void Start()
    {
        this.gameObject.SetActive(false);
    }
  
    void OnEnable()
    {
        ShowEffect();
    }

    [Button]
    void ShowEffect()
    {
        Sequence s = DOTween.Sequence("ShowEffect");

        s.AppendCallback((() => this.gameObject.SetActive(true)))
            .AppendCallback(() => this.transform.localScale = initScale)
            .Append(transform.DOScale(targetScale1, duration1))
            .Append(transform.DOScale(targetScale2, duration2))
            .AppendCallback(() => this.gameObject.SetActive(false));
    }
}