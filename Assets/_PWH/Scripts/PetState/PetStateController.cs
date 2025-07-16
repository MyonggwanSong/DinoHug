using System;
using UnityEditorInternal;
using UnityEngine;

public class PetStateController : MonoBehaviour
{
    [Header("Final")]
    public AnimalControl animal;

    [Header("View")]
    [SerializeField] PetStateView view;
    private PetStateModel model;

    [Header("Init Amount [초기화 데이터]")]
    [SerializeField] float _init_Bond;
    [SerializeField] float _init_Hunger;
    [SerializeField] float _init_Thirsty;
    [SerializeField] float _init_Bored;

    [Header("Minus Amount [초당 변감 수치]")]
    [SerializeField] float _figure_Bond_Origin;
    [SerializeField] float _figure_Bond_Additive;
    [SerializeField] float _figure_Hunger_Origin;
    [SerializeField] float _figure_Hunger_Additive;
    [SerializeField] float _figure_Thirsty_Origin;
    [SerializeField] float _figure_Thirsty_Additive;
    [SerializeField] float _figure_Bored_Origin;
    [SerializeField] float _figure_Bored_Additive;

    [Header("Non Interaction")]
    public float nonInteractionTime;
    public float nonInteractionElapsedTime;

    [Header("Perform Apply")]
    public PetState add_Play;
    [Space(10)]
    public PetState add_Petting;
    [Space(10)]
    public PetState add_Feed;
    [Space(10)]
    public PetState add_GiveWater;

    [Header("Additive Effect Apply [특수 효과 적용]")]
    [SerializeField] float deadline_Bond;
    [SerializeField] float deadline_Hunger;
    [SerializeField] float deadline_Thirsty;
    [SerializeField] float deadline_Bored;

    #region 가중치 합산 Value
    private float totalBondVal;
    private float totalHungerVal;
    private float totalThirstyVal;
    private float totalBoredVal;
    #endregion

    public bool isInteraction { get; private set; }

    void Start()            // 추후에 OnEnable로 변경
    {
        nonInteractionElapsedTime = 0f;
        model = new(view, _init_Bond, _init_Hunger, _init_Thirsty, _init_Bored);
        currentState = new(_init_Bond, _init_Hunger, _init_Thirsty, _init_Bored);
        //Remove...
        totalBondVal = _figure_Bond_Origin;
        totalHungerVal = _figure_Hunger_Origin;
        totalThirstyVal = _figure_Thirsty_Origin;
        totalBoredVal = _figure_Bored_Origin;
    }

    void Update()
    {
        //상호작용 중에는 ui 갱신 수행 X
        if (isInteraction) return;

        // 대기상태나 배회 상태인 경우에만 수행.
        if (animal.state.Equals(AnimalControl.State.Idle) || animal.state.Equals(AnimalControl.State.Wander))
        {
            nonInteractionElapsedTime += Time.time;
        }

        //n초 이상 상호작용이 없으면 프레임마다 {_figure_Bond}씩 하락
        if (nonInteractionElapsedTime >= nonInteractionTime)
        {
            UpdateBond(-1 * totalBondVal);
        }

        UpdateHunger(totalHungerVal);
        UpdateThirsty(totalThirstyVal);
        UpdateBored(totalBoredVal);

        CheckEffects();
    }

    #region Update State

    public void UpdateBond(float amount)
    {
        if (view == null) return;
        currentState.bond += amount;
        currentState.bond = Mathf.Clamp(currentState.bond, 0, 100);
        model.UpdateBond(currentState.bond);
    }

    public void UpdateHunger(float amount)
    {
        if (view == null) return;

        if (animal.state.Equals(AnimalControl.State.Eat)) return;

        currentState.hunger += amount;
        currentState.hunger = Mathf.Clamp(currentState.hunger, 0, 100);
        model.UpdateHunger(currentState.hunger);
    }

    public void UpdateThirsty(float amount)
    {
        if (view == null) return;
        if (animal.state.Equals(AnimalControl.State.Drink)) return;
        currentState.thirsty += amount;
        currentState.thirsty = Mathf.Clamp(currentState.thirsty, 0, 100);
        model.UpdateThirsty(currentState.thirsty);
    }

    public void UpdateBored(float amount)
    {
        if (view == null) return;
        if (animal.state.Equals(AnimalControl.State.Handle)) return;
        currentState.bored += amount;
        currentState.bored = Mathf.Clamp(currentState.bored, 0, 100);
        model.UpdateBored(currentState.bored);
    }
    #endregion

    //Animal Control에 Observer를 통해 상호작용중 여부 확인하기.
    public void UpdateIsInteraction(bool on)
    {
        isInteraction = on;
    }

