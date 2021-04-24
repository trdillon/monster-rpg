using Itsdits.Ravar.Character;

namespace Itsdits.Ravar.Core.Signal
{
    /// <summary>
    /// Struct for a Battler encounter item to be passed into a battle. Holds the BattlerController.
    /// </summary>
    public readonly struct BattlerEncounter
    {
        public readonly BattlerController Battler;

        /// <summary>
        /// Constructor for a Battler encounter item.
        /// </summary>
        /// <param name="battler">BattlerController of the Battler being passed.</param>
        public BattlerEncounter(BattlerController battler)
        {
            Battler = battler;
        }
    }
}