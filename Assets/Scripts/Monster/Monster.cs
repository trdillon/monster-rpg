using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    public MonsterBase Base { get; set; }
    public int Level { get; set; }
    public int CurrentHp { get; set; }
    public List<Move> Moves { get; set; }

    public Monster(MonsterBase mBase, int mLvl)
    {
        Base = mBase;
        Level = mLvl;
        CurrentHp = MaxHp;

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
    }

    public int MaxHp
    {
        get { return Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10; }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5; }
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5; }
    }

    public int SpDefense
    {
        get { return Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; }
    }

    public bool TakeDamage(Move move, Monster attacker)
    {
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float) attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            return true;
        }
        return false;
    }

    public Move GetRandomMove()
    {
        int i = Random.Range(0, Moves.Count);
        return Moves[i];
    }
}