    public void SetAnimal(AnimalControl animal)
    {
        this.animal = animal;
    }

    // 상태 변경시 실행되는 메서드 => 옵저버를 통해 animalControl에 구독해둔다.
    public void OnChangedAdditiveEffect()
    {
        // 가중치 데이터 갱신하기.     
        totalHungerVal = animal.HasEffect(AnimalControl.Effect.Hungry) ? _figure_Hunger_Origin + _figure_Hunger_Additive : _figure_Hunger_Origin;
        totalThirstyVal = animal.HasEffect(AnimalControl.Effect.Thirsty) ? _figure_Thirsty_Origin + _figure_Thirsty_Additive : _figure_Thirsty_Origin;
        totalBoredVal = animal.HasEffect(AnimalControl.Effect.Bored) ? _figure_Bored_Origin + _figure_Bored_Additive : _figure_Bored_Origin;

        float res = _figure_Bond_Origin;
        if (animal.HasEffect(AnimalControl.Effect.Bored)) res += _figure_Bond_Additive;
        if (animal.HasEffect(AnimalControl.Effect.Lonely)) res += _figure_Bond_Additive;         //비교군을 Lonely로 변경하기.
    }

    #region Current Pet State
    public PetState currentState { get; private set; }
    #endregion

    #region Interaction Part
    // 놀아주기
    public void Play()
    {
        currentState += add_Play;
        nonInteractionElapsedTime = 0f;
        model.UpdateView();
    }

    // 쓰다듬기
    public void Petting()
    {
        currentState += add_Petting;
        nonInteractionElapsedTime = 0f;
        model.UpdateView();
    }

    // 먹이주기
    public void Feed()
    {
        currentState += add_Feed;
        nonInteractionElapsedTime = 0f;
        model.UpdateView();
    }

    //물 주기
    public void Drink()
    {
        currentState += add_GiveWater;
        nonInteractionElapsedTime = 0f;
        model.UpdateView();
    }
    #endregion

    public void CheckEffects()
    {
        if (currentState.bond <= deadline_Bond) animal.AddEffect(AnimalControl.Effect.Lonely);
        else animal.RemoveEffect(AnimalControl.Effect.Lonely);

        if (currentState.hunger >= deadline_Hunger) animal.AddEffect(AnimalControl.Effect.Hungry);
        else animal.RemoveEffect(AnimalControl.Effect.Hungry);

        if (currentState.thirsty >= deadline_Thirsty) animal.AddEffect(AnimalControl.Effect.Thirsty);
        else animal.RemoveEffect(AnimalControl.Effect.Thirsty);
        
        if (currentState.bored >= deadline_Bored) animal.AddEffect(AnimalControl.Effect.Bored);
        else animal.RemoveEffect(AnimalControl.Effect.Bored);
    }
}

public class PetStateModel
{
    PetStateView view;

    public float bond;
    public float hunger;
    public float thirsty;
    public float bored;

    public PetStateModel(PetStateView view, float _init_Bond, float _init_Hunger, float _init_Thirsty, float _init_Bored)
    {
        this.view = view;
        this.bond = _init_Bond;
        this.hunger = _init_Hunger;
        this.thirsty = _init_Thirsty;
        this.bored = _init_Bored;

        //UI에 적용
        UpdateView();
    }

    public void UpdateBond(float bond)
    {
        if (view == null) return;
        this.bond = bond;
        UpdateView();
    }

    public void UpdateHunger(float hunger)
    {
        if (view == null) return;
        this.hunger = hunger;
        UpdateView();
    }

    public void UpdateThirsty(float thirsty)
    {
        if (view == null) return;
        this.thirsty = thirsty;
        UpdateView();
    }

    public void UpdateBored(float bored)
    {
        if (view == null) return;
        this.bored = bored;
        UpdateView();
    }

    public void UpdateView()
    {
        view.UpdateBond(this.bond);
        view.UpdateHunger(this.hunger);
        view.UpdateThirsty(this.thirsty);
        view.UpdateBored(this.bored);
    }
}

[Serializable]
public class PetState
{
    public float bond;
    public float hunger;
    public float thirsty;
    public float bored;

    public PetState(float bond, float hunger, float thirsty, float bored)
    {
        this.bond = bond;
        this.hunger = hunger;
        this.thirsty = thirsty;
        this.bored = bored;
    }

    public static PetState operator+ (PetState p1, PetState p2)
    {
        return new PetState(p1.bond + p2.bond, p1.hunger + p2.hunger, p1.thirsty + p2.thirsty, p1.bored + p2.bored);
    }
}