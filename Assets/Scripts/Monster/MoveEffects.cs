using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatChange> statChanges;
    [SerializeField] ConditionID status;

    public List<StatChange> StatChanges {
        get { return statChanges; }
    }

    public ConditionID Status {
        get { return status; }
    }
}
