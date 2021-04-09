using UnityEngine;

namespace Itsdits.Ravar.Levels 
{
    /// <summary>
    /// Class for holding and providing access to map layers.
    /// </summary>
    public class MapLayers : MonoBehaviour
    {
        /// <summary>
        /// Static instance of the MapLayers.
        /// </summary>
        public static MapLayers Instance { get; private set; }

        [Tooltip("The objects layer holds solid objects that obstruct player movement on the map.")]
        [SerializeField] private LayerMask _objectsLayer;
        [Tooltip("The encounters layer holds terrain such a long grass that can trigger encounters with wild monsters when walked on.")]
        [SerializeField] private LayerMask _encountersLayer;
        [Tooltip("The interact layer provides collider detection for interactable objects. This allows the player to interact with world objects and characters.")]
        [SerializeField] private LayerMask _interactLayer;
        [Tooltip("The player layer holds the player character.")]
        [SerializeField] private LayerMask _playerLayer;
        [Tooltip("The LoS layer provides collision detection with character LoS objects to trigger events between the player and characters.")]
        [SerializeField] private LayerMask _losLayer;
        [Tooltip("The trigger layer holds map objects, both visible and hidden, that trigger events when the player walks over them. Doors for scene switching and quest triggers reside here.")]
        [SerializeField] private LayerMask _triggerLayer;

        /// <summary>
        /// The objects layer holds solid objects that obstruct player movement on the map.
        /// </summary>
        public LayerMask ObjectsLayer => _objectsLayer;
        /// <summary>
        /// The encounters layer holds terrain such a long grass that can trigger encounters with wild monsters when walked on.
        /// </summary>
        public LayerMask EncountersLayer => _encountersLayer;
        /// <summary>
        /// The interact layer provides collider detection for interactable objects.
        /// </summary>
        /// <remarks>This allows the player to interact with world objects and characters.</remarks>
        public LayerMask InteractLayer => _interactLayer;
        /// <summary>
        /// The player layer holds the player character.
        /// </summary>
        public LayerMask PlayerLayer => _playerLayer;
        /// <summary>
        /// The LoS layer provides collision detection with character LoS objects to trigger events between the player and characters.
        /// </summary>
        public LayerMask LosLayer => _losLayer;
        /// <summary>
        /// The trigger layer holds map objects, both visible and hidden, that trigger events when the player walks over them.
        /// </summary>
        /// <remarks>Doors for scene switching and quest triggers reside here.</remarks>
        public LayerMask TriggerLayer => _triggerLayer;
        /// <summary>
        /// The action layers are a group of layers that trigger events when the player interacts with them.
        /// </summary>
        /// <remarks>This group is used for collider detection when the player moves around the world.</remarks>
        public LayerMask ActionLayers => _encountersLayer | _losLayer | _triggerLayer;

        private void Awake()
        {
            Instance = this;
        }
    }
}