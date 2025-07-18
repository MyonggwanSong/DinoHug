using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class House : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] GameObject door;
    [SerializeField] GameObject houseUI;
    [SerializeField] Vector3 targetRot;

    public void ResetHouse()
    {
        door.transform.rotation = Quaternion.Euler(Vector3.zero);
        houseUI.SetActive(true);
    }

    public void OpenDoorEvent()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => houseUI.SetActive(false))
            .AppendInterval(0.2f)
            .AppendCallback(()=>
            {
                SFX doorbell = AudioManager.Instance.PlayEffect("Doorbell", transform.position);
                door.transform.DOLocalRotate(targetRot, 1.0f);
            });
    }
}
