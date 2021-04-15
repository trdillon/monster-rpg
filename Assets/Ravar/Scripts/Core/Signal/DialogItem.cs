namespace Itsdits.Ravar.Core.Signal
{
    /// <summary>
    /// Holds a dialog item containing the dialog to be displayed and the speaker's name.
    /// </summary>
    public readonly struct DialogItem
    {
        private readonly string _dialog;
        private readonly string _speaker;

        /// <summary>
        /// Constructor for a dialog item.
        /// </summary>
        /// <param name="dialog">Dialog to be displayed.</param>
        /// <param name="speaker">Speaker of the dialog.</param>
        public DialogItem(string dialog, string speaker)
        {
            _dialog = dialog;
            _speaker = speaker;
        }
    }
}
