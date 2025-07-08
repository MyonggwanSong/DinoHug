using UnityEngine;

public class EnvironmentView : MonoBehaviour
{
    [SerializeField] GameObject currentView;

    [Header("View")]
    [SerializeField] GameObject view_Main;
    [SerializeField] GameObject view_Sound;

    void OnEnable()
    {
        //처음 창은 Sound로 세팅
        currentView = view_Sound;
    }

    public void OnClickSoundeButton()
    {
        currentView.SetActive(false);
        currentView = view_Sound;
        currentView.SetActive(true);
    }

    public void SetActiveView(bool on) => gameObject.SetActive(on);

    void OnDisable()
    {
        view_Main.SetActive(true);
    }
}
