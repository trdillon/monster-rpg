namespace Itsdits.Ravar.Core.Signal
{
    /// <summary>
    /// Collection of the <see cref="Signal{T}"/> that are used in the game. These are broadcast events that can pass
    /// data between scenes and trigger game flow operations.
    /// </summary>
    public static class GameSignals
    {
        // WARNING: These can easily cause memory leaks. Make sure to remove listeners on disable/destroy.
 
        // Game flow management
        public static readonly Signal<string> GAME_NEW = new Signal<string>();
        public static readonly Signal<string> GAME_SAVE = new Signal<string>();
        public static readonly Signal<string> GAME_LOAD = new Signal<string>();
        public static readonly Signal<bool> GAME_QUIT = new Signal<bool>();
        public static readonly Signal<bool> GAME_PAUSE = new Signal<bool>();
        public static readonly Signal<bool> GAME_RESUME = new Signal<bool>();

        // Level changing and quest triggers
        public static readonly Signal<bool> PORTAL_ENTER = new Signal<bool>();
        public static readonly Signal<bool> PORTAL_EXIT = new Signal<bool>();
 
        // Dialog
        public static readonly Signal<DialogItem> DIALOG_OPEN = new Signal<DialogItem>();
        public static readonly Signal<DialogItem> DIALOG_SHOW = new Signal<DialogItem>();
        public static readonly Signal<string> DIALOG_CLOSE = new Signal<string>();

        // Battle
        public static readonly Signal<BattlerEncounter> BATTLE_LOS = new Signal<BattlerEncounter>();
        public static readonly Signal<BattlerEncounter> BATTLE_OPEN = new Signal<BattlerEncounter>();
        public static readonly Signal<BattleItem> BATTLE_START = new Signal<BattleItem>();
        public static readonly Signal<BattleMove> BATTLE_MOVE_SELECT = new Signal<BattleMove>();
        public static readonly Signal<bool> BATTLE_MOVE_UPDATE = new Signal<bool>();
        // public static readonly Signal<> BATTLE_END = new Signal<>();
        
        // Party
        public static readonly Signal<bool> PARTY_OPEN = new Signal<bool>();
        public static readonly Signal<PartyItem> PARTY_SHOW = new Signal<PartyItem>();
        public static readonly Signal<int> PARTY_CHANGE = new Signal<int>();
        public static readonly Signal<bool> PARTY_CLOSE = new Signal<bool>();
    }
}