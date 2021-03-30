using UnityEngine;

namespace Itsdits.Ravar.Levels {
    /// <summary>
    /// Class for holding and providing access to map layers.
    /// </summary>
    public class MapLayers : MonoBehaviour
    {
        public static MapLayers Instance { get; set; }

        [SerializeField] LayerMask objectsLayer;
        [SerializeField] LayerMask encountersLayer;
        [SerializeField] LayerMask interactLayer;
        [SerializeField] LayerMask playerLayer;
        [SerializeField] LayerMask losLayer;
        [SerializeField] LayerMask triggerLayer;

        public LayerMask ObjectsLayer => objectsLayer;
        public LayerMask EncountersLayer => encountersLayer;
        public LayerMask InteractLayer => interactLayer;
        public LayerMask PlayerLayer => playerLayer;
        public LayerMask LosLayer => losLayer;
        public LayerMask TriggerLayer => triggerLayer;
        public LayerMask ActionLayers => encountersLayer | losLayer | triggerLayer;

        private void Awake()
        {
            Instance = this;
        }
    }
}