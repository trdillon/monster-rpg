namespace Itsdits.Ravar.Character
{
    /// <summary>
    /// Current state of the NPC. Used to avoid interactions while performing walk pattern.
    /// </summary>
    /// <remarks>Possible states are Idle, Walking and Interacting.</remarks>
    public enum NpcState
    {
        Idle,
        Walking,
        Interacting
    }
}