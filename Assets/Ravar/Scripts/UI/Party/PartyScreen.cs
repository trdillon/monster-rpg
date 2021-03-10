using Itsdits.Ravar.Monster;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Party
{
    public class PartyScreen : MonoBehaviour
    {
        [SerializeField] Text messageText;

        private PartyMemberUI[] members;
        private List<MonsterObj> monsters;

        /// <summary>
        /// Initialize the members array.
        /// </summary>
        public void Init()
        {
            members = GetComponentsInChildren<PartyMemberUI>(true);
        }

        /// <summary>
        /// Set the data for the monsters.
        /// </summary>
        /// <param name="monsters">Monsters in party</param>
        public void SetPartyData(List<MonsterObj> monsters)
        {
            this.monsters = monsters;

            for (int i = 0; i < members.Length; i++)
            {
                if (i < monsters.Count)
                {
                    members[i].gameObject.SetActive(true);
                    members[i].SetData(monsters[i]);
                } 
                else
                {
                    members[i].gameObject.SetActive(false);
                }   
            }
            messageText.text = "Select a monster";
        }

        /// <summary>
        /// Handle change in selected monster.
        /// </summary>
        /// <param name="selectedMember">Index of monster selected</param>
        public void UpdateMemberSelection(int selectedMember)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                if (i == selectedMember)
                {
                    members[i].SetSelected(true);
                } 
                else
                {
                    members[i].SetSelected(false);
                }
            }
        }

        /// <summary>
        /// Set the text of the message box.
        /// </summary>
        /// <param name="message">String to display</param>
        public void SetMessageText(string message)
        {
            messageText.text = message;
        }
    }
}

