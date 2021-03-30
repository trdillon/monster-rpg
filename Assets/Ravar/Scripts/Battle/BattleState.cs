namespace Itsdits.Ravar.Battle
{
    /// <summary>
    /// Current state of a battle.
    /// </summary>
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
