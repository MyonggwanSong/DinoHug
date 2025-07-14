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
    
    void Awake()
    {
        TryGetComponent(out petStateController);
        FSM_Setting();
    }
    void Start()
    {
        // 게임 시작시 Idle로
        ChangeState(State.Eat);
        OnUpdateEffect?.Invoke(effect);
    }
    #region FSM (스크립트 하나만 키고 나머지는 끄는방식)
    public enum State
    {
        Idle,
        Wander,
        Eat,
        Play,
        Dead,
        Handle,
        Drink,
        CallFollow,
        CallIdle,
    }
    void FSM_Setting()
    {
        AnimalAbility[] animalAbilities = GetComponents<AnimalAbility>();
        for (int i = 0; i < animalAbilities.Length; i++)
        {
            dictionary.Add((State)i, animalAbilities[i]);
            animalAbilities[i].enabled = false;
        }
    }
    public void ChangeState(State newState)
    {
        // 이전 state 스크립트는 Disable 처리
        dictionary[state].UnInit();
        dictionary[state].enabled = false;
        // state 변경
        prevState = state;
        state = newState;
        // 변경할 state 스크립트 Enable 처리
        dictionary[state].enabled = true;
        dictionary[state].Init();
    }
    [HideInInspector] public State prevState;
    Dictionary<State, AnimalAbility> dictionary = new Dictionary<State, AnimalAbility>();
    #endregion
    #region Effect 관련
    // Effect는 여러개가 중복될 수 있으므로 BitMask로 구현
    [Flags]
    public enum Effect
    {
        None = 0,
        Hungry = 1 << 0,
        Thirsty = 1 << 1,
        Bored = 1 << 2,
        Lonely = 1 << 3,
    }
    // 외부에서 <<, | , & 같은 비트연산은 직접 다루기 불편하므로
    // HasEffect(), AddEffect(), RemoveEffect() 라는 메소드를 아래에 미리 만들어둡니다.
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
    #endregion

    #region PWH_ 
    public UnityAction<Effect> OnUpdateEffect;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (state.Equals(State.Idle) || state.Equals(State.Wander) || state.Equals(State.CallFollow))
            {
                Debug.Log("Player Calling!!");
                ChangeState(State.CallFollow);
            }
        }
    }
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
