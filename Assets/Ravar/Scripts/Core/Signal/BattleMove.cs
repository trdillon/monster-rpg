using System;
using Itsdits.Ravar.Battle;

namespace Itsdits.Ravar.Core.Signal
{
    /// <summary>
    /// Struct for a Battle Move that contains the <see cref="BattleState"/> and number of the Move selected.
    /// </summary>
    /// <remarks>BattleState should be either MoveSelection or ForgetSelection.</remarks>
    public readonly struct BattleMove
    {
        public readonly BattleState State;
        public readonly int MoveNumber;

        /// <summary>
        /// Constructor for a BattleMove item.
        /// </summary>
        /// <param name="state">BattleState during move selection. MoveSelection or ForgetSelection only.</param>
        /// <param name="moveNumber">Number of the move selected.</param>
        public BattleMove(BattleState state, int moveNumber)
        {
            if (state != BattleState.MoveSelection && state != BattleState.ForgetSelection)
            {
                throw new ArgumentException("BattleMove constructor passed invalid BattleState.");
            }
            
            State = state;
            MoveNumber = moveNumber;
        }
    }
}