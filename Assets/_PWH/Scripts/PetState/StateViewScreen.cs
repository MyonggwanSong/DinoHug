using UnityEngine;

public class StateViewScreen : MonoBehaviour
{
    [SerializeField] GameObject view_On;
    [SerializeField] GameObject view_Off;

    public void OnClickPowerButton()
    {
        if (view_On.gameObject.activeSelf)
        {
            view_On.SetActive(false);
            // view_Off.SetActive(true);
        }
        else
        {
            view_On.SetActive(true);
            // view_Off.SetActive(false);
        }
    }
}