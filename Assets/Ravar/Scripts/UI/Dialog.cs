using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.UI
{
    [System.Serializable]
    public class Dialog
    {
        [SerializeField] List<string> strings;

        public List<string> Strings => strings;
    }
}