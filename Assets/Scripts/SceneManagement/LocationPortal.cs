using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// The location portal teleports the player to a different position without switching scenes.

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] PrimaryPortalID portalA;
    [SerializeField] SecondaryPortalID portalB;
    [SerializeField] TertiaryPortalID portalC;
    [SerializeField] SceneReference destinationScene;

    PlayerController player;

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        this.player = player;
        StartCoroutine(Teleport());
    }

    public bool TriggerRepeatedly => false;

    Fader fader;

    private void Start()
    {
        fader = FindFirstObjectByType<Fader>(FindObjectsInactive.Exclude);
    }

    IEnumerator Teleport()
    {
        DontDestroyOnLoad(gameObject);
        if (gameObject.transform.parent != null)
        {
            DontDestroyOnLoad(gameObject.transform.parent);
            if (gameObject.transform.parent.transform.parent != null)
                DontDestroyOnLoad(gameObject.transform.parent.transform.parent);
        }

        GameController.Instance.PauseGame(true);
        yield return fader.FadeIn(0.5f);

        yield return LoadDestinationScene();

        var destinationPortal = FindObjectsByType<LocationPortal>(FindObjectsSortMode.InstanceID).First(x => x != this && x.portalA == this.portalA && x.portalB == this.portalB && x.portalC == this.portalC);
        player.Character.SetPositionAndSnapToTile(destinationPortal.spawnPoint.position);

        player.Character.Animator.MoveX = destinationPortal.spawnPoint.localPosition.x;
        player.Character.Animator.MoveY = destinationPortal.spawnPoint.localPosition.y;

        yield return fader.FadeOut(0.5f);
        GameController.Instance.PauseGame(false);

        if (gameObject.transform.parent != null)
        {
            if (gameObject.transform.parent.transform.parent != null)
                Destroy(gameObject.transform.parent.transform.parent);

            Destroy(gameObject.transform.parent);
        }

        Destroy(gameObject);
    }

    IEnumerator LoadDestinationScene()
    {
        var currentScene = GameController.Instance.CurrentScene;
        if (currentScene != null)
        {
            var scenesToLoad = (currentScene.IsInterior) ? currentScene.ConnectedScenes : currentScene.ConnectedInteriorScenes;
            if (!string.IsNullOrEmpty(destinationScene.SceneName))
            {
                var targetScene = scenesToLoad.FirstOrDefault(s => s.SceneName == this.destinationScene.SceneName);
                if (targetScene != null)
                    yield return targetScene.LoadScene();
            }
            else
            {
                foreach (var scene in scenesToLoad)
                {
                    if (!scene.IsLoaded)
                        yield return scene.LoadScene();
                }
            }
        }
    }

    public Transform SpawnPoint => spawnPoint;
}

public enum PrimaryPortalID { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z }
public enum SecondaryPortalID { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z }
public enum TertiaryPortalID { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z }