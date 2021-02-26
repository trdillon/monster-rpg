using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int levelLearned;

    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int LevelLearned
    {
        get { return levelLearned; }
    }
}
