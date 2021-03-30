using Itsdits.Ravar.Levels;
using UnityEngine;

namespace Itsdits.Ravar.Character
{
    /// <summary>
    /// This class handles logic for triggers on the LoS layer.
    /// </summary>
    public class LoS : MonoBehaviour, ITriggerable
    {
        /// <summary>
        /// What happens when the LoS is triggered.
        /// </summary>
        /// <param name="player">The player that triggered the encounter.</param>
        public void OnTriggered(PlayerController player)
        {
            GameController.Instance.StartCharEncounter(GetComponentInParent<BattlerController>());
        }
    }
}
