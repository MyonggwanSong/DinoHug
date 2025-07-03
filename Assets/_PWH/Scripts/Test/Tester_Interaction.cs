using UnityEngine;

public class Tester_Interaction : MonoBehaviour
{
    [SerializeField] InteractionEventHandler interactionEventHandler;
    [SerializeField] ParticleHandler particleHandler;

    bool isHover;

    void Awake()
    {
        TryGetComponent(out interactionEventHandler);
        TryGetComponent(out particleHandler);

        isHover = false;
        particleHandler.SetActiveParticle(false);
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
        particleHandler.SetActiveParticle(on);
    }

    void UpdateParticlePosition(Vector3 hitPoint)
    {
        particleHandler.UpdateTransform(hitPoint);
    }
}