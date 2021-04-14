namespace Itsdits.Ravar.Monster.Move
{
    /// <summary>
    /// Category of a <see cref="MoveBase"/>.
    /// </summary>
    /// <remarks>Physical moves are affected by Attack and Defense, 
    /// Special moves are affected by Special Attack and Special Defense, 
    /// and Status moves inflict status conditions.</remarks>
    public enum MoveCategory
    {
        Physical,
        Special,
        Status
    }
}