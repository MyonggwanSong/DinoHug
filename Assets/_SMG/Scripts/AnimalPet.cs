using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class AnimalPet : AnimalAbility
{
    [SerializeField] AnimalControl ac;
    [SerializeField] Animator animator;
    //[SerializeField] float detectRadius = 2f;
    [SerializeField] GameObject popUpKey;

    [SerializeField] XRController controller;

    Collider[] _playerController;

    void Start()
    {
       // popUpKey.SetActive(false);
        anim = GetComponentInChildren<Animator>();
        animator = anim;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("들어옴");
            controller = other.GetComponentInParent<XRController>();
            popUpKey.SetActive(true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("나감");
            controller = null;
            popUpKey.SetActive(false);
        }
    }
    //     void Update()
    //     {
    //         Collider[] cols = Physics.OverlapSphere(transform.position, detectRadius);
    //         if (cols != null)
    //         {
    //             foreach (var col in cols)
    //             {
    //                 // _playerController = col;
    //                 if (col.CompareTag("Player"))
    //                 {
    //  Debug.Log("들어옴");
    //                     controller = col.GetComponent<XRController>();
    //                     popUpKey.SetActive(true);
    //                 }
    //                 else
    //                 {
    //                     controller = null;
    //                     popUpKey.SetActive(false);
    //                 }

    //                 continue;
    //             }
    //         }
    //     }

    public override void Init()
    {
        StartCoroutine(Play());
    }

    public override void UnInit()
    {

    }

    IEnumerator Play()
    {
        yield return null;

        anim.Play("aniClip1");
        
         Debug.Log("들어옴");

    }
    IEnumerator Pet()
    {
        yield return new WaitUntil(() => controller.enableInputActions);
         Debug.Log("들어옴");


    }
}
