using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLayers : MonoBehaviour
{
    [SerializeField] LayerMask objectsLayer;
    [SerializeField] LayerMask encountersLayer;
    [SerializeField] LayerMask interactLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask losLayer;

    public static MapLayers Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public LayerMask ObjectsLayer {
        get => objectsLayer;
    }

    public LayerMask EncountersLayer {
        get => encountersLayer;
    }

    public LayerMask InteractLayer {
        get => interactLayer;
    }

    public LayerMask PlayerLayer {
        get => playerLayer;
    }

    public LayerMask LosLayer {
        get => losLayer;
    }
}
