using Itsdits.Ravar.Character;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Itsdits.Ravar.Core
{
    /// <summary>
    /// Controller class for the core game scene. Controls player, camera and event functions.
    /// </summary>
    public class CoreController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Controller object for the player.")]
        [SerializeField] private PlayerController _playerController;
        [Tooltip("Camera attached to the player prefab.")]
        [SerializeField] private Camera _playerCamera;

        [Header("Event System")]
        [Tooltip("Event System for the core scene.")]
        [SerializeField] private EventSystem _eventSystem;

        private void Awake()
        {
            _eventSystem.enabled = false;
            _playerCamera.enabled = false;
            _playerController.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}