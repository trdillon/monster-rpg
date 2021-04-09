using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Monster 
{
    /// <summary>
    /// Base class for Monsters. ScriptableObject that is implemented by <see cref="MonsterObj"/> to create monsters in game.
    /// </summary>
    /// <remarks>Contains the static details, base stats, moves that can be learned, type of monster and monster sprites that define a monster.</remarks>
    [CreateAssetMenu(fileName = "Monster", menuName = "Monster/Create a new monster")]
    public class MonsterBase : ScriptableObject
    {
        /// <summary>
        /// The maximum number of moves a monster can have in it's move list.
        /// </summary>
        public static int MaxNumberOfMoves { get; } = 4;

        [Header("Details")]
        [Tooltip("The name of this monster. This name will be displayed in game.")]
        [SerializeField] private string _name;
        [Tooltip("Description of this monster and any story related details.")]
        [TextArea]
        [SerializeField] private string _description;
        [Tooltip("Unique number of this monster base in the monster index.")]
        [SerializeField] private int _monsterNumber;

        [Header("Base Stats")]
        [Tooltip("The base maximum HP stat of this monster. Range is 5 to 250.")]
        [Range(5, 250)]
        [SerializeField] private int _maxHp;
        [Tooltip("The base Attack stat of this monster. Range is 5 to 250.")]
        [Range(5, 250)]
        [SerializeField] private int _attack;
        [Tooltip("The base Defense stat of this monster. Range is 5 to 250.")]
        [Range(5, 250)]
        [SerializeField] private int _defense;
        [Tooltip("The base Special Attack stat of this monster. Range is 5 to 250.")]
        [Range(5, 250)]
        [SerializeField] private int _spAttack;
        [Tooltip("The base Special Defense stat of this monster. Range is 5 to 250.")]
        [Range(5, 250)]
        [SerializeField] private int _spDefense;
        [Tooltip("The base Speed stat of this monster. Range is 5 to 250.")]
        [Range(5, 250)]
        [SerializeField] private int _speed;

        [Header("Special Stats")]
        [Tooltip("The amount of experience this monster gives when it is defeated. Range is 30 to 500.")]
        [Range(30, 500)]
        [SerializeField] private int _expGiven;
        [Tooltip("The difficulty rate of catching this monster. A lower number represents a higher difficulty. Range is 5 to 250.")]
        [Range(5, 250)]
        [SerializeField] private int _catchRate = 250;
        [Tooltip("The relative growth rate of this monster represents how quickly it gains experience and levels up.")]
        [SerializeField] private GrowthRate _growthRate;

        [Header("Types and Moves")]
        [Tooltip("The primary type of this monster. This determines its strengths and weaknesses when attacked, and what types of moves it can learn.")]
        [SerializeField] private MonsterType _primaryType;
        [Tooltip("Optional secondary type of this monster. Secondary types can provide a wider range of learnable moves and affect damage calculations.")]
        [SerializeField] private MonsterType _secondaryType;
        [Tooltip("The moves this monster is capable of learning.")]
        [SerializeField] private List<LearnableMove> _learnableMoves;

        [Header("Sprites")]
        [Tooltip("The sprite of this monster to be displayed on the left, player side of the battle screen.")]
        [SerializeField] private Sprite _leftSprite;
        [Tooltip("The sprite of this monster to be displayed on the right, enemy side of the battle screen.")]
        [SerializeField] private Sprite _rightSprite;

        /// <summary>
        /// The monster's name.
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// Description of the monster.
        /// </summary>
        public string Description => _description;
        /// <summary>
        /// Number of the monster in the monster index.
        /// </summary>
        public int MonsterNumber => _monsterNumber;
        /// <summary>
        /// Max HP base.
        /// </summary>
        /// <remarks>Range is 5 to 250.</remarks>
        public int MaxHp => _maxHp;
        /// <summary>
        /// Attack stat base.
        /// </summary>
        /// <remarks>Range is 5 to 250.</remarks>
        public int Attack => _attack;
        /// <summary>
        /// Defense stat base.
        /// </summary>
        /// <remarks>Range is 5 to 250.</remarks>
        public int Defense => _defense;
        /// <summary>
        /// Special Attack stat base.
        /// </summary>
        /// <remarks>Range is 5 to 250.</remarks>
        public int SpAttack => _spAttack;
        /// <summary>
        /// Special Defense stat base.
        /// </summary>
        /// <remarks>Range is 5 to 250.</remarks>
        public int SpDefense => _spDefense;
        /// <summary>
        /// Speed stat base.
        /// </summary>
        /// <remarks>Range is 5 to 250.</remarks>
        public int Speed => _speed;
        /// <summary>
        /// How much experience this monster gives when defeated.
        /// </summary>
        /// <remarks>Range is 30 to 500.</remarks>
        public int ExpGiven => _expGiven;
        /// <summary>
        /// How difficult this monster is to catch. Lower number is a higher difficulty.
        /// </summary>
        /// <remarks>Range is 5 to 250.</remarks>
        public int CatchRate => _catchRate;
        /// <summary>
        /// The relative rate at which this monster gains experience.
        /// </summary>
        public GrowthRate GrowthRate => _growthRate;
        /// <summary>
        /// The primary <see cref="MonsterType"/> of this monster.
        /// </summary>
        public MonsterType PrimaryType => _primaryType;
        /// <summary>
        /// The optional secondary <see cref="MonsterType"/> of this monster.
        /// </summary>
        public MonsterType SecondaryType => _secondaryType;
        /// <summary>
        /// The moves that this monster is capable of learning.
        /// </summary>
        public List<LearnableMove> LearnableMoves => _learnableMoves;
        /// <summary>
        /// The sprite that appears on the left side in a battle.
        /// </summary>
        /// <remarks>This is for the player's monster.</remarks>
        public Sprite LeftSprite => _leftSprite;
        /// <summary>
        /// The sprite that appears on the right side in a battle.
        /// </summary>
        /// <remarks>This is for the enemy's monster.</remarks>
        public Sprite RightSprite => _rightSprite;

        /// <summary>
        /// Gets the amount of experience required to be at the provided level.
        /// </summary>
        /// <param name="level">Level to get exp requirement for.</param>
        public int GetExpForLevel(int level)
        {
            return _growthRate switch
            {
                GrowthRate.Fast => 4 * (level * level * level) / 5,
                GrowthRate.MediumFast => level * level * level,
                GrowthRate.MediumSlow => 6 * (level * level * level) / 5 - 15 * (level * level) + 100 * level - 140,
                GrowthRate.Slow => 5 * (level * level * level) / 4,
                GrowthRate.Fluctuating => GetFluctuating(level),
                _ => -1
            };
        }

        private int GetFluctuating(int level)
        {
            if (level <= 15)
            {
                return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.Floor((level + 1) / 3) + 24) / 50));
            }

            return level <= 36 ? 
                Mathf.FloorToInt(Mathf.Pow(level, 3) * ((level + 14) / 50)) : 
                Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.Floor(level / 2) + 32) / 50));
        }
    }
}