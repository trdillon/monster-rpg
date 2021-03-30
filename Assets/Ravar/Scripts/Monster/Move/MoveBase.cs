using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Monster
{
    /// <summary>
    /// Base class for a monster Move.
    /// </summary>
    [CreateAssetMenu(fileName = "Move", menuName = "Monster/Create a new move")]
    public class MoveBase : ScriptableObject
    {
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
    }
}