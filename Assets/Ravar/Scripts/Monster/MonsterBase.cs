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
        [SerializeField] int monsterNumber;

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
        /// <summary>
        /// The monster's name.
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// Description of the monster.
        /// </summary>
        public string Description => description;
        /// <summary>
        /// Number of the monster in the monster index.
        /// </summary>
        public int MonsterNumber => monsterNumber;

        // Base Stats
        /// <summary>
        /// Max HP base.
        /// </summary>
        public int MaxHp => maxHp;
        /// <summary>
        /// Attack stat base.
        /// </summary>
        public int Attack => attack;
        /// <summary>
        /// Defense stat base.
        /// </summary>
        public int Defense => defense;
        /// <summary>
        /// Special Attack stat base.
        /// </summary>
        public int SpAttack => spAttack;
        /// <summary>
        /// Special Defense stat base.
        /// </summary>
        public int SpDefense => spDefense;
        /// <summary>
        /// Speed stat base.
        /// </summary>
        public int Speed => speed;

        // Special Stats
        /// <summary>
        /// How much experience this monster gives when defeated.
        /// </summary>
        public int ExpGiven => expGiven;
        /// <summary>
        /// How difficult this monster is to catch. Lower number is a higher difficulty.
        /// </summary>
        public int CatchRate => catchRate;
        /// <summary>
        /// The relative rate at which this monster gains experience.
        /// </summary>
        public GrowthRate GrowthRate => growthRate;

        // Types and Moves
        /// <summary>
        /// The primary <see cref="MonsterType"/> of this monster.
        /// </summary>
        public MonsterType PrimaryType => primaryType;
        /// <summary>
        /// The optional secondary <see cref="MonsterType"/> of this monster.
        /// </summary>
        public MonsterType SecondaryType => secondaryType;
        /// <summary>
        /// The moves that this monster is capable of learning.
        /// </summary>
        public List<LearnableMove> LearnableMoves => learnableMoves;

        // Sprites
        /// <summary>
        /// The sprite that appears on the left, player side, in a battle.
        /// </summary>
        public Sprite LeftSprite => leftSprite;
        /// <summary>
        /// The sprite that appears on the right, enemy side, in a battle.
        /// </summary>
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