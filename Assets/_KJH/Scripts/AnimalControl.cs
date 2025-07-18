using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using NaughtyAttributes;
public class AnimalControl : MonoBehaviour
{
    [ReadOnlyInspector] public State state;
    [ReadOnlyInspector] public Effect effect;
    [HideInInspector] public PetStateController petStateController;
    MultiAimConstraint headIK;
    void Awake()
    {
        GetComponentInChildren<Rig>().transform.Find("HeadLookAtConstraint").TryGetComponent(out headIK);
        transform.Find("Animator/Mesh/Face_A").TryGetComponent(out faceMR);
        TryGetComponent(out petStateController);
        FSM_Setting();
    }
    void Start()
    {
        // 게임 시작시 Idle로
        ChangeState(State.Idle);
        defaultFaceMat = faceMR.material;
        OnUpdateEffect?.Invoke(effect);
    }
    void OnEnable()
    {
        StopCoroutine(nameof(ChangeFaceLoop));
        StartCoroutine(nameof(ChangeFaceLoop));
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
        GameClear,
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
        if (state == State.Dead)
        {
            Debug.Log("죽었을때는 다른상태로 바꿀 수 없습니다.");
            return;
        }
        if (state == State.GameClear)
        {
            Debug.Log("게임 클리어이므로 다른상태로 바꿀 수 없습니다.");
            return;
        }
        // 이전 state 스크립트는 Disable 처리
        dictionary[state].UnInit();
        dictionary[state].enabled = false;
        Debug.Log($"{state} 종료");
        // state 변경
        prevState = state;
        state = newState;
        // 변경할 state 스크립트 Enable 처리
        dictionary[state].enabled = true;
        dictionary[state].Init();
        Debug.Log($"{state} 시작");
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
        AudioManager.Instance.PlayEffect("Alert", transform.position, 0.4f);
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
    public int EffectCount()
    {
        int count = 0;
        uint copy = (uint)effect;
        while (copy > 0)
        {
            copy &= (copy - 1);
            count++;
        }
        return count;
    }
    #endregion

    #region PWH_ 
    public UnityAction<Effect> OnUpdateEffect;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (state.Equals(State.Idle) || state.Equals(State.Wander) || state.Equals(State.CallIdle))
            {
                Debug.Log("Player Calling!!");
                ChangeState(State.CallFollow);
            }
        }
        if (petStateController.currentState.hunger >= 100)
        {
            if (state != State.Dead)
            {
                ChangeState(State.Dead);
                petStateController.UpdateIsInteraction(true);
            } 
        }
        if (petStateController.currentState.thirsty >= 100)
        {
            if (state != State.Dead)
            {
                ChangeState(State.Dead);
                petStateController.UpdateIsInteraction(true);
            }
        }
        if (petStateController.currentState.bond >= 100)
        {
            if (state != State.GameClear)
            {
                ChangeState(State.GameClear);
                petStateController.UpdateIsInteraction(true);
            }
        }
        
        // 바닥에 잘 안붙어서 아래로 붙여줌
        if (state == State.Wander)
            transform.position += 0.2f * Vector3.down * Time.deltaTime;
    }
    #endregion

    Tween tweenHeadIK;
    public void HeadIK_ON()
    {
        tweenHeadIK?.Kill();
        tweenHeadIK = DOTween.To(() => headIK.weight, x => headIK.weight = x, 0.8f, 1.5f);
    }
    public void HeadIK_OFF()
    {
        tweenHeadIK?.Kill();
        tweenHeadIK = DOTween.To(() => headIK.weight, x => headIK.weight = x, 0f, 1.5f);
    }
    SkinnedMeshRenderer faceMR;
    [SerializeField] Material[] faceMats;
    Material defaultFaceMat;
    public enum Face
    {
        Default,
        Hungry,
        Thirsty,
        Bored,
        Lonely,
        Angry,
        Happy,
        TwoEffect,
        ThreeEffect,
        FourEffect,
        Dead,
    }
    // 2초마다 항상 실행되고 있는 루프
    IEnumerator ChangeFaceLoop()
    {
        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(1.2f);
            // 만약 세부 동작들에서 일시적으로 얼굴 변경을 하고 있는경우 자동변경은 잠시 중단
            yield return new WaitUntil(() => !isChangeFaceTemporal);
            if (EffectCount() == 0)
            {
                ChangeFace(Face.Default);
            }
            else if (EffectCount() == 1)
            {
                if (effect == Effect.Hungry)
                    ChangeFace(Face.Hungry);
                else if (effect == Effect.Thirsty)
                    ChangeFace(Face.Thirsty);
                else if (effect == Effect.Bored)
                    ChangeFace(Face.Bored);
                else if (effect == Effect.Lonely)
                    ChangeFace(Face.Lonely);
            }
            else if (EffectCount() == 2)
            {
                ChangeFace(Face.TwoEffect);
            }
            else if (EffectCount() == 3)
            {
                ChangeFace(Face.ThreeEffect);
            }
            else if (EffectCount() == 4)
            {
                ChangeFace(Face.FourEffect);
            }
        }
    }
    bool isChangeFaceTemporal = false;
    public void ChangeFaceTemporal(Face face, float time)
    {
        if (coChangeFaceTemporal != null) StopCoroutine(coChangeFaceTemporal);
        coChangeFaceTemporal = StartCoroutine(ChangeFaceTemporal_co(face, time));
    }
    Coroutine coChangeFaceTemporal;
    IEnumerator ChangeFaceTemporal_co(Face face, float time)
    {
        ChangeFace(face);
        isChangeFaceTemporal = true;
        yield return YieldInstructionCache.WaitForSeconds(time);
        isChangeFaceTemporal = false;
    }
    void ChangeFace(Face face)
    {
        switch (face)
        {
            case Face.Default:
                faceMR.material = defaultFaceMat;
                break;
            case Face.Hungry:
                faceMR.material = faceMats[4];
                break;
            case Face.Thirsty:
                faceMR.material = faceMats[4];
                break;
            case Face.Bored:
                faceMR.material = faceMats[14];
                break;
            case Face.Lonely:
                faceMR.material = faceMats[8];
                break;
            case Face.Angry:
                faceMR.material = faceMats[10];
                break;
            case Face.Happy:
                faceMR.material = faceMats[2];
                break;
            case Face.TwoEffect:
                faceMR.material = faceMats[12];
                break;
            case Face.ThreeEffect:
                faceMR.material = faceMats[8];
                break;
            case Face.FourEffect:
                faceMR.material = faceMats[13];
                break;
            case Face.Dead:
                faceMR.material = faceMats[17];
                break;
        }
    }




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
    [SerializeField] Face testFace;
    [Button]
    public void Test_Face()
    {
        ChangeFace(testFace);
    }
#endif









}
