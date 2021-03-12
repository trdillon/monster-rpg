using Itsdits.Ravar.Monster.Condition;
using Itsdits.Ravar.Monster.Move;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Itsdits.Ravar.Monster { 

    [System.Serializable]
    public class MonsterObj
    {
        [SerializeField] MonsterBase _base;
        [SerializeField] int level;

        public MonsterObj(MonsterBase mbase, int mlvl)
        {
            _base = mbase;
            level = mlvl;
            Init();
        }

        #region Properties
        public int CurrentHp { get; set; }
        public int MaxHp { get; private set; }
        public int Exp { get; set; }
        public int StatusTimer { get; set; }
        public int VolatileStatusTimer { get; set; }
        public bool IsHpChanged { get; set; }
        public MoveObj CurrentMove { get; set; }
        public List<MoveObj> Moves { get; set; }
        public ConditionObj Status { get; private set; }
        public ConditionObj VolatileStatus { get; private set; }
        public Dictionary<MonsterStat, int> Stats { get; private set; }
        public Dictionary<MonsterStat, int> StatsChanged { get; private set; }
        public Queue<string> StatusChanges { get; private set; }
        public MonsterBase Base => _base;
        public int Level => level;
        public int Attack => GetStat(MonsterStat.Attack);
        public int Defense => GetStat(MonsterStat.Defense);
        public int SpAttack => GetStat(MonsterStat.SpAttack);
        public int SpDefense => GetStat(MonsterStat.SpDefense);
        public int Speed => GetStat(MonsterStat.Speed);
        #endregion

        public event Action OnStatusChange;

        /// <summary>
        /// Initialize the monster.
        /// </summary>
        public void Init()
        {
            //TODO - decide how to handle move list generation with more than MaxNumberOfMoves
            Moves = new List<MoveObj>();
            foreach (var move in Base.LearnableMoves)
            {
                if (move.LevelLearned <= Level)
                {
                    Moves.Add(new MoveObj(move.Base));
                }

                if (Moves.Count >= MonsterBase.MaxNumberOfMoves)
                {
                    break;
                }    
            }

            if (Moves.Count < 1)
            {
                Debug.LogError($"MO001: {Base.Name} initialized with no moves.");
            }

            Exp = Base.GetExpForLevel(Level);
            StatusChanges = new Queue<string>();
            CalculateStats();
            CurrentHp = MaxHp;
            ResetStatsChanged();
            RemoveStatus();
            RemoveVolatileStatus();
        }

        private void CalculateStats()
        {
            Stats = new Dictionary<MonsterStat, int>();
            Stats.Add(MonsterStat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
            Stats.Add(MonsterStat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
            Stats.Add(MonsterStat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
            Stats.Add(MonsterStat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
            Stats.Add(MonsterStat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);
            MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10 + Level;
        }

        private void ResetStatsChanged()
        {
            StatsChanged = new Dictionary<MonsterStat, int>()
            {
                {MonsterStat.Attack, 0},
                {MonsterStat.Defense, 0},
                {MonsterStat.SpAttack, 0},
                {MonsterStat.SpDefense, 0},
                {MonsterStat.Speed, 0},
                {MonsterStat.Accuracy, 0},
                {MonsterStat.Evasion, 0}
            };
        }

        private int GetStat(MonsterStat stat)
        {
            int statVal = Stats[stat];

            // Stat changes based on original game's formula
            int changeVal = StatsChanged[stat];
            var changeVals = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

            if (changeVal >= 0)
            {
                statVal = Mathf.FloorToInt(statVal * changeVals[changeVal]);
            }  
            else
            {
                statVal = Mathf.FloorToInt(statVal / changeVals[-changeVal]);
            }

            return statVal;
        }

        /// <summary>
        /// Apply stat changes to monster.
        /// </summary>
        /// <param name="statChanges">Changes to apply</param>
        public void ApplyStatChanges(List<StatChange> statChanges)
        {
            foreach (var statChange in statChanges)
            {
                var stat = statChange.stat;
                var changeVal = statChange.changeVal;
                StatsChanged[stat] = Mathf.Clamp(StatsChanged[stat] + changeVal, -6, 6);

                if (changeVal > 0)
                {
                    StatusChanges.Enqueue($"{Base.Name}'s {stat} increased!");
                }
                else
                {
                    StatusChanges.Enqueue($"{Base.Name}'s {stat} decreased!");
                }
            }
        }

        /// <summary>
        /// Set a status condition on monster.
        /// </summary>
        /// <param name="conditionID">Condition to set</param>
        public void SetStatus(ConditionID conditionID)
        {
            // Only one status effect at a time
            if (Status != null)
            {
                StatusChanges.Enqueue($"{Base.Name} is already being affected by another condition!");
                return;
            }

            Status = ConditionDB.Conditions[conditionID];
            Status?.OnStart?.Invoke(this);
            StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
            OnStatusChange?.Invoke();
        }

        /// <summary>
        /// Set a volatile status condition on monster.
        /// </summary>
        /// <param name="conditionID">Condition to set</param>
        public void SetVolatileStatus(ConditionID conditionID)
        {
            // Only one volatile status effect at a time
            if (VolatileStatus != null)
            {
                return;
            }
            
            VolatileStatus = ConditionDB.Conditions[conditionID];
            VolatileStatus?.OnStart?.Invoke(this);
            StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
        }

        /// <summary>
        /// Remove status condition.
        /// </summary>
        public void RemoveStatus()
        {
            Status = null;
            OnStatusChange?.Invoke();
        }

        /// <summary>
        /// Remove volatile status condition.
        /// </summary>
        public void RemoveVolatileStatus()
        {
            VolatileStatus = null;
        }

        /// <summary>
        /// Update monster HP after damage.
        /// </summary>
        /// <param name="damage">How much damage taken</param>
        public void UpdateHP(int damage)
        {
            CurrentHp = Mathf.Clamp(CurrentHp - damage, 0, MaxHp);
            IsHpChanged = true;
        }

        /// <summary>
        /// Calculate how much damage is taken.
        /// </summary>
        /// <param name="move">Move the monster is hit with</param>
        /// <param name="attacker">Monster who attacked</param>
        /// <returns>Damage to take</returns>
        public DamageDetails TakeDamage(MoveObj move, MonsterObj attacker)
        {
            // critical hit chance is 6.25%
            float critical = 1f;
            if (UnityEngine.Random.value * 100f <= 6.25f)
            {
                critical = 2f;
            }

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
            float modifiers = UnityEngine.Random.Range(0.85f, 1f) * type * critical;
            float a = (2 * attacker.Level + 10) / 250f;
            float d = a * move.Base.Power * ((float)attack / defense) + 2;
            int damage = Mathf.FloorToInt(d * modifiers);

            Debug.Log($"Damage was {damage}.");
            UpdateHP(damage);
            return damageDetails;
        }

        /// <summary>
        /// Learn a move by adding it to the monster's move list.
        /// </summary>
        /// <param name="newMove">Move to be learned</param>
        public void LearnMove(LearnableMove newMove)
        {
            if (Moves.Count > MonsterBase.MaxNumberOfMoves)
            {
                Debug.LogError("MO002: Move count maxed. Attempt to add failed.");
                return;
            }

            Moves.Add(new MoveObj(newMove.Base));
        }

        /// <summary>
        /// Forget a move to make space for a new one.
        /// </summary>
        /// <param name="oldMove">Move to forget</param>
        public void ForgetMove(MoveObj oldMove)
        {
            Moves.Remove(oldMove);
        }

        /// <summary>
        /// Gets a LearnableMove when monster levels up.
        /// </summary>
        /// <returns>Move that can be learned or null if list is empty</returns>
        public LearnableMove GetLearnableMove()
        {
            return Base.LearnableMoves.Where(m => m.LevelLearned == level).FirstOrDefault();
        }

        /// <summary>
        /// Select a random move for the enemy to use.
        /// </summary>
        /// <returns>Move to use</returns>
        public MoveObj GetRandomMove()
        {
            var usableMoves = Moves.Where(m => m.Energy > 0).ToList();
            if (usableMoves.Count > 0)
            {
                int i = UnityEngine.Random.Range(0, usableMoves.Count);
                return usableMoves[i];
            }
            else
            {
                Debug.LogError($"MO003: {Base.Name} has no usable moves. Escaping battle sequence to recover.");
                return null;
            }
        }

        /// <summary>
        /// Checks if the monster leveled up.
        /// </summary>
        /// <returns></returns>
        public bool CheckForLevelUp()
        {
            if (Exp > Base.GetExpForLevel(level + 1)) {
                ++level;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if monster can attack.
        /// </summary>
        /// <returns></returns>
        public bool CheckIfCanAttack()
        {
            bool canAttack = true;

            if (Status?.OnTurnStart != null)
            {
                if(!Status.OnTurnStart(this))
                    canAttack = false;
            }

            if (VolatileStatus?.OnTurnStart != null)
            {
                if (!VolatileStatus.OnTurnStart(this))
                    canAttack = false;
            }

            return canAttack;
        }

        /// <summary>
        /// Check for status damage after a turn.
        /// </summary>
        public void CheckForStatusDamage()
        {
            Status?.OnTurnEnd?.Invoke(this);
            VolatileStatus?.OnTurnEnd?.Invoke(this);
        }

        /// <summary>
        /// Resets monster stats and removes volatile statuses after battle.
        /// </summary>
        public void CleanUpMonster()
        {
            RemoveVolatileStatus();
            ResetStatsChanged();
        }
    }
}