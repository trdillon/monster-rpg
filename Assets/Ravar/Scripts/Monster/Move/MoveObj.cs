namespace Itsdits.Ravar.Monster
{
    /// <summary>
    /// Implements <see cref="MoveBase"/> and is used as moves during battles.
    /// </summary>
    [System.Serializable]
    public class MoveObj
    {
        /// <summary>
        /// <see cref="MoveBase"/> that the move is implemented from.
        /// </summary>
        public MoveBase Base { get; set; }
        /// <summary>
        /// The amount of energy remaining on this move.
        /// </summary>
        public int Energy { get; set; }

        /// <summary>
        /// Constructor for moves that will be added a monster's move list.
        /// </summary>
        /// <param name="mBase"></param>
        public MoveObj(MoveBase mBase)
        {
            Base = mBase;
            Energy = mBase.Energy;
        }

        /// <summary>
        /// Constructor for moves that are added when loading a saved monster. 
        /// This allows for persisting remaining Energy levels.
        /// </summary>
        /// <param name="mBase"></param>
        /// <param name="mEnergy"></param>
        public MoveObj(MoveBase mBase, int mEnergy)
        {
            Base = mBase;
            Energy = mEnergy;
        }
    }
}