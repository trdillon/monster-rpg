using UnityEngine;

namespace Itsdits.Ravar.Monster.Move
{
    /// <summary>
    /// Implementation class for <see cref="MoveBase"/>.
    /// </summary>
    [System.Serializable]
    public class LearnableMove
    {
        [Tooltip("The base move class for this move.")]
        [SerializeField] private MoveBase _moveBase;
        [Tooltip("The earliest level at which this move can be learned.")]
        [SerializeField] private int _levelLearned;

        /// <summary>
        /// The base move class for this move.
        /// </summary>
        public MoveBase Base => _moveBase;
        /// <summary>
        /// The earliest level at which this move can be learned.
        /// </summary>
        public int LevelLearned => _levelLearned;
    }
}