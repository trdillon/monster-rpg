using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatChange> statChanges;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;

    public List<StatChange> StatChanges => statChanges;
    public ConditionID Status => status;
    public ConditionID VolatileStatus => volatileStatus;
}
