using UnityEngine;

public class ParticleHandler : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;

    void Start()
    {
        particle.gameObject.SetActive(false);
    }

    public void SetActiveParticle(bool on)
    {
        particle.gameObject.SetActive(on);
    }

    public void UpdateTransform(Vector3 position)
    {
        particle.gameObject.transform.position = position;
    }
}