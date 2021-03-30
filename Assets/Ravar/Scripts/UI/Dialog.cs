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
        [SerializeField] List<string> strings;

        public List<string> Strings => strings;
    }
}