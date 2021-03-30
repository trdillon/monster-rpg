namespace Itsdits.Ravar.Monster
{
    /// <summary>
    /// Stat changes caused by <see cref="MoveEffects"/>. Range is -6 to 6.
    /// </summary>
    [System.Serializable]
    public class StatChange
    {
        public MonsterStat stat;
        public int changeVal;
    }
}