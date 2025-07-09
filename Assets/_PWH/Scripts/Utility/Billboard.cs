using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] Camera targetCam;

    void Start()
    {
        targetCam = Camera.main;
    }

    void OnEnable()
    {
        if (targetCam == null)
        {
            targetCam = Camera.main;
        }
    }

    void Update()
    {
        ApplyBillboard();
    }

    void ApplyBillboard()
    {
        this.gameObject.transform.LookAt(targetCam.transform);
    }
}
