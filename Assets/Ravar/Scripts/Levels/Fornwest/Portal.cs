using Itsdits.Ravar.Character;
using UnityEngine;

namespace Itsdits.Ravar.Levels
{
    /// <summary>
    /// This class handles logic for portals in the Trigger layer. Used to control scene transition and player quests.
    /// </summary>
    public class Portal : MonoBehaviour, ITriggerable
    {
        /// <summary>
        /// What happens when the portal is triggered.
        /// </summary>
        /// <param name="player">The player that triggered the portal.</param>
        public void OnTriggered(PlayerController player)
        {
            Debug.Log("Portal triggered.");
        }
    }
}
