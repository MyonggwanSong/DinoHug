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
        AnimalAbility[] animalAbilities = GetComponents<AnimalAbility>();
        for (int i = 0; i < animalAbilities.Length; i++)
        {
            dictionary.Add((State)i, animalAbilities[i]);
            animalAbilities[i].enabled = false;
        }
    }
    void OnEnable()
    {
        EventManager.Instance.ChangeStateAction += OnChangeState;
    }
    void OnDisable()
    {
        EventManager.Instance.ChangeStateAction -= OnChangeState;
    }
    void Start()
    {
        // 게임 시작시 공룡은 Idle로
        ChangeState(State.Eat);
        OnUpdateEffect?.Invoke(effect);
    }

    #region FSM (스크립트 하나만 켜고 나머지는 끄는 방식을 사용)
    public enum State
    {
        Idle,
        Wander,
        Eat,
        Play,
        Dead,
        Handle,
        Drink,
        Call,
    }
    public void ChangeState(State newState)
    {
        EventManager.Instance.ChangeStateAction.Invoke(newState);
    }
    void OnChangeState(State newState)
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
    // Effect는 여러개가 중복될 수 있으므로 BitMask로 구현하였음
    [Flags]
    public enum Effect
    {
        None = 0,
        Hungry = 1 << 0,
        Thirsty = 1 << 1,
        Bored = 1 << 2,
        Lonely = 1 << 3,
    }
    // 외부에서 <<, | , & 같은 비트연산을 직접 다루기 어려울수 있으므로
    // HasEffect(), AddEffect(), RemoveEffect() 라는 메소드를 아래에 미리 만들어둡니다. 아래를 이용합니다.
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
