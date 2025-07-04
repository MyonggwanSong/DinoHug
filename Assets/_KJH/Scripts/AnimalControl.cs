using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimalControl : MonoBehaviour
{
    public State state;
    public Effect effect;
    [SerializeField] float eatRange = 3f;
    [SerializeField] float viewRange = 15f;

    Dictionary<State, AnimalAbility> dictionary = new Dictionary<State, AnimalAbility>();
    AnimalStatus status;
    State prevState = State.Dead;
    Collider[] colliders = new Collider[20];
    List<IInteractable> nearInteractableList = new List<IInteractable>();
    [SerializeField] List<Transform> view = new List<Transform>();
    [HideInInspector] public IInteractable target;
    void Awake()
    {
        TryGetComponent(out status);
    }
    void Start()
    {
        // 최초 실행시 AnimalAbility 스크립트들을 다 꺼지게 합니다.
        AnimalAbility[] abilities = GetComponents<AnimalAbility>();
        for (int i = 0; i < abilities.Length; i++)
        {
            abilities[i].enabled = false;
            dictionary.Add((State)i, abilities[i]);
        }
        StartCoroutine(nameof(StateMachine));
        StartCoroutine(nameof(Sensor));
        // 처음에는 Idle 상태로 시작합니다.
        state = State.Idle;
    }
    public enum State
    {
        Idle,
        Dead,
        Wander,
        GoTo,
        Eat,
        Drink,
        Handle,
        Play,
    }
    // 아래는 StateMachine패턴입니다. state가 바뀌면 해당하는 스크립트를 활성화시키고. 나머지는 비활성화 시킵니다.
    IEnumerator StateMachine()
    {
        while (true)
        {
            yield return null;
            yield return new WaitUntil(() => state != prevState);
            dictionary[prevState].UnInit();
            dictionary[prevState].enabled = false;
            dictionary[state].enabled = true;
            dictionary[state].Init();
            prevState = state;
        }
    }
    // 아래는 Sensor입니다. 0.1초마다 가까운 Interactable 태그 오브젝트들(먹이,물,공 등등)을 감지합니다.
    IEnumerator Sensor()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            nearInteractableList.Clear();
            view.Clear();
            Vector3 tPos = transform.position;
            int count = Physics.OverlapSphereNonAlloc(tPos + viewRange * 0.2f * transform.forward, viewRange, colliders);
            for (int i = 0; i < count; i++)
            {
                Transform root = colliders[i].transform.root;
                if (root == transform) continue;
                IInteractable interactable = root.GetComponentInChildren<IInteractable>();
                if (interactable == null) continue;
                if (!interactable.isPlace) continue;
                nearInteractableList.Add(interactable);
                view.Add(interactable.root);
            }
        }
    }
    // 아래는 Animal이 다음에 무슨행동을 선택할지 결정하는 로직입니다.
    public void NextState()
    {

        State nextState = State.Dead;
        
        if (HasEffect(Effect.Hungry))
            FindTarget(Effect.Hungry, out nextState);

        else if (HasEffect(Effect.Thirsty))
            FindTarget(Effect.Thirsty, out nextState);

        else if (HasEffect(Effect.Bored))
            FindTarget(Effect.Bored, out nextState);
            

        if(Random.value * 200f < status.hungry)
            FindTarget(Effect.Hungry, out nextState);
        if(Random.value * 200f < status.thirsty)
            FindTarget(Effect.Thirsty, out nextState);
        if(Random.value * 200f < status.bored)
            FindTarget(Effect.Bored, out nextState);


        // 평소에는 25% 확률로 Idle, 75% 확률로 Wander를 하려고 합니다.
        if (nextState == State.Dead)
            nextState = (Random.value <= 0.25f) ? State.Idle : State.Wander;
        
        // 기존 state 를 종료하고 nextState를 실행시키는 코드
        dictionary[state].UnInit();
        dictionary[state].enabled = false;
        dictionary[nextState].enabled = true;
        dictionary[nextState].Init();
        state = nextState;
        return;
    }
    bool FindTarget(Effect e, out State result)
    {
        State nextState;
        State s = State.Dead;
        IInteractable.Type t = IInteractable.Type.Food;
        switch (e)
        {
            case Effect.Hungry:
                s = State.Eat;
                t = IInteractable.Type.Food;
                break;
            case Effect.Thirsty:
                s = State.Drink;
                t = IInteractable.Type.Water;
                break;
            case Effect.Bored:
                s = State.Play;
                t = IInteractable.Type.Toy;
                break;
        }
        // 주변에 오브젝트가 있는지 확인
        int index = nearInteractableList.FindIndex(x => x.type == t);
        if (index != -1)
        {
            target = nearInteractableList[index];
            float distance = Vector3.Distance(nearInteractableList[index].root.position, transform.position);
            // eatRange보다 가까우면 바로 먹는 상태로 변경 or eatRange보다 멀리 있다면 GoTo 상태로
            if (distance <= eatRange)
            {
                nextState = s;
            }
            else
            {
                nextState = State.GoTo;
            }
            result = nextState;
            return true;
        }
        result = State.Dead;
        return false;
    }
    public enum Effect
    {
        None = 0,
        Hungry = 1 << 0,
        Thirsty = 1 << 1,
        Bored = 1 << 2,
    }
    public void AddEffect(Effect _effect)
    {
        if ((effect & _effect) != 0) return;
        effect = effect | _effect;
    }
    public void RemoveEffect(Effect _effect)
    {
        if ((effect & _effect) == 0) return;
        effect = effect & ~(_effect);
    }
    public bool HasEffect(Effect _effect)
    {
        return (effect & _effect) != 0;
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + viewRange * 0.2f * transform.forward, viewRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, eatRange);
    }
#endif





}
