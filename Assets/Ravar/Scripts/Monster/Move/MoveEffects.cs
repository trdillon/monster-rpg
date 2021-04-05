using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Monster
{
    /// <summary>
    /// Holds status conditions from <see cref="ConditionObj"/> and <see cref="StatChange"/> effects from <see cref="MoveObj"/>.
    /// </summary>
    [System.Serializable]
    public class MoveEffects
    {
        [Tooltip("List of the stat changes this move can produce. Each stat change has a range of -6 to 6.")]
        [SerializeField] List<StatChange> statChanges;
        [Tooltip("Status condition this move can inflict. Only one status condition can be active on a monster at a time.")]
        [SerializeField] ConditionID status;
        [Tooltip("Volatile status condition this move can inflict. Only one volatile status condition can be active on a monster at a time.")]
        [SerializeField] ConditionID volatileStatus;

        /// <summary>
        /// List of stat changes to the monster's current stats.
        /// </summary>
        public List<StatChange> StatChanges => statChanges;
        /// <summary>
        /// Status condition that has been inflicted.
        /// </summary>
        /// <remarks>Only one status condition can be active on a monster at a time.</remarks>
        public ConditionID Status => status;
        /// <summary>
        /// Volatile status condition that has been inflicted.
        /// </summary>
        /// <remarks>Only one volatile status condition can be active on a monster at a time.</remarks>
        public ConditionID VolatileStatus => volatileStatus;
    }
}