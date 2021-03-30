using Itsdits.Ravar.Character;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Itsdits.Ravar.Levels
{
    /// <summary>
    /// This class handles logic for portals in the Trigger layer. Used to control scene transition and player quests.
    /// </summary>
    public class Portal : MonoBehaviour, ITriggerable
    {
        [SerializeField] PortalID portalId;
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;

        private PlayerController player;

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
            GameController.Instance.PauseGame(true);

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            var destination = FindObjectsOfType<Portal>().First(x => x != this && x.portalId == this.portalId);
            player.SetOffsetOnTile(destination.SpawnPoint.position);

            GameController.Instance.PauseGame(false);
            Destroy(gameObject);
        }
    }
}
