using UnityEngine;
public class AnimalStatus : MonoBehaviour
{
    public float bond = 40f;
    public float hungry = 20f;
    public float thirsty = 20f;
    public float bored = 20f;
    AnimalControl animal;
    void Awake()
    {
        TryGetComponent(out animal);
    }
    void Update()
    {
        //--------------------------
        // 시간마다 배고픔 계속 증가
        if (hungry > 0) hungry += 0.1f * Time.deltaTime;
        // 배고픔이 0이하 일시 사망 처리
        else if (hungry <= 0) animal.state = AnimalControl.State.Dead;

        // 시간마다 목마름 계속 증가
        if (thirsty > 0) thirsty += 0.1f * Time.deltaTime;
        // 목마름이 0이하 일시 사망 처리
        else if (thirsty <= 0) animal.state = AnimalControl.State.Dead;

        // 시간마다 지루함 계속 증가
        if (bored > 0) bored += 0.1f * Time.deltaTime;


        //--------------------------
        // 배고픔이 60이상일시 Hungry 추가
        if (hungry > 60) animal.AddEffect(AnimalControl.Effect.Hungry);
        else animal.RemoveEffect(AnimalControl.Effect.Hungry);

        // 목마름이 60이상일시 Thirsty 추가
        if (thirsty > 60) animal.AddEffect(AnimalControl.Effect.Thirsty);
        else animal.RemoveEffect(AnimalControl.Effect.Thirsty);

        // 지루함이 60이상일시 Bored 추가
        if (bored > 60) animal.AddEffect(AnimalControl.Effect.Bored);
        else animal.RemoveEffect(AnimalControl.Effect.Bored);


        //----------- Clamp --------------
        bond = Mathf.Clamp(bond, 0f, 100f);
        hungry = Mathf.Clamp(hungry, 0f, 100f);
        thirsty = Mathf.Clamp(thirsty, 0f, 100f);
        bored = Mathf.Clamp(bored, 0f, 100f);
    }
}
