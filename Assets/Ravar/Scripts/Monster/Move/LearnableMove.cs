using UnityEngine;

namespace Itsdits.Ravar.Monster
{
    /// <summary>
    /// Implementation class for <see cref="MoveBase"/>.
    /// </summary>
    [System.Serializable]
    public class LearnableMove
    {
        [SerializeField] MoveBase moveBase;
        [SerializeField] int levelLearned;

        public MoveBase Base => moveBase;
        public int LevelLearned => levelLearned;
    }
}