using UnityEngine;

namespace Itsdits.Ravar.Monster
{ 
    /// <summary>
    /// Handles secondary effects that have a chance to trigger after a <see cref="MoveObj"/>.
    /// </summary>
    [System.Serializable]
    public class MoveSecondaryEffects : MoveEffects
    {
        [SerializeField] int chance;
        [SerializeField] MoveTarget target;

        public int Chance => chance;
        public MoveTarget Target => target;
    }
}