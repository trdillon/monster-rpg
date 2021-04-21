namespace Itsdits.Ravar.Core.Signal
{
    /// <summary>
    /// Collection of the <see cref="Signal{T}"/> that are used in the game. These are broadcast events that can pass
    /// data between scenes and trigger game flow operations.
    /// </summary>
    public static class GameSignals
    {
        // WARNING: These can easily cause memory leaks. Make sure to remove listeners on disable/destroy.
        public static readonly Signal<bool> PAUSE_GAME = new Signal<bool>();
        public static readonly Signal<bool> RESUME_GAME = new Signal<bool>();
        public static readonly Signal<string> SAVE_GAME = new Signal<string>();
        public static readonly Signal<string> LOAD_GAME = new Signal<string>();
        public static readonly Signal<bool> QUIT_GAME = new Signal<bool>();
        public static readonly Signal<bool> PORTAL_ENTER = new Signal<bool>();
        public static readonly Signal<bool> PORTAL_EXIT = new Signal<bool>();
    }
}