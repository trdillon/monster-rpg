using System;

namespace Itsdits.Ravar.Monster
{
    /// <summary>
    /// Implementation of <see cref="ConditionID"/> as defined in <see cref="ConditionDB"/>.
    /// </summary>
    public class ConditionObj
    {
        public ConditionID Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StartMessage { get; set; }
        public Action<MonsterObj> OnStart { get; set; }
        public Func<MonsterObj, bool> OnTurnStart { get; set; }
        public Action<MonsterObj> OnTurnEnd { get; set; }
    }
}