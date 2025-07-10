using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class AnimalPlay : AnimalAbility
{
    [SerializeField] float stopDistance = 1.5f;
    private NavMeshAgent agent;
    public Transform ballpos;
    [ReadOnlyInspector] [SerializeField] Transform playerCam;
    private bool isPlay = false;
    public bool isBite = false;

    /*
     * ���� ������ ĳ���ʹ� ���� ���󰡼� ���� �ֿ췯 ����.
     * ���� ������ ���� ã�´�.
     * ���� ��� ���ƿ´�.
     */
    protected override void Awake()
    {
            base.Awake();
            TryGetComponent(out agent);
            playerCam = Camera.main.transform;
            target.TryGetComponent(out toy);
    }

    public override void Init()
    {
        StopCoroutine(nameof(PlayBall));
        StartCoroutine(nameof(PlayBall));
        agent.isStopped = false;
    }
    public override void UnInit()
    {
        base.UnInit();
        StopCoroutine(nameof(PlayBall));
        agent.isStopped = true;
    }

    public Transform target;
    Toy toy;
    public Collider[] Collider;
    private IEnumerator PlayBall()
    {
        // 플레이어에게 다가오는거 구현
        // 공까지 가는 동안 대기
        while (true)
        {
            //  공하고 stopDistance 보다 멀면 계속 플레이어 추적, 너무 가까우면 루프문에서 대기는 하는데 추적은 안함
            agent.isStopped = false;
            agent.SetDestination(playerCam.position);
            while (true)
            {

                if ((target.position - transform.position).sqrMagnitude < stopDistance * stopDistance)
                {
                    agent.isStopped = true;
                    break;
                }
                else
                {
                    agent.isStopped = false;
                }
                if (toy.isThrow) break;
                yield return null;


            }
            if (toy.isThrow) break;
            yield return new WaitForSeconds(0.2f);
        }

        // 던져진 공을 쫒아간다.
        // 플레이어에게 다가오는거 구현
        // 공까지 가는 동안 대기
        float startTime = Time.time;
        Rigidbody ball_rb = target.GetComponent<Rigidbody>();
        while (true)
        {
            agent.SetDestination(target.position);
            yield return new WaitForSeconds(0.2f);
            if (Time.time - startTime > 1f && ((target.position - transform.position).sqrMagnitude < stopDistance * stopDistance))
            {
                break;
            }
        }

        Debug.Log(" 던져진 공에 도착 완료했습니다.");


        // 공을 ballPos 에 자식으로 부착하는데 이때 공의 rigidbody나 움직이던거를 다 정지로 초기화 해야함. 선택사항) 이때 이 공을 XR 그랩 못하게함
        yield return new WaitForSeconds(1f);

        agent.isStopped = true;
        target.SetParent(ballpos);
        Debug.Log("입에 넣기");


        yield return new WaitForSeconds(1f);

        while(true)
        {
            agent.isStopped = false;
            Debug.Log("1.플레이어에게");
            agent.SetDestination(playerCam.position);
            target.localPosition = Vector3.zero;
            Debug.Log("간다");
            if (Vector3.Distance(transform.position, playerCam.position) < stopDistance + 1f)
            {
                target.SetParent(null);
                target.position += transform.forward;
                ball_rb.isKinematic = false;
                toy.isThrow = false;
                Debug.Log("놀아주기 끝");
                animal.petStateController.Play();
                break;
            }
            yield return null;
        }
        
       

        // 종료
        animal.ChangeState(AnimalControl.State.Idle);







    }
}
