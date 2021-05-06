using Itsdits.Ravar.Character;
using Itsdits.Ravar.Monster;

namespace Itsdits.Ravar.Core.Signal
{
    /// <summary>
    /// Struct for a wild encounter item containing the monster encountered and the player.
    /// </summary>
    public readonly struct EncounterItem
    {
        public readonly MonsterObj Monster;
        public readonly PlayerController Player;

        /// <summary>
        /// Constructor for an encounter item.
        /// </summary>
        /// <param name="monster">Wild monster that was encountered.</param>
        /// <param name="player">The player.</param>
        public EncounterItem(MonsterObj monster, PlayerController player)
        {
            Monster = monster;
            Player = player;
        }
    }
}