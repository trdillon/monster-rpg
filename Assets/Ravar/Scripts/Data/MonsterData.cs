// ReSharper disable InconsistentNaming
namespace Itsdits.Ravar.Data
{
    /// <summary>
    /// Data class that holds data for a Monster such as monster base, level and moves learned.
    /// </summary>
    [System.Serializable]
    public class MonsterData
    {
        public string monsterName;
        public int currentLevel;
        public int currentExp;
        public int currentHp;
        public string[] currentMoves;
        public int[] currentEnergy;

        /// <summary>
        /// Constructor for a MonsterData data object.
        /// </summary>
        /// <param name="name">Name of the monster.</param>
        /// <param name="level">Current level of the monster.</param>
        /// <param name="exp">Current experience at this level.</param>
        /// <param name="hp">Current Hp of the monster.</param>
        /// <param name="moves">Move numbers that hold a reference to the <see cref="Monster.MoveBase"/> of the monster's learned moves.</param>
        /// <param name="energy">Current energy level of each move in the move list.</param>
        public MonsterData(string name, int level, int exp, int hp, string[] moves, int[] energy)
        {
            monsterName = name;
            currentLevel = level;
            currentExp = exp;
            currentHp = hp;
            currentMoves = moves;
            currentEnergy = energy;
        }
    }
}
