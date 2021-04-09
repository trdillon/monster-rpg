using Itsdits.Ravar.Monster;
using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Levels 
{
    /// <summary>
    /// Class that holds what wild monsters, quests and objects reside in a map area.
    /// </summary>
    public class MapArea : MonoBehaviour
    {
        [Tooltip("List of possible wild monsters that can appear in this MapArea.")]
        [SerializeField]
        private List<MonsterObj> _wildMonsters;

        /// <summary>
        /// Get a random monster from the available pool.
        /// </summary>
        /// <returns>Monster to encounter.</returns>
        public MonsterObj GetRandomMonster()
        {
            //TODO - refactor this for monster rarity
            MonsterObj wildMonster = _wildMonsters[Random.Range(0, _wildMonsters.Count)]; 
            wildMonster.Init();
            return wildMonster;
        }
    }
}