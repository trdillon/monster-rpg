namespace Itsdits.Ravar.Battle
{
    /// <summary>
    /// Current state of a battle.
    /// </summary>
    /// <remarks>Possible states are Start, Busy, ActionSelection, 
    /// Move Selection, ForgetSelection, ChoiceSelection,
    /// ExecutingTurn, BattleOver and PartyScreen.</remarks>
    public enum BattleState
    {
        Start,
        Busy,
        ActionSelection,
        MoveSelection,
        ForgetSelection,
        ChoiceSelection,
        ExecutingTurn,
        BattleOver,
        PartyScreen
    }
}
