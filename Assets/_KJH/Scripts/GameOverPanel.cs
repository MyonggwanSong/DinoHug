using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.SceneManagement;
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
        sfx = AudioManager.Instance.PlayEffect("GameOverPop", transform.position);
    }
    public void ReButton()
    {
        sfx?.Stop();
        sfx = AudioManager.Instance.PlayEffect("UIClick1", transform.position);
        SceneManager.LoadScene(0);
    }
    public void QuitButton()
    {
        sfx?.Stop();
        sfx = AudioManager.Instance.PlayEffect("UIClick1", transform.position);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }





}
