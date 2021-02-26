using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    MonsterBase monsterBase;
    int level;

    public int CurrentHp { get; set; }

    public List<Move> Moves { get; set; }

    public Monster(MonsterBase mBase, int mLvl)
    {
        monsterBase = mBase;
        level = mLvl;
        CurrentHp = monsterBase.MaxHp;

        // Generate move list
        Moves = new List<Move>();
        foreach (var move in monsterBase.LearnableMoves) //TODO - refactor this to account for optional learned moves or forgetting moves
        {
            if (move.LevelLearned <= level)
            {
                Moves.Add(new Move(move.Base));
            }

            if (Moves.Count >= 4)
                break;
        }
    }

    public int MaxHp
    {
        get { return Mathf.FloorToInt((monsterBase.MaxHp * level) / 100f) + 10; }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((monsterBase.Attack * level) / 100f) + 5; }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((monsterBase.Defense * level) / 100f) + 5; }
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt((monsterBase.SpAttack * level) / 100f) + 5; }
    }

    public int SpDefense
    {
        get { return Mathf.FloorToInt((monsterBase.SpDefense * level) / 100f) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((monsterBase.Speed * level) / 100f) + 5; }
    }
}
