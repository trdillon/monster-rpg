using UnityEngine;

namespace Itsdits.Ravar.Monster.Move { 
    [System.Serializable]
    public class MoveSecondaryEffects : MoveEffects
    {
        [SerializeField] int chance;
        [SerializeField] MoveTarget target;

        public int Chance => chance;
        public MoveTarget Target => target;
    }
}