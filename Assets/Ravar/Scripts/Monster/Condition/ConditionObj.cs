using System;

namespace Itsdits.Ravar.Monster
{
    /// <summary>
    /// Implementation of <see cref="ConditionID"/> as defined in <see cref="ConditionDB"/>.
    /// </summary>
    public class ConditionObj
    {
        /// <summary>
        /// Reference to the Id of the condition.
        /// </summary>
        public ConditionID Id { get; set; }
        /// <summary>
        /// Name of the condition.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of what the condition is or does.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The message to be displayed when the condition is applied.
        /// </summary>
        public string StartMessage { get; set; }
        /// <summary>
        /// What happens when the condition is first applied.
        /// </summary>
        /// <remarks>This is used to set the turn timer for the condition.</remarks>
        public Action<MonsterObj> OnStart { get; set; }
        /// <summary>
        /// What happens when the monster executes a turn.
        /// </summary>
        /// <remarks>This is used to apply the effects of the condition such as not attacking due to paralysis.</remarks>
        public Func<MonsterObj, bool> OnTurnStart { get; set; }
        /// <summary>
        /// What happens after the monster's turn is complete.
        /// </summary>
        /// <remarks>This is used to apply the after effects of the condition such as damage from poison.</remarks>
        public Action<MonsterObj> OnTurnEnd { get; set; }
    }
}