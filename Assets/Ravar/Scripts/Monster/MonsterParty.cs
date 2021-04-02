using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Itsdits.Ravar.Monster 
{
    /// <summary>
    /// Holds a party of <see cref="MonsterObj"/> for a character. Max size of the party is 6.
    /// </summary>
    public class MonsterParty : MonoBehaviour
    {
        [Tooltip("Party of monsters that travel with the player. The maximum size is 6 monsters.")]
        [SerializeField] List<MonsterObj> monsters;

        /// <summary>
        /// Returns a List of type MonsterObj that are in the party.
        /// </summary>
        public List<MonsterObj> Monsters => monsters;

        private void Start()
        {
            foreach (var monster in monsters)
            {
                monster.Init();
            }
        }

        /// <summary>
        /// Get the next healthy monster in the party.
        /// </summary>
        /// <returns>Next monster in the list of monsters.</returns>
        public MonsterObj GetHealthyMonster()
        {
            var healthyMonster = monsters.Where(x => x.CurrentHp > 0).FirstOrDefault();
            return healthyMonster;
        }

        /// <summary>
        /// Add a new monster to the team. If the party is full the monster is added to the bank.
        /// </summary>
        /// <param name="newMonster">Monster to add to the list.</param>
        public void AddMonster(MonsterObj newMonster)
        {
            if (monsters.Count < 6)
            {
                monsters.Add(newMonster);
            }
            else
            {
                //TODO - send to storage bank
            }
        }
    }
}