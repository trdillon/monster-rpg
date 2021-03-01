using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Monster/Create a new move")]
public class MoveBase : ScriptableObject
{
    // Move definition
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] MonsterType type;

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

    public int Power {
        get { return power; }
    }
    public int Accuracy {
        get { return accuracy; }
    }
    public int Energy {
        get { return energy; }
    }

    public bool IsSpecial {
        get {
            if (type == MonsterType.Air || type == MonsterType.Aqua || type == MonsterType.Earth ||
                type == MonsterType.Ember || type == MonsterType.Force || type == MonsterType.Light ||
                type == MonsterType.Shadow || type == MonsterType.Shock || type == MonsterType.Spirit)
            {
                return true;
            }
            else
            { 
                return false;
            }
        }
    }
}
