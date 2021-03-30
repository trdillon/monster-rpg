namespace Itsdits.Ravar.Monster
{
    /// <summary>
    /// Holds details about a damaging attack.
    /// </summary>
    public class DamageDetails
    {
        public bool Downed { get; set; }
        public float Critical { get; set; }
        public float TypeEffectiveness { get; set; }
    }
}