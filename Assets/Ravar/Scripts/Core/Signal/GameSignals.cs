namespace Itsdits.Ravar.Core.Signal
{
    /// <summary>
    /// Collection of the signals that are used in the game.
    /// </summary>
    public static class GameSignals
    {
        public static readonly Signal<DialogItem> DISPLAY_DIALOG = new Signal<DialogItem>();
        public static readonly Signal<bool> PAUSE_GAME = new Signal<bool>();
        public static readonly Signal<bool> RESUME_GAME = new Signal<bool>();
        public static readonly Signal<bool> SAVE_GAME = new Signal<bool>();
    }
}