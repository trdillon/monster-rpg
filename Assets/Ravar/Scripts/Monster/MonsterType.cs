namespace Itsdits.Ravar.Monster 
{
    /// <summary>
    /// Types of <see cref="MonsterBase"/> and <see cref="MoveBase"/>. Used with <see cref="TypeChart"/> to determine strengths and weaknesses.
    /// </summary>
    /// <remarks>Types include Normal, Aqua, Ember, Earth, Shock, Air,
    /// Spirit, Force, Shadow, Light and None.
    /// None is used for empty optional secondary types.</remarks>
    public enum MonsterType
    {
        Normal,
        Aqua,
        Ember,
        Earth,
        Shock,
        Air,
        Spirit,
        Force,
        Shadow,
        Light,
        None
    }
}