using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BehaviourSingleton<GameManager>
{
    [SerializeField, ReadOnlyInspector] Material selectedMaterial;
    [SerializeField, ReadOnlyInspector] ColorSelector colorSelector;
    [SerializeField] string inGameScene;

    protected override void Awake()
    {
        base.Awake();

        colorSelector = FindAnyObjectByType<ColorSelector>();
        colorSelector.OnUpdateSelectedMaterial += SetSelectedMaterial;
    }

    void SetSelectedMaterial(Material mat)
    {
        selectedMaterial = mat;
    }

    public void OnClickInGameButton()
    {
        SceneManager.LoadScene(inGameScene);
    }


    #region SceneManager
    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SceneHome_Fixed")
        {
            OnInGameSceneLoaded();
        }
    }

    void OnInGameSceneLoaded()
    {
        Debug.Log("Ingame Scene Load");
        AnimalControl animal = FindAnyObjectByType<AnimalControl>();

        if (animal == null)
        {
            Debug.Log("참조할 animal이 없습니다.");
            return;
        }

        animal.GetComponentInChildren<CharacterMeshControl>()?.SetMaterial(selectedMaterial);
    }
    #endregion

    protected override bool IsDontDestroy() => true;
}