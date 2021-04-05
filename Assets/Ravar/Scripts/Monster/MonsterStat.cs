namespace Itsdits.Ravar.Monster 
{
    /// <summary>
    /// Stat types for <see cref="MonsterBase"/>.
    /// </summary>
    /// <remarks>Base stats include Attack, Defense, Special Attack,
    /// Special Defense and Speed. Accuracy and Evasion are dynamically generated
    /// to determine hit chance during battle.</remarks>
    public enum MonsterStat
    {
        Attack,
        Defense,
        SpAttack,
        SpDefense,
        Speed,
        Accuracy,
        Evasion
    }
}