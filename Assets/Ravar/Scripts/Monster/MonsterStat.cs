namespace Itsdits.Ravar.Monster 
{
    /// <summary>
    /// Stat types for <see cref="MonsterBase"/>.
    /// </summary>
    public enum MonsterStat
    {
        Attack,
        Defense,
        SpAttack,
        SpDefense,
        Speed,
        // Only used for hit calculation
        Accuracy,
        Evasion
    }
}