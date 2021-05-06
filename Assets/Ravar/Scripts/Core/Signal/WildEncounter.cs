using Itsdits.Ravar.Monster;

namespace Itsdits.Ravar.Core.Signal
{
    /// <summary>
    /// Struct for a wild encounter item containing the monster that was encountered.
    /// </summary>
    public readonly struct WildEncounter
    {
        public readonly MonsterObj Monster;

        /// <summary>
        /// Constructor for a wild encounter.
        /// </summary>
        /// <param name="monster">Monster that was encountered.</param>
        public WildEncounter(MonsterObj monster)
        {
            Monster = monster;
        }
    }
}