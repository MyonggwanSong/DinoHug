using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
public class AnimalControl : MonoBehaviour
{
    [ReadOnlyInspector] public State state;
    [ReadOnlyInspector] public Effect effect;
    [HideInInspector] public PetStateController petStateController;
    public enum State
    {
        Idle,
        Wander,
        Eat,
        Play,
        Dead,
        Handle,
        Drink,
    }
    // Effect는 중복 될수있으므로 BitMask로 구현하였음. 외부에서 다른개발자가 1 << n 같은 시프트 연산을 직접하기 어려울수 있으므로
    // HasEffect(), AddEffect(), RemoveEffect() 라는 메소드를 만들어둘테니 직접 시프트 연산하지말고 이 메소드들을 이용하세요.
    [Flags]
    public enum Effect
    {
        None = 0,
        Hungry = 1 << 0,
        Thirsty = 1 << 1,
        Bored = 1 << 2,
        Lonely = 1 << 3,
    }
    public bool HasEffect(Effect checkEffect)
    {
        return (effect & checkEffect) != 0;
    }
    public void AddEffect(Effect addEffect)
    {
        if (HasEffect(addEffect))
        {
            // 이미 켜져있는 이펙트 입니다.
            return;
        }
        effect = effect | addEffect;
        
        OnUpdateEffect?.Invoke(effect);
    }
    public void RemoveEffect(Effect removeEffect)
    {
        if (!HasEffect(removeEffect))
        {
            // 이미 꺼져있는 이펙트인데 리무브 시도 할 경우 그냥 리턴처리
            return;
        }
        effect = effect & ~(removeEffect);

        OnUpdateEffect?.Invoke(effect);
    }
    [HideInInspector] public State prevState;
    Dictionary<State, AnimalAbility> dictionary = new Dictionary<State, AnimalAbility>();
    void Awake()
    {
        TryGetComponent(out petStateController);
        AnimalAbility[] animalAbilities = GetComponents<AnimalAbility>();
        for (int i = 0; i < animalAbilities.Length; i++)
        {
            dictionary.Add((State)i, animalAbilities[i]);
            animalAbilities[i].enabled = false;
        }
        
    }
    void Start()
    {
        // 게임 시작시 공룡은 Idle로
        prevState = State.Idle;
        ChangeState(State.Idle);
        OnUpdateEffect?.Invoke(effect);
    }
    // 외부에서 공룡의 상태를 변경하고 싶다면 아래 메소드를 이용한다.
    public void ChangeState(State newState)
    {     
        //Debug.Log($"변경전 newState : {newState}, prevState : {prevState}, state : {state}");
        // 이전 state 스크립트는 Disable 처리
        //// 선택사항) 상태변경은 Idle 이나 Wander에서만 가능
        //if (prevState != State.Idle && prevState != State.Wander) return;
        dictionary[prevState].UnInit();
        dictionary[prevState].enabled = false;
        dictionary[state].UnInit();
        dictionary[state].enabled = false;
        // 변경할 state 스크립트로 Enable 처리
        prevState = state;
        state = newState;
        dictionary[newState].enabled = true;
        dictionary[newState].Init();
        // Debug.Log($"변경후 newState : {newState}, prevState : {prevState}, state : {state}");
    }

    #region PWH_ 
    public UnityAction<Effect> OnUpdateEffect;
    #endregion


#if UNITY_EDITOR
    [Header("에디터에서 강제변경 테스트하려면 아래 버튼으로")]
    [SerializeField] State testState;
    [Button]
    public void Test()
    {
        ChangeState(testState);
    }
    
    [Space(10)]
    [SerializeField] Effect testEffect;
    [Button]
    public void Test_Effect()
    {
        effect = testEffect;
        OnUpdateEffect?.Invoke(effect);
    }
#endif

}
