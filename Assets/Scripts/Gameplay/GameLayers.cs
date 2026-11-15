using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask tallGrassLayer;
    [SerializeField] LayerMask forestGrassLayer;
    [SerializeField] LayerMask caveTilesLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask portalLayer;
    [SerializeField] LayerMask triggerLayer;
    [SerializeField] LayerMask ledgeLayer;
    [SerializeField] LayerMask invisibleInteractableLayer;
    [SerializeField] LayerMask waterLayer;
    [SerializeField] LayerMask strengthBoulderLayer;

    public static GameLayers i { get; set; }

    private void Awake()
    {
        i = this;
    }

    public LayerMask SolidLayer {
        get => solidObjectsLayer;
    }

    public LayerMask InteractableLayer {
        get => interactableLayer;
    }

    public LayerMask TallGrassLayer {
        get => tallGrassLayer;
    }

    public LayerMask ForestGrassLayer {
        get => forestGrassLayer;
    }

    public LayerMask CaveTilesLayer {
        get => caveTilesLayer;
    }

    public LayerMask PlayerLayer {
        get => playerLayer;
    }

    public LayerMask FOVLayer {
        get => fovLayer;
    }

    public LayerMask PortalLayer {
        get => portalLayer;
    }

    public LayerMask LedgeLayer {
        get => ledgeLayer;
    }

    public LayerMask InvisibleInteractableLayer {
        get => invisibleInteractableLayer;
    }

    public LayerMask WaterLayer {
        get => waterLayer;
    }

    public LayerMask StrengthBoulderLayer {
        get => strengthBoulderLayer;
    }

    public LayerMask TriggerableLayers {
        get => tallGrassLayer | forestGrassLayer | caveTilesLayer | fovLayer | portalLayer | triggerLayer | waterLayer;
    }
}