using System.Collections;

using UnityEngine;

[System.Serializable]
public class MoveSecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance {
        get { return chance; }
    }

    public MoveTarget Target {
        get { return target; }
    }
}
