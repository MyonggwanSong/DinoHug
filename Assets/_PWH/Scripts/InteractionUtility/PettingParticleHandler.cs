using UnityEngine;

public class PettingParticleHandler : MonoBehaviour
{
    [SerializeField] AnimalPet ap;

    void Update()
    {
        if (ap.isPetting)
        {
            ShowParticle();
        }
    }

    void ShowParticle()
    {
        // if (animal.controller == null) return;

        Vector3 point = ap.controller.transform.position;
        ParticleManager.Instance.SpawnParticle(ParticleFlag.Petting, point, Quaternion.identity, this.gameObject.transform);
    }
}