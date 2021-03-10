using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Monster.Move {
    [CreateAssetMenu(fileName = "Move", menuName = "Monster/Create a new move")]
    public class MoveBase : ScriptableObject
    {
        #region config
        [SerializeField] string _name;
        [TextArea]
        [SerializeField] string description;
        [SerializeField] int power;
        [SerializeField] int accuracy;
        [SerializeField] int energy;
        [SerializeField] int priority;
        [SerializeField] bool alwaysHits;
        [SerializeField] MonsterType type;
        [SerializeField] MoveCategory category;
        [SerializeField] MoveTarget target;
        [SerializeField] MoveEffects effects;
        [SerializeField] List<MoveSecondaryEffects> moveSecondaryEffects;
        #endregion
        #region Properties
        public string Name => _name;
        public string Description => description;
        public int Power => power;
        public int Accuracy => accuracy;
        public int Energy => energy;
        public int Priority => priority;
        public bool AlwaysHits => alwaysHits;
        public MonsterType Type => type;
        public MoveCategory Category => category;
        public MoveTarget Target => target;
        public MoveEffects Effects => effects;
        public List<MoveSecondaryEffects> MoveSecondaryEffects => moveSecondaryEffects;
        #endregion
    }
}