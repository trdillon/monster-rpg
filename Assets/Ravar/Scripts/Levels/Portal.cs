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
        [SerializeField] PortalID portalId;
        [Tooltip("Index of the scene to load when the portal is triggered. Number is determined by the build index.")]
        [SerializeField] int sceneToLoad = -1;
        [Tooltip("The spawn point the player should be placed at when the portal is used.")]
        [SerializeField] Transform spawnPoint;

        private PlayerController player;

        /// <summary>
        /// The spawn point the player should be placed at when the portal is used.
        /// </summary>
        public Transform SpawnPoint => spawnPoint;

        /// <summary>
        /// What happens when the portal is triggered.
        /// </summary>
        /// <param name="player">The player that triggered the portal.</param>
        public void OnTriggered(PlayerController player)
        {
            this.player = player;
            StartCoroutine(SwitchScene());
        }

        private IEnumerator SwitchScene() 
        {
            DontDestroyOnLoad(gameObject);
            GameController.Instance.FreezePlayer(true);

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            GameController.Instance.UpdateCurrentScene();

            var destination = FindObjectsOfType<Portal>().First(x => x != this && x.portalId == this.portalId);
            player.SetOffsetOnTile(destination.SpawnPoint.position);

            GameController.Instance.FreezePlayer(false);
            Destroy(gameObject);
        }
    }
}
