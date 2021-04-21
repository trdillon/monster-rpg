using Itsdits.Ravar.Character;
using Itsdits.Ravar.Core;
using System.Collections;
using System.Linq;
using Itsdits.Ravar.Core.Signal;
using UnityEngine;

namespace Itsdits.Ravar.Levels
{
    /// <summary>
    /// Portals are triggerable colliders that control scene transitions and player quests.
    /// </summary>
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
            GameSignals.PORTAL_ENTER.Dispatch(true);
            yield return SceneLoader.Instance.LoadScene(_sceneToLoad);

            //TODO - refactor this to avoid FOOT call
            Portal destination = FindObjectsOfType<Portal>().First(x => x != this && x._portalId == _portalId);
            _player.SetOffsetOnTile(destination._spawnPoint.position);

            GameSignals.PORTAL_EXIT.Dispatch(true);
        }
    }
}