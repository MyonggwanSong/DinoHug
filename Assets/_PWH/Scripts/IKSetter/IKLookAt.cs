using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKLookAt : MonoBehaviour
{
    [SerializeField] Transform target;          //Player
    [SerializeField] MultiAimConstraint constraint;
    [SerializeField] float maxDistance = 5f;
    [SerializeField] private float maxAngle = 45f;
    [SerializeField] private float transitionTime = 0.3f;
    AnimalControl animal;

    void Awake()
    {
        target = Camera.main.transform;
        this.constraint = GetComponentInChildren<MultiAimConstraint>();
        animal = GetComponentInParent<AnimalControl>();

        if (constraint == null || target == null) return;

        InitConstraint(target);
    }

    private float currentWeight = 0f;
    private Tweener weightTweener;

    void Update()
    {
        if (animal.state.Equals(AnimalControl.State.Eat)) return;
        if (animal.state.Equals(AnimalControl.State.Dead)) return;
        if (animal.state.Equals(AnimalControl.State.Drink)) return;
        if (CheckDistance()) return;
        
        Vector3 toTarget = target.position - transform.position;
        float angle = Vector3.Angle(transform.forward, toTarget.normalized);
        bool shouldLook = angle < maxAngle;
        float targetWeight = shouldLook ? 1f : 0f;

        if (!Mathf.Approximately(currentWeight, targetWeight))
        {
            if (weightTweener != null && weightTweener.IsActive())
                weightTweener.Kill();

            weightTweener = DOTween.To(() => currentWeight, x =>
            {
                currentWeight = x;

                var data = constraint.data;
                constraint.weight = currentWeight;
            }, targetWeight, transitionTime);
        }
    }

    bool CheckDistance()
    {
        float d = Vector3.Distance(this.transform.position, target.position);

        return d >= maxDistance;
    }

    void InitConstraint(Transform target)
    {
        WeightedTransformArray arr = new WeightedTransformArray(1);
        arr.SetWeight(0, 1f);
        arr.SetTransform(0, target);
        
        var data = constraint.data;
        data.sourceObjects = arr;
        constraint.data = data;
    }
}