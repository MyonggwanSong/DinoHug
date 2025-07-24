using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingControl : MonoBehaviour
{
    private static string nextScene;
    [SerializeField] float loadingTime;

    float elapsed = 0f; 
    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    public static void LoadScene(string sceneName)
    {
        Debug.Log("로딩씬으로 이동...");
        nextScene = sceneName;
        SceneManager.LoadScene("SceneLoading");
    }

    IEnumerator LoadSceneAsync()
    {
        yield return null;

        Debug.Log("Loading Scene...");
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(loadingTime);

        operation.allowSceneActivation = true;

        elapsed = 0f; 
    }
}
