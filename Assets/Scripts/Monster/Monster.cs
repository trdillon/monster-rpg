using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Monster
{
    [SerializeField] MonsterBase _base;
    [SerializeField] int level;
    
    public int CurrentHp { get; set; }
    public bool IsHpChanged { get; set; }
    public List<Move> Moves { get; set; }
    public Dictionary<MonsterStat, int> Stats { get; private set; }
    public Dictionary<MonsterStat, int> StatsChanged { get; private set; }
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public Condition Status { get; private set; }
    public int StatusTimer { get; set; }

    public MonsterBase Base {
        get { return _base; }
    }
    public int Level {
        get { return level; }
    }

    public int MaxHp { get; private set; }

    public int Attack {
        get { return GetStat(MonsterStat.Attack); }
    }

    public int Defense {
        get { return GetStat(MonsterStat.Defense); }
    }

    public int SpAttack {
        get { return GetStat(MonsterStat.SpAttack); }
    }

    public int SpDefense {
        get { return GetStat(MonsterStat.SpDefense); }
    }

    public int Speed {
        get { return GetStat(MonsterStat.Speed); }
    }
    
    public void Init()
    {
        // Generate move list
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves) //TODO - refactor this to account for optional learned moves or forgetting moves
        {
            if (move.LevelLearned <= Level)
            {
                Moves.Add(new Move(move.Base));
            }

            if (Moves.Count >= 4)
                break;
        }

        CalculateStats();
        CurrentHp = MaxHp;
        ResetStatsChanged();
    }

    void CalculateStats()
    {
        Stats = new Dictionary<MonsterStat, int>();
        Stats.Add(MonsterStat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(MonsterStat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(MonsterStat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(MonsterStat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
        Stats.Add(MonsterStat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);
        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10 + Level;
    }

    void ResetStatsChanged()
    {
        StatsChanged = new Dictionary<MonsterStat, int>()
        {
            {MonsterStat.Attack, 0},
            {MonsterStat.Defense, 0},
            {MonsterStat.SpAttack, 0},
            {MonsterStat.SpDefense, 0},
            {MonsterStat.Speed, 0}
        };
    }

    int GetStat(MonsterStat stat)
    {
        int statVal = Stats[stat];

        // Stat changes based on original game's formula
        int changeVal = StatsChanged[stat];
        var changeVals = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (changeVal >= 0)
            statVal = Mathf.FloorToInt(statVal * changeVals[changeVal]);
        else
            statVal = Mathf.FloorToInt(statVal / changeVals[-changeVal]);

        return statVal;
    }

    public void ApplyStatChanges(List<StatChange> statChanges)
    {
        foreach (var statChange in statChanges)
        {
            var stat = statChange.stat;
            var changeVal = statChange.changeVal;

            StatsChanged[stat] = Mathf.Clamp(StatsChanged[stat] + changeVal, -6, 6);

            if (changeVal > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} increased!");
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat} decreased!");


            Debug.Log($"{stat} has been changed by {changeVal}. {stat} is now {GetStat(stat)}");
        }
    }

    public void SetStatus(ConditionID conditionID)
    {
        // Only one status effect at a time
        if (Status != null) return;

        Status = ConditionDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
    }

    public void RemoveStatus()
    {
        Status = null;
    }

    public void UpdateHP(int damage)
    {
        CurrentHp = Mathf.Clamp(CurrentHp - damage, 0, MaxHp);
        IsHpChanged = true;
    }

    public DamageDetails TakeDamage(Move move, Monster attacker)
    {
        // critical hit chance is 6.25%
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;

        // type effectiveness per TypeChart
        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.PrimaryType) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.SecondaryType);

        var damageDetails = new DamageDetails()
        {
            Critical = critical,
            TypeEffectiveness = type,
            Downed = false
        };

        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;

        // damage calculation based on the original monster catching game's formula
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(damage);

        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int i = Random.Range(0, Moves.Count);
        return Moves[i];
    }

    public bool OnTurnBegin()
    {
        if (Status?.OnTurnStart != null)
            return Status.OnTurnStart(this);
        else
            return true;
    }

    public void OnTurnOver()
    {
        Status?.OnTurnEnd?.Invoke(this);
    }

    public void OnBattleOver()
    {
        ResetStatsChanged();
    }
}
