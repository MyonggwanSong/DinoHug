using DG.Tweening;
using UnityEngine;

public class SelectionControl : MonoBehaviour
{
    Animator animator;

    [SerializeField] float animTime = 5f;

    float elapsed = 0f;

    void Start()
    {
        TryGetComponent(out animator);
    }

    void OnEnable()
    {
        if (animator == null)
        {
            TryGetComponent(out animator);
        }
        animator.SetTrigger("Happy");
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        if (animTime < elapsed)
        {
            elapsed = 0f;
            animator.SetTrigger("Happy");
        }
    }

    // public void MoveToTarget()
    // {
    //     Sequence seq = DOTween.Sequence();


    // }
}