using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;

    PartyMemberUI[] members;
    List<Monster> monsters;

    public void Init()
    {
        members = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Monster> monsters) 
    {
        this.monsters = monsters;

        for (int i = 0; i < members.Length; i++)
        {
            if (i < monsters.Count)
                members[i].SetData(monsters[i]);
            else
                members[i].gameObject.SetActive(false);
        }

        messageText.text = "Select a monster";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            if (i == selectedMember)
                members[i].SetSelected(true);
            else
                members[i].SetSelected(false);
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
