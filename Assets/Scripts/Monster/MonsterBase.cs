using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster/Create a new monster")] 
public class MonsterBase : ScriptableObject
{
    // Monster definition
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] MonsterType primaryType;
    [SerializeField] MonsterType secondaryType;

    // Monster base stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMove> learnableMoves;

    public string Name {
        get { return name; }
    }

    public string Description {
        get { return description; }
    }

    public Sprite FrontSprite {
        get { return frontSprite; }
    }

    public Sprite BackSprite {
        get { return backSprite; }
    }

    public MonsterType PrimaryType {
        get { return primaryType; }
    }

    public MonsterType SecondaryType {
        get { return secondaryType; }
    }

    public int MaxHp {
        get { return maxHp; }
    }
    
    public int Attack {
        get { return attack; }
    }
    
    public int Defense {
        get { return defense; }
    }
    
    public int SpAttack {
        get { return spAttack; }
    }
    
    public int SpDefense {
        get { return spDefense; }
    }
    
    public int Speed {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves {
        get { return learnableMoves; }
    }
}
