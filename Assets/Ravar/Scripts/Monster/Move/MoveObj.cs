namespace Itsdits.Ravar.Monster.Move {
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