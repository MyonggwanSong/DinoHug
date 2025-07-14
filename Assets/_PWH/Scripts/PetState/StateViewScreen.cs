using UnityEngine;

public class StateViewScreen : MonoBehaviour
{
    [SerializeField] GameObject view_On;

    public void OnClickPowerButton()
    {
        if (view_On.gameObject.activeSelf)
        {
            view_On.SetActive(false);
        }
        else
        {
            view_On.SetActive(true);
        }
    }
}