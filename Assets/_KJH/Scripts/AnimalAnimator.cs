using UnityEngine;
public class AnimalAnimator : MonoBehaviour
{
    public enum Type
    {
        Left,
        Right
    }

    Transform leftFoot;
    Transform rightFoot;
    void Awake()
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform tr in children)
        {
            if (tr.name == "LegL3")
            {
                leftFoot = tr;
            }
            if (tr.name == "LegR3")
            {
                rightFoot = tr;
            }
        }
    }
    public void FootEffect(Type type)
    {
        if (type == Type.Left)
        {
            AudioManager.Instance.PlayEffect("FootStep", leftFoot.position, 0.66f);
            ParticleManager.Instance.SpawnParticle(ParticleFlag.DustSmall, leftFoot.position, Quaternion.identity, null);
        }
        else
        {
            AudioManager.Instance.PlayEffect("FootStep", rightFoot.position, 0.66f);
            ParticleManager.Instance.SpawnParticle(ParticleFlag.DustSmall, rightFoot.position, Quaternion.identity, null);
        }
    }


}
