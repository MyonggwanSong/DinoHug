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

        //������ ��������
        float rad = angle * Mathf.Deg2Rad;

        // �ӵ� ���
        float horzForce = Mathf.Cos(rad) * force;   //����
        float vertForce = Mathf.Sin(rad) * force;   //����

        //�ʱ� �ӵ�
        init_vel = horzForce * forward + Vector3.up * vertForce;

        // ������
        rb.AddForce(init_vel, ForceMode.VelocityChange);
    }

}