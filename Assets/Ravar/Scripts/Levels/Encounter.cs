using Itsdits.Ravar.Character;
using Itsdits.Ravar.Core;
using Itsdits.Ravar.Levels;
using UnityEngine;

namespace Itsdits.Ravar
{
    /// <summary>
    /// This class handles logic for triggers on the Encounter layer.
    /// </summary>
    public class Encounter : MonoBehaviour, ITriggerable
    {
        /// <summary>
        /// What happens when the encounter is triggered.
        /// </summary>
        /// <param name="player">The player that triggered the encounter.</param>
        public void OnTriggered(PlayerController player)
        {
            if (Random.Range(1, 101) <= 7)
            {
                GameController.Instance.StartWildBattle();
            }
        }
    }
}
