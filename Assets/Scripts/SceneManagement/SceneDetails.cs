using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using E7.Introloop;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class SceneDetails : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] List<SceneDetails> connectedScenes;
    [SerializeField] IntroloopAudio sceneMusic;
    [SerializeField] bool isDark = false;
    [SerializeField] bool isInterior;

    public List<SceneDetails> ConnectedInteriorScenes { get; private set; }
    public List<SceneDetails> ConnectedExteriorScenes { get; private set; }
    public List<SceneDetails> ConnectedScenes => connectedScenes;
    public bool IsLoaded { get; private set; }
    public bool IsInterior => isInterior;

    List<SavableEntity> savableEntities;

    private void Awake()
    {
        if (isInterior)
            ConnectParentScenes();

        ConnectChildScenes();

        ConnectedInteriorScenes = connectedScenes.Where(s => s.isInterior).ToList();
        ConnectedExteriorScenes = connectedScenes.Where(s => !s.isInterior).ToList();
    }

    private void ConnectChildScenes()
    {
        foreach (Transform child in transform)
        {
            var childScene = child.GetComponent<SceneDetails>();
            if (childScene != null)
                AddConnectedScene(childScene);
        }
    }

    private void ConnectParentScenes()
    {
        var parentScene = transform.parent?.GetComponent<SceneDetails>();
        if (parentScene != null)
            AddConnectedScene(parentScene);
    }

    public void AddConnectedScene(SceneDetails newScene)
    {
        if (!connectedScenes.Contains(newScene))
            connectedScenes.Add(newScene);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            LoadScene();
            GameController.Instance.SetCurrentScene(this);

            AudioManager.i.PlayMusic(sceneMusic, true);

            // if we're in an exterior scene, load all connected exterior scenes. Otherwise, don't load any other scenes.
            var scenesToLoad = new List<SceneDetails>();

            if (!isInterior)
                scenesToLoad = ConnectedExteriorScenes;
            
            foreach (var scene in scenesToLoad)
                scene.LoadScene();

            // Unload scenes that aren't connected.
            var previousScene = GameController.Instance.PreviousScene;
            if (previousScene != null)
            {
                var previouslyLoadedScenes = previousScene.connectedScenes;
                foreach (var scene in previouslyLoadedScenes)
                {
                    if (!scenesToLoad.Contains(scene) && scene != this)
                        scene.UnloadScene();
                }

                if (!scenesToLoad.Contains(previousScene))
                    previousScene.UnloadScene();
            }
        }
    }

    public AsyncOperation LoadScene()
    {
        if (!IsLoaded)
        {
            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;

            operation.completed += (AsyncOperation op) =>
            {
                savableEntities = GetSavableEntitiesInScene();
                SavingSystem.i.RestoreEntityStates(savableEntities);

                if (isInterior)
                    InteriorBounds.Register(this, SceneManager.GetSceneByName(gameObject.name));
            };

            return operation;
        }

        return null;
    }

    public void UnloadScene()
    {
        if (IsLoaded)
        {
            SavingSystem.i.CaptureEntityStates(savableEntities);

            InteriorBounds.Unregister(this);

            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }

    void OnDestroy()
    {
        InteriorBounds.Unregister(this);
    }

#if UNITY_EDITOR
    [ContextMenu("Open Scene")]
    public void OpenSceneInEditor()
    {
        if (!EditorSceneManager.GetSceneByName(gameObject.name).isLoaded)
        {
            string path = $"Assets/Scenes/{gameObject.name}.unity";
            EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        }
    }

    [ContextMenu("Close Scene")]
    public void CloseSceneInEditor()
    {
        var scene = EditorSceneManager.GetSceneByName(gameObject.name);
        if (scene.isLoaded)
            EditorSceneManager.CloseScene(scene, true);
    }
#endif
    List<SavableEntity> GetSavableEntitiesInScene()
    {
        var currentScene = SceneManager.GetSceneByName(gameObject.name);
        var savableEntities = FindObjectsByType<SavableEntity>(FindObjectsSortMode.InstanceID).Where(x => x.gameObject.scene == currentScene).ToList();

        return savableEntities;
    }

    public IntroloopAudio SceneMusic => sceneMusic;
    public bool IsDark => isDark;
    public string SceneName => sceneName;
}