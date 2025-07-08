using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalPlay : AnimalAbility
{
    [SerializeField] float stopDistance = 1.5f;
    private NavMeshAgent agent;
    public Transform ballpos;
    public GameObject carry;

    public GameObject player;

    private bool isPlay = false;

    /*
     * 공을 던지면 캐릭터는 공을 따라가서 공을 주우러 간다.
     * 범위 내에서 공을 찾는다.
     * 공을 집어서 돌아온다.
     */
    private void Awake()
    {
        TryGetComponent(out agent);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if(!isPlay)
            Init();
    }

    public override void Init()
    {
        StopCoroutine(nameof(PlayBall));
        StartCoroutine(nameof(PlayBall));
        agent.isStopped = false;
    }
    public override void UnInit()
    {
        StopCoroutine(nameof(PlayBall));
        agent.isStopped = true;
    }

    public GameObject target;
    public Collider[] Collider;
    private IEnumerator PlayBall()
    {
        isPlay = true;
        state = AnimalControl.State.Play;
        // 범위 지정
        Collider = Physics.OverlapSphere(transform.position, 80f);

        // 공찾기
        for (int i = 0; i < Collider.Length; i++)
        {
            if (Collider[i].CompareTag("Ball"))

            {
                target = Collider[i].gameObject;
                break;
            }
        }

        // 목적지
        agent.SetDestination(target.transform.position);

        while(true)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            Debug.Log(dist);
            if(dist < stopDistance)
            {
                agent.isStopped = true;
                break;
            }
            yield return null;
        }


        //yield return new WaitForSeconds(2f);
        //yield return new WaitUntil(() => Vector3.Distance(transform.position, target.transform.position) < stopDistance);
        //Debug.Log($"{stopDistance} 앞에서 멈춤");
        //agent.isStopped = true;



        // 거리가 가까워질 때
        if (Vector3.Distance(transform.position, target.transform.position) < 30f)
        {
            // target을 carry라는 임시 변수에 대입
            carry = target;

            carry.transform.SetParent(ballpos);

            //자연스럽게 움직이기
            float elapsed = 0f;
            float duration = 1f;

            while(elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                carry.transform.localPosition = Vector3.Lerp(carry.transform.localPosition, new Vector3(0.01f, 0.1f, 0.5f), t);
                yield return null;
            }

            //공 중력 끄기
            Rigidbody ball_rb = carry.GetComponent<Rigidbody>();
            if (ball_rb != null) ball_rb.isKinematic = true;
            
        }

        //if(Vector3.Distance(transform.position, target.transform.position) < 2f)
        //{
        //    carry = target;

        //    carry.transform.SetParent(ballpos);
        //    carry.transform.localPosition = Vector3.Lerp(carry.transform.position, new Vector3(0.01f, 0.1f, 0.5f), 2f);

        //    // 공 중력 끄기
        //    Rigidbody ball_rb = carry.GetComponent<Rigidbody>();
        //    if(ball_rb != null)
        //        ball_rb.isKinematic = true;

        //    yield return null;

        //    //애니메이션 실행

        //    //carry.transform.localPosition = new Vector3(-1f, 0f, -0.5f);


        //    Debug.Log("1");
        //    // 2초 동안 멈춤
        //    Debug.Log("2");

        //    yield return new WaitForSeconds(3f);

        //    target = player;
        //    Debug.Log("3");

        //    agent.SetDestination(target.transform.position);

        //    while (Vector3.Distance(transform.position, target.transform.position) < 2f)
        //    {
        //        ball_rb.isKinematic = false;

        //        yield return null;
        //        //애니메이션 실행
        //    }


        //}
        isPlay = false;


        Debug.Log("실행 종료");
        state = AnimalControl.State.Idle;
    }
}
