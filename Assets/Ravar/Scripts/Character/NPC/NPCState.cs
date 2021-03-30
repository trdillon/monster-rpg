namespace Itsdits.Ravar.Character
{
    /// <summary>
    /// Current state of the NPC. Used to avoid interactions while performing walk pattern.
    /// </summary>
    public enum NPCState
    {
        Idle,
        Walking,
        Interacting
    }
}