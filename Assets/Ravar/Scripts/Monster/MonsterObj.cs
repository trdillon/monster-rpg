using Itsdits.Ravar.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Itsdits.Ravar.Monster.Condition;
using Itsdits.Ravar.Monster.Move;
using Itsdits.Ravar.UI.Battle;
using UnityEngine;

namespace Itsdits.Ravar.Monster 
{ 
    /// <summary>
    /// Implements <see cref="MonsterBase"/>. Holds level, experience, move list, status conditions and stat changes.
    /// </summary>
    [Serializable]
    public class MonsterObj
    {
        [Tooltip("The base class of this monster.")]
        [SerializeField] private MonsterBase _base;
        [Tooltip("The current level of this monster.")]
        [SerializeField] private int _level;

        /// <summary>
        /// Constructor used for wild encounters.
        /// </summary>
        /// <param name="mBase">Monster base to construct from.</param>
        /// <param name="level">Level to construct the monster at.</param>
        public MonsterObj(MonsterBase mBase, int level)
        {
            _base = mBase;
            _level = level;
            Init();
        }

        /// <summary>
        /// Constructor used for loading monsters from saved <see cref="MonsterData"/>.
        /// </summary>
        /// <param name="monsterData">Saved data of the monster to be loaded.</param>
        public MonsterObj(MonsterData monsterData)
        {
            _base = Resources.Load<MonsterBase>($"Monsters/{monsterData.monsterName}");
            _level = monsterData.currentLevel;
            Exp = monsterData.currentExp;
            CurrentHp = monsterData.currentHp;
            Moves = new List<MoveObj>();

            for (var i = 0; i < monsterData.currentMoves.Length; i++)
            {
                var moveBase = Resources.Load<MoveBase>($"Moves/{monsterData.currentMoves[i]}");
                int moveEnergy = monsterData.currentEnergy[i];
                var move = new MoveObj(moveBase, moveEnergy);
                Moves.Add(move);
            }

            StatusChanges = new Queue<string>();
            CalculateStats();
            ResetStatsChanged();
            RemoveStatus();
            RemoveVolatileStatus();
        }

        /// <summary>
        /// <see cref="MonsterBase"/> that the monster is implemented from.
        /// </summary>
        public MonsterBase Base => _base;
        /// <summary>
        /// Current level of the monster.
        /// </summary>
        public int Level => _level;
        /// <summary>
        /// Current experience of the monster.
        /// </summary>
        public int Exp { get; set; }
        /// <summary>
        /// Current HP of the monster.
        /// </summary>
        public int CurrentHp { get; set; }
        /// <summary>
        /// Maximum HP the monster can have.
        /// </summary>
        public int MaxHp { get; private set; }
        /// <summary>
        /// Checks if HP has changed to update the <see cref="BattleHud"/>.
        /// </summary>
        public bool IsHpChanged { get; set; }
        /// <summary>
        /// Attack stat of the monster at it's current level.
        /// </summary>
        public int Attack => GetStat(MonsterStat.Attack);
        /// <summary>
        /// Defense stat of the monster at it's current level.
        /// </summary>
        public int Defense => GetStat(MonsterStat.Defense);
        /// <summary>
        /// Special Attack stat of the monster at it's current level.
        /// </summary>
        public int SpAttack => GetStat(MonsterStat.SpAttack);
        /// <summary>
        /// Special Defense stat of the monster at it's current level.
        /// </summary>
        public int SpDefense => GetStat(MonsterStat.SpDefense);
        /// <summary>
        /// Speed stat of the monster at it's current level.
        /// </summary>
        public int Speed => GetStat(MonsterStat.Speed);
        /// <summary>
        /// The monster's current stats.
        /// </summary>
        public Dictionary<MonsterStat, int> Stats { get; private set; }
        /// <summary>
        /// The amount of change to the monster's stats after being affected by a stat changing move.
        /// </summary>
        /// <remarks>Range is -6 to 6.</remarks>
        public Dictionary<MonsterStat, int> StatsChanged { get; private set; }
        /// <summary>
        /// The move being used on this turn.
        /// </summary>
        public MoveObj CurrentMove { get; set; }
        /// <summary>
        /// List of move the monster knows.
        /// </summary>
        /// <remarks>Max is 4.</remarks>
        public List<MoveObj> Moves { get; set; }
        /// <summary>
        /// Status condition the monster is affected by.
        /// </summary>
        public ConditionObj Status { get; private set; }
        /// <summary>
        /// Volatile status condition the monster is affected by.
        /// </summary>
        public ConditionObj VolatileStatus { get; private set; }
        /// <summary>
        /// Moves left until the status wears off.
        /// </summary>
        public int StatusTimer { get; set; }
        /// <summary>
        /// Moves left until the volatile status wears off.
        /// </summary>
        public int VolatileStatusTimer { get; set; }
        /// <summary>
        /// Message queue for displaying dialog regarding status changes.
        /// </summary>
        public Queue<string> StatusChanges { get; private set; }

