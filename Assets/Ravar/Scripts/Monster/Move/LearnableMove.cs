using UnityEngine;

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int levelLearned;

    public MoveBase Base => moveBase;
    public int LevelLearned => levelLearned;
}
