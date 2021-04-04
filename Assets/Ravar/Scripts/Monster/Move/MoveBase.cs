using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Monster
{
    /// <summary>
    /// Base class for a monster Move.
    /// </summary>
    [CreateAssetMenu(fileName = "Move", menuName = "Monster/Create a new move")]
    public class MoveBase : ScriptableObject
    {
        [SerializeField] string _name;
        [TextArea]
        [SerializeField] string description;
        [SerializeField] int moveNumber;
        [SerializeField] int power;
        [SerializeField] int accuracy;
        [SerializeField] int energy;
        [SerializeField] int priority;
        [SerializeField] bool alwaysHits;
        [SerializeField] MonsterType type;
        [SerializeField] MoveCategory category;
        [SerializeField] MoveTarget target;
        [SerializeField] MoveEffects effects;
        [SerializeField] List<MoveSecondaryEffects> moveSecondaryEffects;

        /// <summary>
        /// Name of the move.
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// Filename of the move used to find this move on Resources.Load calls.
        /// </summary>
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
        /// Power of the move. Range is 0 to 100.
        /// </summary>
        public int Power => power;
        /// <summary>
        /// Accuracy of the move. Range is 0 to 100.
        /// </summary>
        public int Accuracy => accuracy;
        /// <summary>
        /// Energy the move has. One use takes one energy.
        /// </summary>
        public int Energy => energy;
        /// <summary>
        /// Priority rating is used to decide which move is executed first.
        /// </summary>
        public int Priority => priority;
        /// <summary>
        /// If the move AlwaysHits then Accuracy check is skipped.
        /// </summary>
        public bool AlwaysHits => alwaysHits;
        /// <summary>
        /// Type of attack determines strengths and weaknesses per the <see cref="TypeChart"/>.
        /// </summary>
        public MonsterType Type => type;
        /// <summary>
        /// Category of the move. Physical, Special, Status.
        /// </summary>
        public MoveCategory Category => category;
        /// <summary>
        /// Intended target of the move. Enemy or self.
        /// </summary>
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