using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Monster.Condition { 
    public class ConditionDB
    {
        /// <summary>
        /// Initialize the DB.
        /// </summary>
        public static void Init()
        {
            foreach (var entry in Conditions)
            {
                var conditionId = entry.Key;
                var condition = entry.Value;
                condition.Id = conditionId;
            }
        }

        /// <summary>
        /// Get status bonuses for capture algo.
        /// </summary>
        /// <param name="condition">Condition monster is affected by</param>
        /// <returns>Boost for capture algo</returns>
        public static float GetStatusBonus(ConditionObj condition)
        {
            if (condition == null)
            {
                return 1f;
            }
            else if (condition.Id == ConditionID.FRZ || condition.Id == ConditionID.SLP)
            {
                return 2f;
            }
            else if (condition.Id == ConditionID.BRN || condition.Id == ConditionID.PAR || condition.Id == ConditionID.PSN)
            {
                return 1.5f;
            }
            else
            {
                return 1f;
            }
        }

        /// <summary>
        /// Dictionary for condition statuses
        /// </summary>
        public static Dictionary<ConditionID, ConditionObj> Conditions { get; set; } = new Dictionary<ConditionID, ConditionObj>()
        {
            // Status conditions
            {
                ConditionID.PSN,
                new ConditionObj()
                {
                    Name = "Poison",
                    StartMessage = "has been infected with poison!",
                    OnTurnEnd = (MonsterObj monster) =>
                    {
                        monster.UpdateHP(monster.MaxHp / 8);
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} is suffering from the effects of the poison!");
                    }
                }
            },
            {
                ConditionID.BRN,
                new ConditionObj()
                {
                    Name = "Burn",
                    StartMessage = "has been burned badly!",
                    OnTurnEnd = (MonsterObj monster) =>
                    {
                        monster.UpdateHP(monster.MaxHp / 16);
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} is suffering from the effects of the burn!");
                    }
                }
            },
            {
                ConditionID.SLP,
                new ConditionObj()
                {
                    Name = "Sleep",
                    StartMessage = "has been put to sleep!",
                    OnStart = (MonsterObj monster) =>
                    {
                        monster.StatusTimer = Random.Range(1, 4);
                        Debug.Log($"Sleep for {monster.StatusTimer} turns.");
                    },
                    OnTurnStart = (MonsterObj monster) =>
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
                new ConditionObj()
                {
                    Name = "Paralyze",
                    StartMessage = "has been paralyzed by that attack!",
                    OnTurnStart = (MonsterObj monster) =>
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
                new ConditionObj()
                {
                    Name = "Freeze",
                    StartMessage = "has been frozen by that attack!",
                    OnTurnStart = (MonsterObj monster) =>
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
                new ConditionObj()
                {
                    Name = "Confusion",
                    StartMessage = "has been confused and doesn't know what to do!",
                    OnStart = (MonsterObj monster) =>
                    {
                        monster.VolatileStatusTimer = Random.Range(1, 5);
                        Debug.Log($"Confused for {monster.VolatileStatusTimer} turns.");
                    },
                    OnTurnStart = (MonsterObj monster) =>
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
                        {
                            return true;
                        }

                        // Attack will hurt self
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} is suffering from the effects of the confusion! It attacked itself!");
                        monster.UpdateHP(monster.MaxHp / 8);
                        return false;
                    }
                }
            }
        };
    }
}