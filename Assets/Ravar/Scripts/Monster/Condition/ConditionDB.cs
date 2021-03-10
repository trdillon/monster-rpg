using System.Collections.Generic;
using UnityEngine;

public class ConditionDB
{
    public static void Init()
    {
        foreach (var entry in Conditions)
        {
            var conditionId = entry.Key;
            var condition = entry.Value;
            condition.Id = conditionId;
        }
    }

    public static float GetStatusBonus(Condition condition)
    {
        if (condition == null)
            return 1f;
        else if (condition.Id == ConditionID.FRZ || condition.Id == ConditionID.SLP)
            return 2f;
        else if (condition.Id == ConditionID.BRN || condition.Id == ConditionID.PAR || condition.Id == ConditionID.PSN)
            return 1.5f;
        else
            return 1f;
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        // Status conditions
        {
            ConditionID.PSN,
            new Condition()
            {
                Name = "Poison",
                StartMessage = "has been infected with poison!",
                OnTurnEnd = (Monster monster) =>
                {
                    monster.UpdateHP(monster.MaxHp / 8);
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} is suffering from the effects of the poison!");
                }
            }
        },
        {
            ConditionID.BRN,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "has been burned badly!",
                OnTurnEnd = (Monster monster) =>
                {
                    monster.UpdateHP(monster.MaxHp / 16);
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} is suffering from the effects of the burn!");
                }
            }
        },
        {
            ConditionID.SLP,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = "has been put to sleep!",
                OnStart = (Monster monster) =>
                {
                    monster.StatusTimer = Random.Range(1, 4);
                    Debug.Log($"Sleep for {monster.StatusTimer} turns.");
                },
                OnTurnStart = (Monster monster) =>
                {
                    if (monster.StatusTimer <= 0)
                    {
                        monster.RemoveStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} has woken up from it's sleep!");
                        return true;
                    }

                    monster.StatusTimer--;
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} is sleeping and it can't attack!");
                    return false;
                }
            }
        },
        {
            ConditionID.PAR,
            new Condition()
            {
                Name = "Paralyze",
                StartMessage = "has been paralyzed by that attack!",
                OnTurnStart = (Monster monster) =>
                {
                    if (Random.Range(0, 5) == 1)
                    {
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} is suffering from the effects of the paralysis! It can't attack!");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.FRZ,
            new Condition()
            {
                Name = "Freeze",
                StartMessage = "has been frozen by that attack!",
                OnTurnStart = (Monster monster) =>
                {
                    if (Random.Range(0, 5) == 1)
                    {
                        monster.RemoveStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} has thawed, it's no longer frozen!");
                        return true;
                    }
                    return false;
                }
            }
        },
        // Volatile status conditions
        {
            ConditionID.CNF,
            new Condition()
            {
                Name = "Confusion",
                StartMessage = "has been confused and doesn't know what to do!",
                OnStart = (Monster monster) =>
                {
                    monster.VolatileStatusTimer = Random.Range(1, 5);
                    Debug.Log($"Confused for {monster.VolatileStatusTimer} turns.");
                },
                OnTurnStart = (Monster monster) =>
                {
                    if (monster.VolatileStatusTimer <= 0)
                    {
                        monster.RemoveVolatileStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} has snapped out of it's confusion!");
                        return true;
                    }

                    monster.VolatileStatusTimer--;
                    
                    // 50/50 chance attack hurts self
                    if (Random.Range(1, 3) == 1)
                        return true;

                    // Attack will hurt self
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} is suffering from the effects of the confusion! It attacked itself!");
                    monster.UpdateHP(monster.MaxHp / 8);
                    return false;
                }
            }
        }
    };
}
