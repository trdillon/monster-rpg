using Itsdits.Ravar.Monster.Move;
using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Monster {
    [CreateAssetMenu(fileName = "Monster", menuName = "Monster/Create a new monster")]
    public class MonsterBase : ScriptableObject
    {
        #region config
        [SerializeField] string _name;
        [TextArea]
        [SerializeField] string description;
        [SerializeField] int maxHp;
        [SerializeField] int attack;
        [SerializeField] int defense;
        [SerializeField] int spAttack;
        [SerializeField] int spDefense;
        [SerializeField] int speed;
        [SerializeField] int expGiven;
        [SerializeField] int catchRate = 255;
        [SerializeField] Sprite leftSprite;
        [SerializeField] Sprite rightSprite;
        [SerializeField] MonsterType primaryType;
        [SerializeField] MonsterType secondaryType;
        [SerializeField] GrowthRate growthRate;
        [SerializeField] List<LearnableMove> learnableMoves;
        #endregion
        #region Properties
        public string Name => _name;
        public string Description => description;
        public int MaxHp => maxHp;
        public int Attack => attack;
        public int Defense => defense;
        public int SpAttack => spAttack;
        public int SpDefense => spDefense;
        public int Speed => speed;
        public int ExpGiven => expGiven;
        public int CatchRate => catchRate;
        public Sprite LeftSprite => leftSprite;
        public Sprite RightSprite => rightSprite;
        public MonsterType PrimaryType => primaryType;
        public MonsterType SecondaryType => secondaryType;
        public GrowthRate GrowthRate => growthRate;
        public List<LearnableMove> LearnableMoves => learnableMoves;
        #endregion

        /// <summary>
        /// Gets the exp required to be at the current level, used for Init function.
        /// </summary>
        /// <param name="level">Level of the monster</param>
        /// <returns>Required exp</returns>
        public int GetExpForLevel(int level)
        {
            if (growthRate == GrowthRate.Fast)
            {
                return 4 * (level * level * level) / 5;
            }
            else if (growthRate == GrowthRate.MediumFast)
            {
                return level * level * level;
            }
            else if (growthRate == GrowthRate.MediumSlow)
            {
                return 6 * (level * level * level) / 5 - 15 * (level * level) + 100 * level - 140;
            }
            else if (growthRate == GrowthRate.Slow)
            {
                return 5 * (level * level * level) / 4;
            }
            else if (growthRate == GrowthRate.Fluctuating)
            {
                return GetFluctuating(level);
            }

            return -1;
        }

        private int GetFluctuating(int level)
        {
            if (level <= 15)
            {
                return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.Floor((level + 1) / 3) + 24) / 50));
            }
            else if (level >= 15 && level <= 36)
            {
                return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((level + 14) / 50));
            }
            else
            {
                return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.Floor(level / 2) + 32) / 50));
            }
        }
    }
}