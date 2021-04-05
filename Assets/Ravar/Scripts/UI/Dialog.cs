using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Holds the strings to display in the <see cref="DialogController"/>.
    /// </summary>
    [System.Serializable]
    public class Dialog
    {
        [Tooltip("List of dialog strings to be displayed.")]
        [SerializeField] List<string> strings;

        /// <summary>
        /// List of dialog strings to be displayed.
        /// </summary>
        public List<string> Strings => strings;
    }
}