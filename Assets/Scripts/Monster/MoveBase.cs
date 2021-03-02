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
    [SerializeField] MoveTarget target;

    // Move stats
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int energy;

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
}
