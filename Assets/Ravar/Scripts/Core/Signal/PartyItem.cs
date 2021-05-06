using Itsdits.Ravar.Monster;

namespace Itsdits.Ravar.Core.Signal
{
    /// <summary>
    /// Struct for a monster party item containing the <see cref="MonsterParty"/> to be passed.
    /// </summary>
    public readonly struct PartyItem
    {
        public readonly MonsterParty Party;

        /// <summary>
        /// Constructor for a PartyItem.
        /// </summary>
        /// <param name="party">MonsterParty to be passed.</param>
        public PartyItem(MonsterParty party)
        {
            Party = party;
        }
    }
}