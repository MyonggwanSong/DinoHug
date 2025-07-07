using UnityEngine;

public class Tester_Interaction : MonoBehaviour
{
    [SerializeField] InteractionEventHandler interactionEventHandler;

    [Header("Paticle Prefab")]
    [SerializeField] PoolableParticle particle;

    bool isHover;

    void Awake()
    {
        TryGetComponent(out interactionEventHandler);
        isHover = false;
    }

    void OnEnable()
    {
        interactionEventHandler.OnHoverCheck += InteractorHoverHandler;
        interactionEventHandler.OnHoverPointUpdate += UpdateParticlePosition;
    }

    void OnDisable()
    {
        interactionEventHandler.OnHoverCheck -= InteractorHoverHandler;
        interactionEventHandler.OnHoverPointUpdate -= UpdateParticlePosition;
    }

    void InteractorHoverHandler(bool on)
    {
        isHover = on;
    }

    void UpdateParticlePosition(Vector3 hitPoint)
    {
        Debug.Log($"Interactor Point : {interactionEventHandler.interactor.gameObject.transform.position}");
        Vector3 targetDir = interactionEventHandler.interactor.gameObject.transform.position - hitPoint;
        Quaternion quat = Quaternion.LookRotation(targetDir);

        Debug.Log($"Quaternion : {quat}");
        PoolManager.Instance.Spawn(particle, hitPoint, quat, this.gameObject.transform);
    }
}