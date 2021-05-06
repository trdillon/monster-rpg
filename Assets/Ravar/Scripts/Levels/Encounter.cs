using Itsdits.Ravar.Character;
using Itsdits.Ravar.Core.Signal;
using Itsdits.Ravar.Monster;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Itsdits.Ravar.Levels
{
    /// <summary>
    /// Handles logic for wild monster encounters. <seealso cref="ITriggerable"/>
    /// </summary>
    public class Encounter : MonoBehaviour, ITriggerable
    {
        private MapArea _map;

        private void Awake()
        {
            _map = GetComponentInParent<MapArea>();
        }

        /// <summary>
        /// What happens when the encounter is triggered.
        /// </summary>
        /// <param name="player">The player that triggered the encounter.</param>
        public void OnTriggered(PlayerController player)
        {
            if (Random.Range(1, 101) > 7)
            {
                return;
            }

            MonsterObj monster = _map.GetRandomMonster();
            GameSignals.WILD_ENCOUNTER.Dispatch(new WildEncounter(monster));
        }
    }
}