        public event Action OnStatusChange;

        /// <summary>
        /// Initialize the monster.
        /// </summary>
        public void Init()
        {
            Moves = new List<MoveObj>();
            foreach (LearnableMove move in Base.LearnableMoves)
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

            Exp = Base.GetExpForLevel(Level);
            StatusChanges = new Queue<string>();
            CalculateStats();
            CurrentHp = MaxHp;
            ResetStatsChanged();
            RemoveStatus();
            RemoveVolatileStatus();
        }

        ////////////// DAMAGE AND STAT FUNCTIONS ////////////////

        /// <summary>
        /// Calculate how much damage is taken.
        /// </summary>
        /// <param name="move">Move the monster is hit with.</param>
        /// <param name="attacker">Monster who attacked.</param>
        /// <returns>Updated currentHP after damage and <see cref="DamageDetails"/>.</returns>
        public DamageDetails TakeDamage(MoveObj move, MonsterObj attacker)
        {
            // Critical hit chance is 6.25%.
            var critical = 1f;
            if (UnityEngine.Random.value * 100f <= 6.25f)
            {
                critical = 2f;
            }

            // Type effectiveness defined in TypeChart.
            float type = TypeChart.GetEffectiveness(move.Base.Type, Base.PrimaryType) * 
                         TypeChart.GetEffectiveness(move.Base.Type, Base.SecondaryType);

            var damageDetails = new DamageDetails()
            {
                Critical = critical,
                TypeEffectiveness = type,
                Downed = false
            };

            float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
            float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;

            // Damage calculation based on the original game's formula.
            float modifiers = UnityEngine.Random.Range(0.85f, 1f) * type * critical;
            float a = (2 * attacker.Level + 10) / 250f;
            float d = a * move.Base.Power * (attack / defense) + 2;
            int damage = Mathf.FloorToInt(d * modifiers);

            UpdateHp(damage);
            return damageDetails;
        }

        /// <summary>
        /// Checks if monster can attack this turn.
        /// </summary>
        /// <returns>True if monster can attack or false if not.</returns>
        public bool CheckIfCanAttack()
        {
            var canAttack = true;
            if (Status?.OnTurnStart != null)
            {
                if (!Status.OnTurnStart(this))
                {
                    canAttack = false;
                }
            }

            if (VolatileStatus?.OnTurnStart == null)
            {
                return canAttack;
            }

            if (!VolatileStatus.OnTurnStart(this))
            {
                canAttack = false;
            }

            return canAttack;
        }

        /// <summary>
        /// Update monster HP after taking damage.
        /// </summary>
        /// <param name="damage">How much damage was taken.</param>
        public void UpdateHp(int damage)
        {
            CurrentHp = Mathf.Clamp(CurrentHp - damage, 0, MaxHp);
            IsHpChanged = true;
        }

        /// <summary>
        /// Apply stat changes to monster.
        /// </summary>
        /// <remarks>Range is -6 to 6.</remarks>
        /// <param name="statChanges">Changes to apply.</param>
        public void ApplyStatChanges(List<StatChange> statChanges)
        {
            foreach (StatChange statChange in statChanges)
            {
                MonsterStat stat = statChange.stat;
                int changeVal = statChange.changeVal;
                StatsChanged[stat] = Mathf.Clamp(StatsChanged[stat] + changeVal, -6, 6);

                StatusChanges.Enqueue(changeVal > 0
                                          ? $"{Base.Name}'s {stat} increased!"
                                          : $"{Base.Name}'s {stat} decreased!");
            }
        }

