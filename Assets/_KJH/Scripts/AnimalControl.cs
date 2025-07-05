using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimalControl : MonoBehaviour
{
    public State state;
    #region State Machine 패턴
    public enum State
    {
        Idle,
        Dead,
        Wander,
        GoTo,
        Eat,
        Handle,
        Drink,
        Play,
    }
    void Start()
    {
        AnimalAbility[] abilities = GetComponents<AnimalAbility>();
        // 최초 실행시 AnimalAbility 스크립트들 다 꺼지게 합니다.
        for (int i = 0; i < abilities.Length; i++)
        {
            abilities[i].enabled = false;
            dictionary.Add((State)i, abilities[i]);
        }
        StartCoroutine(nameof(StateMachine));
        StartCoroutine(nameof(Sensor));
        // 처음에는 Idle 상태로 시작하게 합니다.
        state = State.Idle;
    }
    Dictionary<State, AnimalAbility> dictionary = new Dictionary<State, AnimalAbility>();
    State prevState = State.Dead;
    // state가 바뀌면 자동으로 해당하는 스크립트를 활성화시키고. 이전 스크립트는 자동으로 비활성화 시킵니다.
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
    #endregion
    public enum Effect
    {
        None = 0,
        Hungry = 1 << 0,
        Thirsty = 1 << 1,
        Bored = 1 << 2,
    }
    public Effect effect;
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
    Collider[] colliders = new Collider[20];
    [SerializeField] List<Collider> nearInteractableList = new List<Collider>();
    IEnumerator Sensor()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            nearInteractableList.Clear();
            Vector3 tPos = transform.position;
            int count = Physics.OverlapSphereNonAlloc(tPos + 3f * transform.forward, 9f, colliders);
            for (int i = 0; i < count; i++)
            {
                nearInteractableList.Add(colliders[i]);
            }
            nearInteractableList.Sort((a, b) => (tPos - a.transform.position).sqrMagnitude.CompareTo((tPos - b.transform.position).sqrMagnitude));
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + 3f * transform.forward, 9f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
#endif





}
