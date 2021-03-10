using UnityEngine;

namespace Itsdits.Ravar.Levels { 
    public class MapLayers : MonoBehaviour
    {
        public static MapLayers Instance { get; set; }

        [SerializeField] LayerMask objectsLayer;
        [SerializeField] LayerMask encountersLayer;
        [SerializeField] LayerMask interactLayer;
        [SerializeField] LayerMask playerLayer;
        [SerializeField] LayerMask losLayer;

        public LayerMask ObjectsLayer => objectsLayer;
        public LayerMask EncountersLayer => encountersLayer;
        public LayerMask InteractLayer => interactLayer;
        public LayerMask PlayerLayer => playerLayer;
        public LayerMask LosLayer => losLayer;

        private void Awake()
        {
            Instance = this;
        }
    }
}