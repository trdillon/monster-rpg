using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Monster.Condition
{
    /// <summary>
    /// Database class for definitions of <see cref="MoveEffects"/> and <see cref="MoveSecondaryEffects"/>.
    /// </summary>
    public static class ConditionDB
    {
        /// <summary>
        /// Initialize the DB.
        /// </summary>
        public static void Init()
        {
            foreach (KeyValuePair<ConditionID, ConditionObj> entry in Conditions)
            {
                ConditionID conditionId = entry.Key;
                ConditionObj condition = entry.Value;
                condition.Id = conditionId;
            }
        }

        /// <summary>
        /// Get status bonuses for capture algo.
        /// </summary>
        /// <param name="condition">Condition monster is affected by.</param>
        /// <returns>Boost value for capture algo.</returns>
        public static float GetStatusBonus(ConditionObj condition)
        {
            if (condition == null)
            {
                return 1f;
            }

            if (condition.Id == ConditionID.Freeze || condition.Id == ConditionID.Sleep)
            {
                return 2f;
            }

            if (condition.Id == ConditionID.Burn || condition.Id == ConditionID.Paralyze || condition.Id == ConditionID.Poison)
            {
                return 1.5f;
            }

            return 1f;
        }

        /// <summary>
        /// Static dictionary that holds the status conditions database.
        /// </summary>
        public static Dictionary<ConditionID, ConditionObj> Conditions { get; } = new Dictionary<ConditionID, ConditionObj>
        {
            // Status conditions
            {
                ConditionID.Poison,
                new ConditionObj
                {
                    Name = "Poison",
                    StartMessage = "has been infected with poison!",
                    OnTurnEnd = monster =>
                    {
                        monster.UpdateHp(monster.MaxHp / 8);
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} is suffering from the effects of the poison!");
                    }
                }
            },
            {
                ConditionID.Burn,
                new ConditionObj
                {
                    Name = "Burn",
                    StartMessage = "has been burned badly!",
                    OnTurnEnd = monster =>
                    {
                        monster.UpdateHp(monster.MaxHp / 16);
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} is suffering from the effects of the burn!");
                    }
                }
            },
            {
                ConditionID.Sleep,
                new ConditionObj
                {
                    Name = "Sleep",
                    StartMessage = "has been put to sleep!",
                    OnStart = monster =>
                    {
                        monster.StatusTimer = Random.Range(1, 4);
                    },
                    OnTurnStart = monster =>
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
                ConditionID.Paralyze,
                new ConditionObj
                {
                    Name = "Paralyze",
                    StartMessage = "has been paralyzed by that attack!",
                    OnTurnStart = monster =>
                    {
                        if (Random.Range(0, 5) != 1)
                        {
                            return true;
                        }

                        monster.StatusChanges.Enqueue($"{monster.Base.Name} is suffering from the effects of the paralysis! It can't attack!");
                        return false;
                    }
                }
            },
            {
                ConditionID.Freeze,
                new ConditionObj
                {
                    Name = "Freeze",
                    StartMessage = "has been frozen by that attack!",
                    OnTurnStart = monster =>
                    {
                        if (Random.Range(0, 5) != 1)
                        {
                            return false;
                        }

                        monster.RemoveStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} has thawed, it's no longer frozen!");
                        return true;
                    }
                }
            },
            // Volatile status conditions
            {
                ConditionID.Confusion,
                new ConditionObj
                {
                    Name = "Confusion",
                    StartMessage = "has been confused and doesn't know what to do!",
                    OnStart = monster =>
                    {
                        monster.VolatileStatusTimer = Random.Range(1, 5);
                    },
                    OnTurnStart = monster =>
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
                        monster.UpdateHp(monster.MaxHp / 8);
                        return false;
                    }
                }
            }
        };
    }
}