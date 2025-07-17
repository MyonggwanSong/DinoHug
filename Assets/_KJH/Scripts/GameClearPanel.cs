using UnityEngine;
using DG.Tweening;
using UnityEditor;
public class GameClearPanel : MonoBehaviour
{
    Transform pop;
    SFX sfx;
    void Awake()
    {
        pop = transform.GetChild(0);
    }
    public void Activate()
    {
        pop.gameObject.SetActive(true);
        pop.localScale = 0.85f * Vector3.one;
        pop.DOScale(1f, 0.4f).SetEase(Ease.OutBounce);
        sfx = AudioManager.Instance.PlayEffect("GameClearPop", transform.position);
    }
    public void CloseButton()
    {
        sfx = AudioManager.Instance.PlayEffect("UIClick1", transform.position);
        sfx?.Stop();
        pop.gameObject.SetActive(false);
    }
    public void QuitButton()
    {
        sfx = AudioManager.Instance.PlayEffect("UIClick1", transform.position);
        sfx?.Stop();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }


    
}
