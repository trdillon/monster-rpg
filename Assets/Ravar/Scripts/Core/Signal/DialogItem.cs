namespace Itsdits.Ravar.Core.Signal
{
    /// <summary>
    /// Struct for a dialog item containing the dialog to be displayed and the speaker's name.
    /// </summary>
    public readonly struct DialogItem
    {
        public readonly string[] Dialog;
        public readonly string Speaker;

        /// <summary>
        /// Constructor for a dialog item.
        /// </summary>
        /// <param name="dialog">Dialog to be displayed.</param>
        /// <param name="speaker">Speaker of the dialog.</param>
        public DialogItem(string[] dialog, string speaker)
        {
            Dialog = dialog;
            Speaker = speaker;
        }
    }
}