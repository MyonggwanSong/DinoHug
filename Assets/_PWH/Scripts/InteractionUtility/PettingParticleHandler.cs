using UnityEngine;

public class PettingParticleHandler : MonoBehaviour
{
    [SerializeField] AnimalHandle ah;

    void Update()
    {
        if (ah.isPlaying)
        {
            ShowParticle();
        }
    }

    void ShowParticle()
    {
        // if (animal.controller == null) return;
        Vector3 point = ah.controllerTr.position;
        ParticleManager.Instance.SpawnParticle(ParticleFlag.Petting, point, Quaternion.identity, this.gameObject.transform);
    }
}