namespace Itsdits.Ravar.Core.Signal
{
    /// <summary>
    /// Collection of the signals that are used in the game.
    /// </summary>
    public static class GameSignals
    {
        // These can easily cause memory leaks. Make sure to remove listeners on disable/destroy.
        public static readonly Signal<bool> PAUSE_GAME = new Signal<bool>();
        public static readonly Signal<bool> RESUME_GAME = new Signal<bool>();
        public static readonly Signal<string> SAVE_GAME = new Signal<string>();
        public static readonly Signal<string> LOAD_GAME = new Signal<string>();
    }
}