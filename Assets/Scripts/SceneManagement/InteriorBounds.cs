using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public static class InteriorBounds
{
    public static Dictionary<SceneDetails, Tilemap> loadedInteriors = new Dictionary<SceneDetails, Tilemap>();

    public static void Register(SceneDetails sceneDetails, Scene scene)
    {
        var tilemap = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<TilemapRenderer>()).FirstOrDefault(t => t.sortingLayerName == "Background")?.GetComponent<Tilemap>();

        loadedInteriors[sceneDetails] = tilemap;
    }

    public static void Unregister(SceneDetails sceneDetails)
    {
        loadedInteriors.Remove(sceneDetails);
    }

    public static bool IsMoveAllowed(Vector3 initialPosition, Vector3 finalPosition)
    {
        foreach (var tilemap in loadedInteriors.Values)
        {
            if (TilemapHasTile(initialPosition, tilemap))
                return TilemapHasTile(finalPosition, tilemap) || IsExit(finalPosition);
        }

        return true;
    }

    private static bool IsExit(Vector3 position)
    {
        return Physics2D.OverlapCircle(position, 0.2f, GameLayers.i.PortalLayer) != null;
    }

    static bool TilemapHasTile(Vector3 position, Tilemap tilemap)
    {
        return tilemap != null && tilemap.HasTile(tilemap.WorldToCell(position));
    }
}