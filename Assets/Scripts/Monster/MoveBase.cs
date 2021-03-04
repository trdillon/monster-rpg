using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Monster/Create a new move")]
public class MoveBase : ScriptableObject
{
    // Move definition
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] MonsterType type;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<MoveSecondaryEffects> moveSecondaryEffects;
    [SerializeField] MoveTarget target;

    // Move stats
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int energy;
    [SerializeField] bool alwaysHits;

    public string Name {
        get { return name; }
    }

    public string Description {
        get { return description; }
    }

    public MonsterType Type {
        get { return type; }
    }

    public MoveCategory Category {
        get { return category; }
    }

    public MoveEffects Effects {
        get { return effects; }
    }

    public List<MoveSecondaryEffects> MoveSecondaryEffects {
        get { return moveSecondaryEffects; }
    }

    public MoveTarget Target {
        get { return target; }
    }

    public int Power {
        get { return power; }
    }
    public int Accuracy {
        get { return accuracy; }
    }
    public int Energy {
        get { return energy; }
    }

    public bool AlwaysHits {
        get { return alwaysHits; }
    }
}
