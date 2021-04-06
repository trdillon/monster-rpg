using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Monster
{
    /// <summary>
    /// Base class for a monster Move. ScriptableObject that is implemented by <see cref="MoveObj"/> to create moves in game.
    /// </summary>
    /// <remarks>Contains the static details, base stats, move type and effects that define a move.</remarks>
    [CreateAssetMenu(fileName = "Move", menuName = "Monster/Create a new move")]
    public class MoveBase : ScriptableObject
    {
        [Header("Details")]
        [Tooltip("The name of this move. This is the name displayed in game and may contain spaces.")]
        [SerializeField] string _name;
        [Tooltip("Description of this move and what it does when used.")]
        [TextArea]
        [SerializeField] string description;
        [Tooltip("The unique number of this move base in the move index.")]
        [SerializeField] int moveNumber;

        [Header("Base Stats")]
        [Tooltip("The base power of this move. Power determines how much damage a move will cause. Range is 0 to 100.")]
        [Range(0, 100)]
        [SerializeField] int power;
        [Tooltip("The base accuracy of this move. Accuracy determines the chance of the move to hit its intended target. Range is 10 to 100.")]
        [Range(10, 100)]
        [SerializeField] int accuracy;
        [Tooltip("The maximum energy of this move. Energy determines how many times a move may be used before needing to recharge. One move use cost one energy. Range is 1 to 50.")]
        [Range(1, 50)]
        [SerializeField] int energy;

        [Header("Special Stats")]
        [Tooltip("The priority of this move. Higher priority moves will be used first in a turn during battle. Range is 0 to 5.")]
        [Range(0, 5)]
        [SerializeField] int priority;
        [Tooltip("This determines if a move will always hit. If this is set to true, accuracy and evasion checks will be skipped when the move is used.")]
        [SerializeField] bool alwaysHits;

        [Header("Types and Effects")]
        [Tooltip("The type of this move. The type will determine the strength and weakness multipliers during damage calculation.")]
        [SerializeField] MonsterType type;
        [Tooltip("The category of this move. Physical moves calculate damage based on Attack and Defense, Special moves calculate damage based on Special Attack and Special Defense, Status moves inflict status conditions.")]
        [SerializeField] MoveCategory category;
        [Tooltip("The intended target of this move. Self target moves are used for stat increasing moves.")]
        [SerializeField] MoveTarget target;
        [Tooltip("Optional status effects inflicted by this move.")]
        [SerializeField] MoveEffects effects;
        [Tooltip("Optional secondary effects that have a chance to be inflicted after a move is used on a monster.")]
        [SerializeField] List<MoveSecondaryEffects> moveSecondaryEffects;

        /// <summary>
        /// Name of the move.
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// Filename of the move.
        /// </summary>
        /// <remarks>Used to find this move on Resources.Load calls.</remarks>
        public string MoveName => name;
        /// <summary>
        /// Description of what the move does.
        /// </summary>
        public string Description => description;
        /// <summary>
        /// Number of the move in the move index.
        /// </summary>
        public int MoveNumber => moveNumber;
        /// <summary>
        /// Power of the move.
        /// </summary>
        /// <remarks>Range is 0 to 100.</remarks>
        public int Power => power;
        /// <summary>
        /// Accuracy of the move.
        /// </summary>
        /// <remarks>Range is 10 to 100.</remarks>
        public int Accuracy => accuracy;
        /// <summary>
        /// Maximum amount of energy the move has.
        /// </summary>
        /// <remarks>One move use takes one energy. Range is 1 to 50.</remarks>
        public int Energy => energy;
        /// <summary>
        /// Priority rating of the move.
        /// </summary>
        /// <remarks>Used to decide which move is executed first.</remarks>
        public int Priority => priority;
        /// <summary>
        /// If the move AlwaysHits then Accuracy check is skipped.
        /// </summary>
        public bool AlwaysHits => alwaysHits;
        /// <summary>
        /// Type of attack determines strengths and weaknesses.
        /// </summary>
        /// <remarks>Details found in the <seealso cref="TypeChart"/>.</remarks>
        public MonsterType Type => type;
        /// <summary>
        /// Category of the move.
        /// </summary>
        /// <remarks>Physical, Special, Status.</remarks>
        public MoveCategory Category => category;
        /// <summary>
        /// Intended target of the move.
        /// </summary>
        /// <remarks>Enemy or self.</remarks>
        public MoveTarget Target => target;
        /// <summary>
        /// Status effects and stat changes that can be inflicted by the mvoe.
        /// </summary>
        public MoveEffects Effects => effects;
        /// <summary>
        /// Secondary effects that have a chance to be inflicted after using the move.
        /// </summary>
        public List<MoveSecondaryEffects> MoveSecondaryEffects => moveSecondaryEffects;
    }
}