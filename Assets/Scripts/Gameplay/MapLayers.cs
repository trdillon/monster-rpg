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

    public LayerMask ObjectsLayer => objectsLayer;
    public LayerMask EncountersLayer => encountersLayer;
    public LayerMask InteractLayer => interactLayer;
    public LayerMask PlayerLayer => playerLayer;
    public LayerMask LosLayer  => losLayer;
}
