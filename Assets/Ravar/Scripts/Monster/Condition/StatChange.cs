namespace Itsdits.Ravar.Monster
{
    /// <summary>
    /// Stat changes caused by <see cref="MoveEffects"/>.
    /// </summary>
    /// <remarks>Each monster has a stat change range of -6 to 6 for each individual stat.
    /// A single stat change can have a range of -6 to 6 but cannot exceed the total stat change.
    /// A typical individual stat change will range from -2 to 2.</remarks>
    [System.Serializable]
    public class StatChange
    {
        /// <summary>
        /// The stat that is affected by this change.
        /// </summary>
        public MonsterStat stat;
        /// <summary>
        /// The value of this stat change.
        /// </summary>
        public int changeVal;
    }
}