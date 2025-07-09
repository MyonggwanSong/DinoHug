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
     * ���� ������ ĳ���ʹ� ���� ���󰡼� ���� �ֿ췯 ����.
     * ���� ������ ���� ã�´�.
     * ���� ��� ���ƿ´�.
     */
    protected override void Awake()
    {
        base.Awake();
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
        animal.state = AnimalControl.State.Play;
        // ���� ����
        Collider = Physics.OverlapSphere(transform.position, 80f);

        // ��ã��
        for (int i = 0; i < Collider.Length; i++)
        {
            if (Collider[i].CompareTag("Ball"))

            {
                target = Collider[i].gameObject;
                break;
            }
        }

        // ������
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
        //Debug.Log($"{stopDistance} �տ��� ����");
        //agent.isStopped = true;



        // �Ÿ��� ������� ��
        if (Vector3.Distance(transform.position, target.transform.position) < 30f)
        {
            // target�� carry��� �ӽ� ������ ����
            carry = target;

            carry.transform.SetParent(ballpos);

            //�ڿ������� �����̱�
            float elapsed = 0f;
            float duration = 1f;

            while(elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                carry.transform.localPosition = Vector3.Lerp(carry.transform.localPosition, new Vector3(0.01f, 0.1f, 0.5f), t);
                yield return null;
            }

            //�� �߷� ����
            Rigidbody ball_rb = carry.GetComponent<Rigidbody>();
            if (ball_rb != null) ball_rb.isKinematic = true;
            
        }

        //if(Vector3.Distance(transform.position, target.transform.position) < 2f)
        //{
        //    carry = target;

        //    carry.transform.SetParent(ballpos);
        //    carry.transform.localPosition = Vector3.Lerp(carry.transform.position, new Vector3(0.01f, 0.1f, 0.5f), 2f);

        //    // �� �߷� ����
        //    Rigidbody ball_rb = carry.GetComponent<Rigidbody>();
        //    if(ball_rb != null)
        //        ball_rb.isKinematic = true;

        //    yield return null;

        //    //�ִϸ��̼� ����

        //    //carry.transform.localPosition = new Vector3(-1f, 0f, -0.5f);


        //    Debug.Log("1");
        //    // 2�� ���� ����
        //    Debug.Log("2");

        //    yield return new WaitForSeconds(3f);

        //    target = player;
        //    Debug.Log("3");

        //    agent.SetDestination(target.transform.position);

        //    while (Vector3.Distance(transform.position, target.transform.position) < 2f)
        //    {
        //        ball_rb.isKinematic = false;

        //        yield return null;
        //        //�ִϸ��̼� ����
        //    }


        //}
        isPlay = false;


        Debug.Log("���� ����");
        animal.state = AnimalControl.State.Idle;
    }
}
