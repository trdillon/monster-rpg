using UnityEngine;

namespace Itsdits.Ravar.Monster
{
    /// <summary>
    /// Implementation class for <see cref="MoveBase"/>.
    /// </summary>
    [System.Serializable]
    public class LearnableMove
    {
        [Tooltip("The base move class for this move.")]
        [SerializeField] MoveBase moveBase;
        [Tooltip("The earliest level at which this move can be learned.")]
        [SerializeField] int levelLearned;

        /// <summary>
        /// The base move class for this move.
        /// </summary>
        public MoveBase Base => moveBase;
        /// <summary>
        /// The earliest level at which this move can be learned.
        /// </summary>
        public int LevelLearned => levelLearned;
    }
}