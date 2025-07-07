using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    public float force = 5f;
    public float angle = 30f;

    public Transform direction;

    private Rigidbody rb;
    private Vector3 init_vel;

    public void Throwarc()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            return;
        }

        Vector3 forward = transform.forward.normalized;

        //각도를 라디안으로
        float rad = angle * Mathf.Deg2Rad;

        // 속도 계산
        float horzForce = Mathf.Cos(rad) * force;   //수평
        float vertForce = Mathf.Sin(rad) * force;   //수직

        //초기 속도
        init_vel = horzForce * forward + Vector3.up * vertForce;

        // 힘적용
        rb.AddForce(init_vel, ForceMode.VelocityChange);
    }

}