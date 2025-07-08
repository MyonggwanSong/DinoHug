using UnityEngine;

public class MenuControl : MonoBehaviour
{
    [SerializeField] GameObject view_Main;
    [SerializeField] GameObject view_Environment;

    public void OnClickGameStartButton()
    {
        Debug.Log("OnClick Game Start");
    }

    public void OnClickEnvironmentButton()
    {
        Debug.Log("OnClick Environment");

        view_Main.SetActive(false);

        if (view_Environment.activeSelf)
            view_Environment.SetActive(false);
        else
            view_Environment.SetActive(true);   
    }

    public void OnClickExitButton()
    {
        Debug.Log("OnClick Game Exit");
        gameObject.SetActive(false);
    }
}
