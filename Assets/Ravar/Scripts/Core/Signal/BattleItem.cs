using Itsdits.Ravar.Character;

namespace Itsdits.Ravar.Core.Signal
{
    /// <summary>
    /// Struct for a Battle item that contains the <see cref="PlayerController"/> and <see cref="BattlerController"/> of
    /// the characters in the battle.
    /// </summary>
    public readonly struct BattleItem
    {
        public readonly PlayerController Player;
        public readonly BattlerController Battler;

        /// <summary>
        /// Constructor for a Battle Item.
        /// </summary>
        /// <param name="player">Player in the battle.</param>
        /// <param name="battler">Battler character in the battle.</param>
        public BattleItem(PlayerController player, BattlerController battler)
        {
            Player = player;
            Battler = battler;
        }
    }
}