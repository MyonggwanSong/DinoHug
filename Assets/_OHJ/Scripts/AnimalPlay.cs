using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalPlay : AnimalAbility
{
    [SerializeField] float stopDistance = 1.5f;
    [SerializeField] float maxTime = 20f;
    [SerializeField] Transform ballPos;
    Transform playerCam;

    private Coroutine Particle_co;

    private SFX sfx;    // 효과음 

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out agent);
        playerCam = Camera.main.transform;
    }
    public override void Init()
    {
        StopCoroutine(nameof(FollowPlayer));
        StartCoroutine(nameof(FollowPlayer));
        agent.isStopped = false;
        Debug.Log("AnimalPlay 시작");
    }
    public override void UnInit()
    {
        base.UnInit();
        Debug.Log("AnimalPlay 종료");
        Reset();
    }
    private void Reset()
    {
        if (toy != null)
        {
            toy.rb.isKinematic = false;
            toy.isThrow = false;
            toy.coll.enabled = true;
        }
        if (toy != null && toy.transform.parent != null)
        {
            toy.transform.SetParent(null);
        }
        agent.isStopped = false;
        toy.EnableGrab();
        StopCoroutine(nameof(FollowPlayer));
        StopCoroutine(nameof(FollowToy));
        StopCoroutine(nameof(BackToPlayer));
        StopCoroutine(nameof(BiteBall));
        StopCoroutine(nameof(PlayParticle));
    }
    public Toy toy;
    IEnumerator FollowPlayer()
    {
        Debug.Log("FollowPlayer 시작");
        // 공에게 이동 (아랫줄만 호출해도 알아서 시간에따라서 이동)
        bool result = agent.SetDestination(toy.transform.position);
        anim.SetInteger("animation", 18);

        sfx = AudioManager.Instance.PlayEffect("Run", transform.position, 0.66f);

        // 위에서 목적지까지 도착하기 전까지 대기
        float sqrDistance = (toy.transform.position - transform.position).sqrMagnitude;
        float startTime = Time.time;
        while (sqrDistance > stopDistance * stopDistance)
        {
            if (Particle_co != null)
            {
                StopCoroutine(nameof(PlayParticle));
            }
            Particle_co = StartCoroutine(nameof(PlayParticle));

            //플레이어가 위치를 계속 바꾸므로 경로도 0,1초마 재 계산
            result = agent.SetDestination(toy.transform.position);
            // 거리 재 계산
            sqrDistance = (toy.transform.position - transform.position).sqrMagnitude;

            if (sqrDistance <= stopDistance * stopDistance)
            {
                agent.isStopped = true;
                agent.ResetPath();
                break;
            }

            if (!result)
            {
                Debug.Log("경로를 못찾았습니다.");
            }

            if (toy.isThrow)
            {
                Debug.Log("플레이어 손에있는 공을 따라가려다가 도착하기도전에 플레이어 미리 던졌습니다.");
                StartCoroutine(nameof(FollowToy));
                yield break;
            }

            if (toy == null)
            {
                Debug.Log("플레이어 손에있는 공을 따라가려다가. 공이 파괴");
            }

            if (toy != null && !toy.gameObject.activeInHierarchy)
            {
                Debug.Log("플레이어 손에있는 공을 따라가려다가 공이 비활성화됨");
            }

            if (Time.time - startTime > maxTime)
            {
                Debug.Log("플레이어 손에있는 공을 따라가려다가 시간초과");
                animal.ChangeState(AnimalControl.State.Idle);
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }

        agent.isStopped = false;
        Debug.Log("플레이어 손에있는 공에 도착완료");
        // 플레이어 손에 있는 공에 도착
        anim.SetInteger("animation", 1);
        sfx?.Stop();    // 소리 멈춤

        if (Particle_co != null)
        {
            StopCoroutine(nameof(PlayParticle));
        }
        Debug.Log("파티클 멈춰라");

        while (true)
        {
            if (toy.isThrow)
            {
                StartCoroutine(nameof(FollowToy));
                yield break;
            }
            if (toy == null)
            {
                Debug.Log("플레이어 손에 있는공이 던질때까지 기다리고있는데. 공이 파괴");
            }

            if (toy != null && !toy.gameObject.activeInHierarchy)
            {
                Debug.Log("플레이어 손에 있는공이 던질때까지 기다리고있는데  공이 비활성화됨");
            }

            yield return null;
        }
    }

    IEnumerator FollowToy()
    {
        Debug.Log("FollowToy 시작");
        // 공을 던진순간 공룡과 공이 충분히 멀어질떄까지 무조건 대기
        yield return new WaitForSeconds(2f);


        // 공에게 이동 (아랫줄만 호출해도 알아서 시간에따라서 이동)
        bool result = agent.SetDestination(toy.transform.position);
        anim.SetInteger("animation", 18);
        sfx = AudioManager.Instance.PlayEffect("Run", transform.position, 0.66f);

        // 위에서 목적지까지 도착하기 전까지 대기
        float sqrDistance = (toy.transform.position - transform.position).sqrMagnitude;
        float startTime = Time.time;
        while (sqrDistance > stopDistance * stopDistance)
        {
            if (Particle_co != null)
            {
                StopCoroutine(nameof(PlayParticle));
            }
            Particle_co = StartCoroutine(nameof(PlayParticle));

            //플레이어가 위치를 계속 바꾸므로 경로도 0,1초마 재 계산
            result = agent.SetDestination(toy.transform.position);
            // 거리 재 계산
            sqrDistance = (toy.transform.position - transform.position).sqrMagnitude;

            if (!result)
            {
                Debug.Log("경로를 못찾았습니다.");
            }

            if (toy.isGrab)
            {
                Debug.Log("플레이어 던져진 공을 따라가려다가 도착하기도전에 플레이어 그랩해버렸습니다");
                animal.ChangeState(AnimalControl.State.Idle);
                yield break;
            }

            if (toy == null)
            {
                Debug.Log("플레이어 던져진 공을 따라가려다가. 공이 파괴");
            }

            if (toy != null && !toy.gameObject.activeInHierarchy)
            {
                Debug.Log("플레이어 던져진 공을 따라가려다가 공이 비활성화됨");
            }

            if (Time.time - startTime > maxTime)
            {
                Debug.Log("플레이어 던져진 공을 따라가려다가 시간초과");
                animal.ChangeState(AnimalControl.State.Idle);
                yield break;
            }


            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("던져진 공에 도착완료");
        // 도착 -> 공이 공룡 입에 부착


        anim.SetInteger("animation", 14);
        sfx?.Stop();    // 소리 멈춤
        if (Particle_co != null)
        {
            StopCoroutine(nameof(PlayParticle));
        }

        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlayEffect("DollSound", transform.position, 1f);
        anim.SetInteger("animation", 1);

        toy.DisableGrab();
        toy.rb.isKinematic = true;
        toy.coll.enabled = false;

        toy.transform.SetParent(ballPos);
        startTime = Time.time;
        float duration = 2f;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            toy.transform.position = Vector3.Lerp(transform.position, ballPos.position, t);
            yield return null;
        }

        //toy.transform.position = ballPos.position;
        StartCoroutine(nameof(BiteBall));


        //yield return null;

        StartCoroutine(nameof(BackToPlayer));


    }
    IEnumerator BackToPlayer()
    {
        Debug.Log("backtoplayer 시작");

        agent.angularSpeed = 300f;
        agent.acceleration = 180f;
        yield return new WaitForSeconds(0.5f);
        bool result = agent.SetDestination(playerCam.transform.position);
        anim.SetInteger("animation", 21);

        // 위에서 목적지까지 도착하기 전까지 대기
        float sqrDistance = (playerCam.transform.position - transform.position).sqrMagnitude;
        float startTime = Time.time;

        while (sqrDistance > (stopDistance + 1) * (stopDistance + 1))
        {
            //플레이어가 위치를 계속 바꾸므로 경로도 0,1초마 재 계산
            result = agent.SetDestination(playerCam.transform.position);
            // 거리 재 계산
            sqrDistance = (playerCam.transform.position - transform.position).sqrMagnitude;

            if (!result)
            {
                Debug.Log("경로를 못찾았습니다.");
            }

            if (toy == null)
            {
                Debug.Log("플레이어게 공을 회수하다가. 공이 파괴");
            }

            if (toy != null && !toy.gameObject.activeInHierarchy)
            {
                Debug.Log("플레이어게 공을 회수하다가 공이 비활성화됨");
            }

            if (Time.time - startTime > maxTime)
            {
                Debug.Log("플레이어게 공을 회수하다가 시간초과");
                animal.ChangeState(AnimalControl.State.Idle);
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }


        Debug.Log("도착");

        // 플레이어에 도착
        // 플레이어 방향으로 공을 살짝 던짐

        // 이동 멈춤
        agent.isStopped = true;
        agent.ResetPath();

        StopCoroutine(nameof(BiteBall));
        anim.SetInteger("animation", 2);
        toy.coll.enabled = true;
        toy.transform.SetParent(null);
        toy.transform.position = ballPos.position;
        Vector3 look = playerCam.position - transform.position;
        look.y = 0f;
        look.Normalize();
        transform.forward = look;
        toy.transform.position = transform.position + look;
        yield return null;
        toy.rb.isKinematic = false;
        toy.rb.AddForce(look);
        yield return null;
        toy.EnableGrab();
        toy.isThrow = false;

        agent.angularSpeed = 120f;
        agent.acceleration = 8f;

        animal.petStateController.Play();   // 끝나면 지루함 떨어짐
        Debug.Log("놀아주기 완료");

        animal.ChangeState(AnimalControl.State.Idle);

    }
    IEnumerator BiteBall()
    {
        while (true)
        {
            toy.transform.position = ballPos.position;
            yield return null;
        }
    }

    IEnumerator PlayParticle()
    {
        ParticleManager.Instance.SpawnParticle(ParticleFlag.Playing, transform.position + Vector3.up, Quaternion.identity, this.transform);

        yield return new WaitForSeconds(0.5f);
    }

    //[SerializeField] float stopDistance = 1.5f;
    //[ReadOnlyInspector] [SerializeField] Transform playerCam;
    //[SerializeField] private float maxPlayTime = 10f;


    //private NavMeshAgent agent;
    //public Transform ballpos;


    //private bool isPlay = false;
    //public bool isBite = false; 

    //protected override void Awake()
    //{
    //        base.Awake();
    //        TryGetComponent(out agent);
    //        playerCam = Camera.main.transform;
    //        target.TryGetComponent(out toy);
    //}

    //public override void Init()
    //{
    //    StopCoroutine(nameof(PlayBall));
    //    StartCoroutine(nameof(PlayBall));
    //    agent.isStopped = false;
    //}
    //public override void UnInit()
    //{
    //    base.UnInit();
    //    StopCoroutine(nameof(PlayBall));
    //    agent.isStopped = true;
    //    target.SetParent(null);
    //    if(ball_rb != null)
    //        ball_rb.isKinematic = false;
    //    //if(toy != null)
    //    //    toy.isThrow = false;
    //}

    //public Transform target;
    //Toy toy;
    //public Collider[] Collider;
    //Rigidbody ball_rb;
    //private IEnumerator PlayBall()
    //{
    //    // 플레이어에게 다가오는거 구현
    //    // 공까지 가는 동안 대기
    //    while (true)
    //    {
    //        //  공하고 stopDistance 보다 멀면 계속 플레이어 추적, 너무 가까우면 루프문에서 대기는 하는데 추적은 안함
    //        agent.isStopped = false;
    //        bool result1 = agent.SetDestination(playerCam.position);
    //        while (!toy.isThrow && toy.isGrabbed)
    //        {
    //            if ((target.position - transform.position).sqrMagnitude < stopDistance * stopDistance)
    //            {
    //                Debug.Log("저 왔습니다.");
    //                agent.isStopped = true;
    //                break;
    //            }
    //            else
    //            {
    //                agent.isStopped = false;
    //            }
    //            if (toy.isThrow) break;
    //            // 예외처리 (플레이어가 공을잡거나, 공null, setactive, result = false, ... ,...)
    //            yield return null;
    //            Debug.Log("주인님께가고 있습니다.");
    //        }
    //        if (toy.isThrow) break;
    //        // 예외처리 (플레이어가 공을잡거나, 공null, setactive, result = false, ... ,...)



    //        yield return new WaitForSeconds(0.2f);
    //    }

    //    // 던져진 공을 쫒아간다.
    //    // 플레이어에게 다가오는거 구현
    //    // 공까지 가는 동안 대기
    //    float startTime = Time.time;
    //    ball_rb = target.GetComponent<Rigidbody>();
    //    while (true)
    //    {
    //        agent.SetDestination(target.position);
    //        Debug.Log("공 잡으러 갑니다.");

    //        //temp
    //        if(toy.isGrabbed)
    //        {
    //            toy.isThrow = false;
    //            Debug.Log("던지고 있는 공을 잡았습니다.");
    //            break;
    //        }

    //        yield return new WaitForSeconds(0.2f);
    //        // 예외처리 (플레이어가 공을잡거나, 공null, setactive, result = false, ... ,...)
    //        if (Time.time - startTime > 1f && ((target.position - transform.position).sqrMagnitude < stopDistance * stopDistance))
    //        {
    //            break;
    //        }
    //    }

    //    Debug.Log(" 던져진 공에 도착 완료했습니다.");


    //    // 공을 ballPos 에 자식으로 부착하는데 이때 공의 rigidbody나 움직이던거를 다 정지로 초기화 해야함. 선택사항) 이때 이 공을 XR 그랩 못하게함
    //    yield return new WaitForSeconds(1f);

    //    agent.isStopped = true;
    //    target.SetParent(ballpos);
    //    Debug.Log("입에 넣기");
    //    // 공 그랩못하게


    //    yield return new WaitForSeconds(1f);



    //    bool result2 = agent.SetDestination(playerCam.position);
    //    float elapsed = Time.time;
    //    while(true)
    //    {
    //        agent.isStopped = false;
    //        Debug.Log("1.플레이어에게 갑니다");

    //        target.localPosition = Vector3.zero;
    //        Debug.Log("물고 옵니다");
    //        toy.isThrow = false;

    //        if (Vector3.Distance(transform.position, playerCam.position) < stopDistance + 1f)
    //        {
    //            target.SetParent(null);
    //            target.position += transform.forward;
    //            ball_rb.isKinematic = false;
    //            Debug.Log("놀아주기 끝");
    //            animal.petStateController.Play();
    //            break;
    //        }

    //        if(!result2)
    //        {
    //            result2 = agent.SetDestination(playerCam.position);
    //        }


    //        if (Time.time - elapsed > maxPlayTime)
    //        {
    //            Debug.Log(Time.time - elapsed);
    //            target.SetParent(null);
    //            ball_rb.isKinematic = false;
    //            Debug.Log("시간 지났으니 놀아주기 끝");
    //            break;
    //        }

    //        yield return null;
    //        // 예외처리 (플레이어가 공을잡거나, 공null, setactive, result = false, ... ,...)
    //    }



    //    // 종료
    //    animal.ChangeState(AnimalControl.State.Idle);
    //    Debug.Log("Idle전환");

    //}

}
