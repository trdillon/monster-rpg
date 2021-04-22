using System.Collections;
using Itsdits.Ravar.Core;
using Itsdits.Ravar.Data;
using Itsdits.Ravar.Monster;
using Itsdits.Ravar.UI.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Controller class for the New Game scene. <seealso cref="MenuController"/>
    /// </summary>
    public class NewGameController : MenuController
    {
        [Header("Player Input")]
        [Tooltip("Player name. This is how the saved game will be identified.")]
        [SerializeField] private TMP_InputField _nameInput;
        [Tooltip("Monster selection label that is used to display input error feedback to the player.")]
        [SerializeField] private TextMeshProUGUI _inputFeedback;

        [Header("Monster Selection")]
        [Tooltip("ScriptableObject that represents the Wabbi MonsterBase.")]
        [SerializeField] private MonsterBase _wabbi;
        [Tooltip("ScriptableObject that represents the Firefly MonsterBase.")]
        [SerializeField] private MonsterBase _firefly;
        [Tooltip("ScriptableObject that represents the Tortoad MonsterBase.")]
        [SerializeField] private MonsterBase _tortoad;

        [Header("Monster Details")]
        [Tooltip("Text element that displays the currently selected monster's name.")]
        [SerializeField] private TextMeshProUGUI _monsterName;
        [Tooltip("Text element that displays the currently selected monster's type.")]
        [SerializeField] private TextMeshProUGUI _monsterType;
        [Tooltip("Text element that displays the currently selected monster's description.")]
        [SerializeField] private TextMeshProUGUI _monsterDescription;

        [Header("UI Buttons")]
        [Tooltip("Back button to return to the Main Menu")]
        [SerializeField] private Button _backButton;
        [Tooltip("Start button to begin a new game.")]
        [SerializeField] private Button _startButton;

        private string _selectedMonster;

        private void OnEnable()
        {
            EnableSceneManagement();
            _startButton.onClick.AddListener(StartGame);
            _backButton.onClick.AddListener(ReturnToMenu);
        }

        private void OnDisable()
        {
            _startButton.onClick.RemoveListener(StartGame);
            _backButton.onClick.RemoveListener(ReturnToMenu);
        }

        private void Update()
        {
            UpdateMonsterSelection(EventSystem.currentSelectedGameObject);
        }

        private void StartGame()
        {
            if (!ValidatePlayerName() || !ValidateMonsterSelection())
            {
                return;
            }

            DisableSceneManagement();
            GameData.NewGameData(_nameInput.text, _selectedMonster);
        }

        private void ReturnToMenu()
        {
            DisableSceneManagement();
            StartCoroutine(SceneLoader.Instance.LoadScene("UI.Menu.Main"));
        }

        private bool ValidatePlayerName()
        {
            string playerName = _nameInput.text;
            if (playerName.Length >= 2)
            {
                return true;
            }

            _inputFeedback.GetComponent<TextLocalizer>().ChangeKey("UI_NAME_LENGTH");
            return false;
        }

        private bool ValidateMonsterSelection()
        {
            if (_selectedMonster == "Wabbi" || _selectedMonster == "Firefly" || _selectedMonster == "Tortoad")
            {
                return true;
            }

            _inputFeedback.GetComponent<TextLocalizer>().ChangeKey("UI_MONSTER_EMPTY");
            return false;
        }

        private void UpdateMonsterSelection(Object selection)
        {
            // This gets called during Update() so the ToString() being called on the PrimaryType enums might cause
            // performance issues. We should probably look for another way to handle the text change.
            string selectionName = selection.name;
            if (selectionName == "Wabbi Button")
            {
                _selectedMonster = _wabbi.Name;
                _monsterName.text = _wabbi.Name;
                _monsterType.text = _wabbi.PrimaryType.ToString();
                _monsterDescription.text = _wabbi.Description;
            }
            else if (selectionName == "Firefly Button")
            {
                _selectedMonster = _firefly.Name;
                _monsterName.text = _firefly.Name;
                _monsterType.text = _firefly.PrimaryType.ToString();
                _monsterDescription.text = _firefly.Description;
            }
            else if (selectionName == "Tortoad Button")
            {
                _selectedMonster = _tortoad.Name;
                _monsterName.text = _tortoad.Name;
                _monsterType.text = _tortoad.PrimaryType.ToString();
                _monsterDescription.text = _tortoad.Description;
            }
        }
    }
}