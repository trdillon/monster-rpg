using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Monster 
{
    /// <summary>
    /// Base class for Monsters.
    /// </summary>
    [CreateAssetMenu(fileName = "Monster", menuName = "Monster/Create a new monster")]
    public class MonsterBase : ScriptableObject
    {
        public static int MaxNumberOfMoves { get; set; } = 4;

        [Header("Details")]
        [SerializeField] string _name;
        [TextArea]
        [SerializeField] string description;

        [Header("Base Stats")]
        [SerializeField] int maxHp;
        [SerializeField] int attack;
        [SerializeField] int defense;
        [SerializeField] int spAttack;
        [SerializeField] int spDefense;
        [SerializeField] int speed;

        [Header("Special Stats")]
        [SerializeField] int expGiven;
        [SerializeField] int catchRate = 255;
        [SerializeField] GrowthRate growthRate;

        [Header("Types and Moves")]
        [SerializeField] MonsterType primaryType;
        [SerializeField] MonsterType secondaryType;
        [SerializeField] List<LearnableMove> learnableMoves;

        [Header("Sprites")]
        [SerializeField] Sprite leftSprite;
        [SerializeField] Sprite rightSprite;

        // Details
        public string Name => _name;
        public string Description => description;

        // Base Stats
        public int MaxHp => maxHp;
        public int Attack => attack;
        public int Defense => defense;
        public int SpAttack => spAttack;
        public int SpDefense => spDefense;
        public int Speed => speed;

        // Special Stats
        public int ExpGiven => expGiven;
        public int CatchRate => catchRate;
        public GrowthRate GrowthRate => growthRate;

        // Types and Moves
        public MonsterType PrimaryType => primaryType;
        public MonsterType SecondaryType => secondaryType;
        public List<LearnableMove> LearnableMoves => learnableMoves;

        // Sprites
        public Sprite LeftSprite => leftSprite;
        public Sprite RightSprite => rightSprite;

        /// <summary>
        /// Gets the amount of experience required to be at the provided level.
        /// </summary>
        /// <param name="level">Level to get exp requirement for.</param>
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

            Debug.LogError($"MB001: {Name} missing GrowthRate.");
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