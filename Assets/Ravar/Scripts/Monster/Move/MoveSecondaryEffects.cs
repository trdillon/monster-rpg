using UnityEngine;

namespace Itsdits.Ravar.Monster
{ 
    /// <summary>
    /// Holds secondary effects that have a chance to trigger after a <see cref="MoveObj"/>.
    /// </summary>
    [System.Serializable]
    public class MoveSecondaryEffects : MoveEffects
    {
        [Tooltip("The chance this effect will be inflicted after a move is used.")]
        [SerializeField] private int _chance;
        [Tooltip("The intended target of this effect.")]
        [SerializeField] private MoveTarget _target;

        /// <summary>
        /// Chance of this effect being inflicted after a move is used.
        /// </summary>
        public int Chance => _chance;
        /// <summary>
        /// Intended target of this effect.
        /// </summary>
        public MoveTarget Target => _target;
    }
}