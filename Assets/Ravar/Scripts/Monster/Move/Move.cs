public class Move
{
    public MoveBase Base { get; set; }
    public int Energy { get; set; }

    public Move(MoveBase mBase)
    {
        Base = mBase;
        Energy = mBase.Energy;
    }
}
