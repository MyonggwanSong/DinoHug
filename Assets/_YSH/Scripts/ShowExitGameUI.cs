using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class ShowExitGameUI : MonoBehaviour
{
    public GameObject panelUI; // Panel UI GameObject

    void Start()
    {
        // 시작 시 패널은 꺼져 있음
        panelUI.SetActive(false);
    }

    // VR에서 버튼을 누르면 이 함수를 호출
    public void ShowGameExitPanel()
    {
        AudioManager.Instance.PlayEffect("UIClick1", transform.position);
        panelUI.SetActive(true);
    }

    // 게임 종료 버튼 (StartScene으로 이동)
    public void ExitGameButton()
    {
        AudioManager.Instance.PlayEffect("UIClick1", transform.position);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    // UI 끄기 버튼
    public void ResumeGameButton()
    {
        AudioManager.Instance.PlayEffect("UIClick1", transform.position);
        panelUI.SetActive(false);
    }
}
