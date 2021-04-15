using Itsdits.Ravar.Character;
using Itsdits.Ravar.Core;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Itsdits.Ravar.Levels
{
    /// <summary>
    /// This class handles logic for portals in the Trigger layer.
    /// </summary>
    /// <remarks>Used to control scene transition and player quests.</remarks>
    public class Portal : MonoBehaviour, ITriggerable
    {
        [Tooltip("The ID for this portal. Used to match portals in different scenes to ensure the player is sent to the correct destination.")]
        [SerializeField] private PortalID _portalId;
        [Tooltip("Index of the scene to load when the portal is triggered. Number is determined by the build index.")]
        [SerializeField] private string _sceneToLoad;
        [Tooltip("The spawn point the player should be placed at when the portal is used.")]
        [SerializeField] private Transform _spawnPoint;

        private PlayerController _player;

        /// <summary>
        /// The spawn point the player should be placed at when the portal is used.
        /// </summary>
        public Transform SpawnPoint => _spawnPoint;

        /// <summary>
        /// What happens when the portal is triggered.
        /// </summary>
        /// <param name="player">The player that triggered the portal.</param>
        public void OnTriggered(PlayerController player)
        {
            _player = player;
            StartCoroutine(SwitchScene());
        }

        private IEnumerator SwitchScene()
        {
            DontDestroyOnLoad(gameObject);
            GameController.Instance.FreezePlayer(true);

            yield return GameController.Instance.LoadScene(_sceneToLoad);

            //TODO - refactor this to avoid FOOT call
            Portal destination = FindObjectsOfType<Portal>().First(x => x != this && x._portalId == _portalId);
            _player.SetOffsetOnTile(destination.SpawnPoint.position);

            GameController.Instance.FreezePlayer(false);
            Destroy(gameObject);
        }
    }
}
