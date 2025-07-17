using System.Collections;
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
        sfx?.Stop();
        sfx = AudioManager.Instance.PlayEffect("UIClick1", transform.position);
        pop.gameObject.SetActive(false);
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

    public IEnumerator ConfetiEffect()
    {
        Vector3 randomPos;
        for (int i = 0; i < 10; i++)
        {
            int count = Random.Range(1, 4);
            for (int j = 0; j < count; j++)
            {
                randomPos = transform.position + new Vector3(Random.Range(-5f, 5f), Random.Range(0f, 1f), Random.Range(-5f, 5f));
                AudioManager.Instance.PlayEffect("Confetti", randomPos);
                ParticleFlag flag = ParticleFlag.Confetti1;
                if (Random.value > 0.5f)
                    flag = ParticleFlag.Confetti1;
                else
                    flag = ParticleFlag.Confetti2;
                ParticleManager.Instance.SpawnParticle(flag, randomPos, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), null);
                yield return YieldInstructionCache.WaitForSeconds(Random.Range(0.01f, 0.1f));
            }            
            yield return YieldInstructionCache.WaitForSeconds(Random.Range(0.5f, 1.2f));
        }
    }


    
}
