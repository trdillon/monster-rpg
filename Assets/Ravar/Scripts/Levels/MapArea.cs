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
        [SerializeField] List<MonsterObj> wildMonsters;

        /// <summary>
        /// Get a random monster from the available pool.
        /// </summary>
        /// <returns>Monster to encounter.</returns>
        public MonsterObj GetRandomMonster()
        {
            var wildMonster = wildMonsters[Random.Range(0, wildMonsters.Count)]; //TODO - refactor this for monster rarity
            wildMonster.Init();
            return wildMonster;
        }
    }
}