using UnityEngine;
using DG.Tweening;
using UnityEditor;
public class GameOverPanel : MonoBehaviour
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
        sfx = AudioManager.Instance.PlayEffect("GameOverPop", transform.position, 1f);
    }
    public void CloseButton()
    {
        sfx?.Stop();
    }
    public void QuitButton()
    {
        sfx?.Stop();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }





}
