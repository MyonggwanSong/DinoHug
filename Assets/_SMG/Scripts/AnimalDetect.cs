using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AnimalDetect : MonoBehaviour
{
    [SerializeField] float detectRadius = 2f;
    AnimalControl animal;
    XRController controller;
    [SerializeField] GameObject popUpKey;


    void Awake()
    {
        animal = GetComponent<AnimalControl>();
    }
  
 

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
               controller = other.GetComponentInParent<XRController>();        // 진동 효과 활용 용
            popUpKey.SetActive(true);


            if (animal.state != AnimalControl.State.Play) return;
            
            animal.ChangeState(AnimalControl.State.Idle);
             
            

        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            controller = null;
            popUpKey.SetActive(false);

            if (animal.state != AnimalControl.State.Play) return;

            animal.ChangeState(AnimalControl.State.Idle);
        }
    }
    

    // void Update()
    // {
    //     Collider[] cols = Physics.OverlapSphere(transform.position, detectRadius);
    //     if (cols != null)
    //     {
    //         foreach (var col in cols)
    //         {

    //             if (col.CompareTag("Player"))
    //             {
    //                 _timer += Time.deltaTime;
    //                 Debug.Log(_timer);  
    //                 animal.ChangeState(AnimalControl.State.Idle);

    //                 Debug.Log("들어옴");
    //                 controller = col.GetComponent<XRController>();  // 진동 효과 및 자이로스코프 활용 용

    //                 popUpKey.SetActive(true);   // 쓰다듬기 활성 가능
    //                 if (_timer > 2f) // 들어온 시간이 2초가 넘으면 실행
    //                 {
    //                     _timer = 0f;
    //                     animal.ChangeState(AnimalControl.State.Handle);
    //                 }
    //                 return;
    //             }
    //         }
    //         Debug.Log("나감");
    //         animal.ChangeState(AnimalControl.State.Wander);
    //         controller = null;
    //         popUpKey.SetActive(false);
    //     }

    // }


}
