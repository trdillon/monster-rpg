using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDB
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
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
        }
    };
}
