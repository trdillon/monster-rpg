using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    [SerializeField] List<string> strings;

    public List<string> Strings => strings;
}
