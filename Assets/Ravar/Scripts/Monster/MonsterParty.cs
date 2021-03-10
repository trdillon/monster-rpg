using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Itsdits.Ravar.Monster { 
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
            return monsters.Where(x => x.CurrentHp > 0).FirstOrDefault();
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