        private void CalculateStats()
        {
            Stats = new Dictionary<MonsterStat, int>
            {
                { MonsterStat.Attack, Mathf.FloorToInt(Base.Attack * Level / 100f) + 5 },
                { MonsterStat.Defense, Mathf.FloorToInt(Base.Defense * Level / 100f) + 5 },
                { MonsterStat.SpAttack, Mathf.FloorToInt(Base.SpAttack * Level / 100f) + 5 },
                { MonsterStat.SpDefense, Mathf.FloorToInt(Base.SpDefense * Level / 100f) + 5 },
                { MonsterStat.Speed, Mathf.FloorToInt(Base.Speed * Level / 100f) + 5 }
            };
            MaxHp = Mathf.FloorToInt(Base.MaxHp * Level / 100f) + 10 + Level;
        }
        private void ResetStatsChanged()
        {
            StatsChanged = new Dictionary<MonsterStat, int>
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

            // Stat changes based on original game's formula.
            int changeVal = StatsChanged[stat];
            float[] changeVals = { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

            statVal = changeVal >= 0 ? Mathf.FloorToInt(statVal * changeVals[changeVal]) : 
                                       Mathf.FloorToInt(statVal / changeVals[-changeVal]);

            return statVal;
        }

        ////////////// STATUS FUNCTIONS ////////////////

        /// <summary>
        /// Set a status condition on monster.
        /// </summary>
        /// <param name="conditionID">Condition to set.</param>
        public void SetStatus(ConditionID conditionID)
        {
            // Only one status effect at a time.
            if (Status != null)
            {
                StatusChanges.Enqueue($"{Base.Name} is already being affected by another condition!");
                return;
            }

            Status = ConditionDB.Conditions[conditionID];
            Status?.OnStart?.Invoke(this);
            if (Status != null)
            {
                StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
            }

            OnStatusChange?.Invoke();
        }

        /// <summary>
        /// Set a volatile status condition on monster.
        /// </summary>
        /// <param name="conditionID">Condition to set.</param>
        public void SetVolatileStatus(ConditionID conditionID)
        {
            // Only one volatile status effect at a time.
            if (VolatileStatus != null)
            {
                return;
            }
            
            VolatileStatus = ConditionDB.Conditions[conditionID];
            VolatileStatus?.OnStart?.Invoke(this);
            if (VolatileStatus != null)
            {
                StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
            }
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
        /// Resets monster stats and removes volatile statuses after battle.
        /// </summary>
        public void CleanUpMonster()
        {
            RemoveVolatileStatus();
            ResetStatsChanged();
        }

        ////////////// MOVE FUNCTIONS ////////////////

        /// <summary>
        /// Learn a move by adding it to the monster's move list.
        /// </summary>
        /// <param name="newMove">Move to be learned.</param>
        public void LearnMove(LearnableMove newMove)
        {
            if (Moves.Count > MonsterBase.MaxNumberOfMoves)
            {
                return;
            }
            Debug.Log($"Learning {newMove.Base.Name}");
            Moves.Add(new MoveObj(newMove.Base));
        }

        /// <summary>
        /// Forget a move to make space for a new one.
        /// </summary>
        /// <param name="oldMove">Move to forget.</param>
        public void ForgetMove(MoveObj oldMove)
        {
            Debug.Log($"Forgetting {oldMove.Base.Name}");
            Moves.Remove(oldMove);
        }

        /// <summary>
        /// Gets a LearnableMove when monster levels up.
        /// </summary>
        /// <returns>Move that can be learned or null if no new moves at this level.</returns>
        public LearnableMove GetLearnableMove()
        {
            return Base.LearnableMoves.FirstOrDefault(m => m.LevelLearned == _level);
        }

        /// <summary>
        /// Select a random move for the monster to execute.
        /// </summary>
        /// <remarks>Used for move selection by enemy monsters.</remarks>
        /// <returns>Move to use on this turn.</returns>
        public MoveObj GetRandomMove()
        {
            List<MoveObj> usableMoves = Moves.Where(m => m.Energy > 0).ToList();
            if (usableMoves.Count <= 0)
            {
                //TODO - handle this case properly
                return null;
            }

            int i = UnityEngine.Random.Range(0, usableMoves.Count);
            return usableMoves[i];
        }

        /// <summary>
        /// Checks if the monster leveled up after gaining experience.
        /// </summary>
        /// <returns>Level increase if true, nothing if false.</returns>
        public bool CheckForLevelUp()
        {
            if (Exp <= Base.GetExpForLevel(_level + 1))
            {
                return false;
            }

            ++_level;
            return true;
        }

        ////////////// SAVE AND LOAD FUNCTIONS ////////////////

        /// <summary>
        /// Saves the current monster's data to a new MonsterData object.
        /// </summary>
        /// <returns>MonsterData object with the current monster's data.</returns>
        public MonsterData SaveMonsterData()
        {
            var monsterData = new MonsterData(
                Base.Name,
                Level,
                Exp,
                CurrentHp,
                GetMoveListOnSave(),
                GetMoveEnergyOnSave()
                );

            return monsterData;
        }

        private string[] GetMoveListOnSave()
        {
            return Moves.Select(move => move.Base.MoveName).ToArray();
        }

        private int[] GetMoveEnergyOnSave()
        {
            return Moves.Select(move => move.Energy).ToArray();
        }
    }
}