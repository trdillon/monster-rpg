using Itsdits.Ravar.Core.Signal;
using Itsdits.Ravar.Levels;
using UnityEngine;

namespace Itsdits.Ravar.Character
{
    /// <summary>
    /// This class handles logic for triggers on the LoS layer.
    /// </summary>
    public class LoS : MonoBehaviour, ITriggerable
    {
        private BattlerController _battler;

        private void Awake()
        {
            _battler = GetComponentInParent<BattlerController>();
        }

        /// <summary>
        /// What happens when the LoS is triggered.
        /// </summary>
        /// <param name="player">The player that triggered the encounter.</param>
        public void OnTriggered(PlayerController player)
        {
            var encounterItem = new BattlerEncounter(_battler);
            GameSignals.BATTLE_LOS.Dispatch(encounterItem);
        }
    }
}