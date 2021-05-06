using System.Collections.Generic;
using System.Text.RegularExpressions;
using Itsdits.Ravar.Core.Signal;
using Itsdits.Ravar.Monster;
using Itsdits.Ravar.UI.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Party
{
    /// <summary>
    /// Controller class for the Party selection scene.
    /// </summary>
    public class PartyController : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("The Text element to display status messages and warnings.")]
        [SerializeField] private TextMeshProUGUI _messageText;
        [Tooltip("The Cancel button to close the Party screen without making a change.")]
        [SerializeField] private Button _cancelButton;
        [Tooltip("The Save button to close the Party screen and save changes.")]
        [SerializeField] private Button _saveButton;

        [Header("Monster UI Elements")]
        [Tooltip("List of prefabs that hold the monster UI elements in the party.")]
        [SerializeField] private List<PartyMemberUI> _memberUis;

        private TextLocalizer _localizer;
        private List<MonsterObj> _monsters;
        private int _selectedMonster;
        
        private void OnEnable()
        {
            GameSignals.PARTY_SHOW.AddListener(SetPartyData);
            _localizer = _messageText.GetComponent<TextLocalizer>();
            _cancelButton.onClick.AddListener(OnCancel);
            _saveButton.onClick.AddListener(OnSave);
        }

        private void OnDisable()
        {
            GameSignals.PARTY_SHOW.RemoveListener(SetPartyData);
            _cancelButton.onClick.RemoveListener(OnCancel);
            _saveButton.onClick.RemoveListener(OnSave);
        }

        private void Update()
        {
            if (!EventSystem.current.enabled)
            {
                return;
            }
            
            if (!EventSystem.current.currentSelectedGameObject.name.Contains("Party Member"))
            {
                return;
            }
            
            string selectionName = EventSystem.current.currentSelectedGameObject.name;
            string selectionIndex = Regex.Match(selectionName, @"\d+").Value;
            _selectedMonster = int.Parse(selectionIndex);
        }

        private void OnCancel()
        {
            GameSignals.PARTY_CLOSE.Dispatch(true);
        }

        private void OnSave()
        {
            if (_monsters[_selectedMonster].CurrentHp < 1)
            {
                _localizer.ChangeKey("PARTY_DOWNED_ERROR");
                return;
            }
            
            GameSignals.PARTY_CHANGE.Dispatch(_selectedMonster);
            GameSignals.PARTY_CLOSE.Dispatch(true);
        }

        private void SetPartyData(PartyItem party)
        {
            _monsters = party.Party.Monsters;
            for (var i = 0; i < _memberUis.Count; i++)
            {
                if (i < _monsters.Count)
                {
                    _memberUis[i].gameObject.SetActive(true);
                    _memberUis[i].SetData(_monsters[i]);
                } 
                else
                {
                    _memberUis[i].gameObject.SetActive(false);
                }   
            }
        }
    }
}