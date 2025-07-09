using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuControl : MonoBehaviour
{
    [SerializeField] GameObject view_Main;
    [SerializeField] GameObject view_Environment;

    public void OnClickGameStartButton()
    {
        Debug.Log("OnClick Game Start");
        SceneManager.LoadScene(1);
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
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
