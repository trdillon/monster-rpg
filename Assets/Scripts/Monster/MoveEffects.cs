using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatChange> statChanges;

    public List<StatChange> StatChanges {
        get { return statChanges; }
    }


}
