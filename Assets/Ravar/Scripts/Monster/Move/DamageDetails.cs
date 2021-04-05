namespace Itsdits.Ravar.Monster
{
    /// <summary>
    /// Holds details about a damaging attack.
    /// </summary>
    /// <remarks>Details include whether or not the attack has downed the target monster, 
    /// the critical hit chance, and the type effectiveness multiplier.</remarks>
    public class DamageDetails
    {
        /// <summary>
        /// If the monster has been downed or not.
        /// </summary>
        public bool Downed { get; set; }
        /// <summary>
        /// The critical hit chance of the attack.
        /// </summary>
        public float Critical { get; set; }
        /// <summary>
        /// The type effectiveness multiplier as determined by the <see cref="TypeChart"/>.
        /// </summary>
        public float TypeEffectiveness { get; set; }
    }
}