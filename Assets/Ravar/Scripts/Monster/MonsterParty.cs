using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Itsdits.Ravar.Monster 
{
    /// <summary>
    /// Class that holds a party of <see cref="MonsterObj"/> for a character.
    /// </summary>
    public class MonsterParty : MonoBehaviour
    {
        [SerializeField] List<MonsterObj> monsters;

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
        /// <returns>Next monster to fight with</returns>
        public MonsterObj GetHealthyMonster()
        {
            var healthyMonster = monsters.Where(x => x.CurrentHp > 0).FirstOrDefault();
            if (healthyMonster == null)
            {
                // This doesn't always indicate an error, a Battler with a downed party will also
                // return a null MonsterObj here, which is how we determine the battle is over.
                //Debug.LogError("MP001: GetHealthyMonster returned null MonsterObj.");
            }
            return healthyMonster;
        }

        /// <summary>
        /// Add a new monster to the team.
        /// </summary>
        /// <param name="newMonster">Monster to add</param>
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