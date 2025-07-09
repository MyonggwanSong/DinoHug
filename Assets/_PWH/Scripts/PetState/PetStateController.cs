using System;
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

    [Header("Effect Apply")]
    public AdditiveState add_Play;
    [Space(10)]
    public AdditiveState add_Petting;
    [Space(10)]
    public AdditiveState add_Feed;
    [Space(10)]
    public AdditiveState add_GiveWater;

    #region 가중치 합산 Value
    private float totalBondVal;
    private float totalHungerVal;
    private float totalThirstyVal;
    private float totalBoredVal;
    #endregion

    private bool isInteraction;

    void Start()            // 추후에 OnEnable로 변경
    {
        nonInteractionElapsedTime = 0f;
        model = new(view, _init_Bond, _init_Hunger, _init_Thirsty, _init_Bored);

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
            model.UpdateBond(-1 * totalBondVal);
        }

        model.UpdateHunger(totalHungerVal);
        model.UpdateThirsty(totalThirstyVal);
        model.UpdateBored(totalBoredVal);
    }

    //Animal Control에 Observer를 통해 상호작용중 여부 확인하기.
    void UpdateIsInteraction(bool on)
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

    #region Interaction Part
    // 놀아주기
    public void Play()
    {
        add_Play.Apply(model);
        nonInteractionElapsedTime = 0f;
        model.UpdateView();
    }

    // 쓰다듬기
    public void Petting()
    {
        add_Petting.Apply(model);
        nonInteractionElapsedTime = 0f;
        model.UpdateView();
    }

    // 먹이주기
    public void Feed()
    {
        add_Feed.Apply(model);
        nonInteractionElapsedTime = 0f;
        model.UpdateView();
    }

    //물 주기
    public void Drink()
    {
        add_GiveWater.Apply(model);
        nonInteractionElapsedTime = 0f;
        model.UpdateView();
    }
    #endregion

    #region Getter
    public float GetBond() => model.bond;
    public float GetHungry() => model.hunger;
    public float GetThirsty() => model.thirsty;
    public float GetBored() => model.bored;
    #endregion
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

    public void UpdateBond(float amount)
    {
        if (view == null) return;
        bond += amount;
        bond = Mathf.Clamp(bond, 0, 100);
        UpdateView();
    }

    public void UpdateHunger(float amount)
    {
        if (view == null) return;
        hunger += amount;
        hunger = Mathf.Clamp(hunger, 0, 100);
        UpdateView();
    }

    public void UpdateThirsty(float amount)
    {
        if (view == null) return;
        thirsty += amount;
        thirsty = Mathf.Clamp(thirsty, 0, 100);
        UpdateView();
    }

    public void UpdateBored(float amount)
    {
        if (view == null) return;
        bored += amount;
        bored = Mathf.Clamp(bored, 0, 100);
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
public class AdditiveState
{
    public float bond;
    public float bored;
    public float thirsty;
    public float hunger;

    public void Apply(PetStateModel model)
    {
        model.bond += bond;
        model.bored += bored;
        model.thirsty += thirsty;
        model.hunger += hunger; 
    }
}