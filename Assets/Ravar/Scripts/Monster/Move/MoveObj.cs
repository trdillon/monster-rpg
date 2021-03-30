namespace Itsdits.Ravar.Monster
{
    /// <summary>
    /// Instance class of <see cref="MoveBase"/>.
    /// </summary>
    public class MoveObj
    {
        public MoveBase Base { get; set; }
        public int Energy { get; set; }

        public MoveObj(MoveBase mBase)
        {
            Base = mBase;
            Energy = mBase.Energy;
        }
    }
}