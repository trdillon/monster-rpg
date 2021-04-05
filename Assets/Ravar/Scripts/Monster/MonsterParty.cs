using Itsdits.Ravar.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Itsdits.Ravar.Monster 
{
    /// <summary>
    /// Holds a party of <see cref="MonsterObj"/> for a character.
    /// </summary>
    /// <remarks>Max size of the party is 6.</remarks>
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
        /// Add a new monster to the team.
        /// </summary>
        /// <remarks>If the party is full the monster is added to the bank.</remarks>
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

        /// <summary>
        /// Saves a list of <see cref="MonsterData"/> about the monsters in the party.
        /// </summary>
        /// <returns>List of MonsterData about the party.</returns>
        public List<MonsterData> SaveMonsterParty()
        {
            var partyData = new List<MonsterData>();

            foreach (var monster in monsters)
            {
                var monsterData = monster.SaveMonsterData();
                partyData.Add(monsterData);
            }

            return partyData;
        }

        /// <summary>
        /// Loads a list of <see cref="MonsterData"/> into the party as new <see cref="MonsterObj"/>.
        /// </summary>
        /// <param name="newMonsters">New monsters to load into the party.</param>
        public void LoadMonsterParty(List<MonsterData> newMonsters)
        {
            monsters.Clear();
            foreach (var monster in newMonsters)
            {
                var monsterData = new MonsterObj(monster);
                monsters.Add(monsterData);
            }
        }
    }
}