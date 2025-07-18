using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BehaviourSingleton<GameManager>
{
    [SerializeField, ReadOnlyInspector] Material selectedMaterial;
    [SerializeField, ReadOnlyInspector] ColorSelector colorSelector;
    [SerializeField] string inGameScene;
    [SerializeField] House house;
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

    public void OnClickGameStartButton()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        house.OpenDoorEvent();

        yield return new WaitForSeconds(3f);
        
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

        if (scene.name == "Scene_")
        {
            if (house == null) return;

            house.ResetHouse();
        }
    }

    void OnInGameSceneLoaded()
    {
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