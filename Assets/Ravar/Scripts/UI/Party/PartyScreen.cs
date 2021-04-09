using Itsdits.Ravar.Monster;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// UI class for managing a <see cref="MonsterParty"/>.
    /// </summary>
    public class PartyScreen : MonoBehaviour
    {
        [Tooltip("The message Text to display in the screen.")]
        [SerializeField] private Text _messageText;

        private PartyMemberUI[] _members;
        private List<MonsterObj> _monsters;

        /// <summary>
        /// Initialize the members array.
        /// </summary>
        public void Init()
        {
            _members = GetComponentsInChildren<PartyMemberUI>(true);
        }

        /// <summary>
        /// Set the data for the monsters.
        /// </summary>
        /// <param name="monsters">Monsters in party</param>
        public void SetPartyData(List<MonsterObj> monsters)
        {
            _monsters = monsters;
            for (var i = 0; i < _members.Length; i++)
            {
                if (i < monsters.Count)
                {
                    _members[i].gameObject.SetActive(true);
                    _members[i].SetData(monsters[i]);
                } 
                else
                {
                    _members[i].gameObject.SetActive(false);
                }   
            }
            _messageText.text = "Select a Battokuri";
        }

        /// <summary>
        /// Handle change in selected monster.
        /// </summary>
        /// <param name="selectedMember">Index of monster selected</param>
        public void UpdateMemberSelection(int selectedMember)
        {
            for (var i = 0; i < _monsters.Count; i++)
            {
                _members[i].SetSelected(i == selectedMember);
            }
        }

        /// <summary>
        /// Set the text of the message box.
        /// </summary>
        /// <param name="message">String to display</param>
        public void SetMessageText(string message)
        {
            _messageText.text = message ?? "";
        }
    }
}

