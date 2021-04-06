namespace Itsdits.Ravar.Core
{
    /// <summary>
    /// Represents the current state of the game.
    /// </summary>
    /// <remarks>Possible states include World, Battle, Dialog, Cutscene, Menu and Pause.</remarks>
    public enum GameState
    {
        World,
        Battle,
        Dialog,
        Cutscene,
        Menu,
        Pause
    }
}