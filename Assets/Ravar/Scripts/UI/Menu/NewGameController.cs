using System.Collections;
using System.Collections.Generic;
using Itsdits.Ravar.Core;
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

        [Header("Monster Selection")]
        [Tooltip("List of buttons that represent the monster choice selections.")]
        [SerializeField] private List<Button> _monsterButtons;
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
            _startButton.onClick.AddListener(() => StartCoroutine(StartGame()));
            _backButton.onClick.AddListener(ReturnToMenu);
        }

        private void OnDisable()
        {
            _startButton.onClick.RemoveListener(() => StartCoroutine(StartGame()));
            _backButton.onClick.RemoveListener(ReturnToMenu);
        }

        private IEnumerator StartGame()
        {
            DisableSceneManagement();
            yield return SceneLoader.Instance.LoadScene("World.Fornwest.Main");
        }

        private void ReturnToMenu()
        {
            DisableSceneManagement();
            StartCoroutine(SceneLoader.Instance.LoadScene("UI.Menu.Main"));
        }
    }
}
