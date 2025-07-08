using System.Collections;
using UnityEngine;

public class SongsBilllboard : MonoBehaviour
{
    [SerializeField] private Transform mainCam;
    [SerializeField] Transform offset;
    [SerializeField] float offsetfoce;
    IEnumerator Start()
    {
        yield return new WaitUntil(() => Camera.main != null);
        mainCam = Camera.main.transform;
        gameObject.SetActive(false);

    }

    void Update()
    {
        if (mainCam == null) return;
        Vector3 direction = (mainCam.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(-direction, mainCam.up);
        
        if (offset == null) return;
        offset.position = transform.position - mainCam.forward * offsetfoce;

    }

}
