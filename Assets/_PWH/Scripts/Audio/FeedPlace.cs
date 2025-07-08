using UnityEngine;

public class FeedPlace : MonoBehaviour
{
    [Header("Place Clip")]
    [SerializeField] AudioSource audio;         //3D Sound

    [Header("Volue Test Flag")]
    public bool isEating;
    void Start()
    {
            
    }
    
    void StartEatPerform()
    {
        Debug.Log("Eat...");
        audio.Play();
    }
}