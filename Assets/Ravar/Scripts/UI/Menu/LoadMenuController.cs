using System.Collections.Generic;
using System.IO;
using Itsdits.Ravar.Core.Signal;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Controller class for the Load Menu.
    /// </summary>
    public class LoadMenuController : MonoBehaviour
    {
        [Header("UI Buttons")]
        [Tooltip("Button for returning to the game.")]
        [SerializeField] private Button _backButton;
        [Tooltip("Button for returning to the game.")]
        [SerializeField] private Button _loadButton;
        
        [Header("Save Games Scroll View")]
        [Tooltip("The Content container of the Scroll View used to display the games to load.")]
        [SerializeField] private GameObject _content;
        [Tooltip("The prefab of the buttons that will populate the Scroll View.")]
        [SerializeField] private GameObject _buttonPrefab;

        private const string SAVE_PATH = "/save/";
        private const string FILE_EXT = "*.ravar";

        private List<Button> _contentButtons = new List<Button>();
        private List<string> _gameNames = new List<string>();
        private string _selectedGame;
        
        private void Start()
        {
            ParsePathsToGameNames();
            PopulateContent();
            AddDoubleClickListeners();
            _loadButton.onClick.AddListener(() => LoadGame(_selectedGame));
            _backButton.onClick.AddListener(PauseMenu);
        }

        private void OnDestroy()
        {
            RemoveDoubleClickListeners();
            _loadButton.onClick.RemoveListener(() => LoadGame(_selectedGame));
            _backButton.onClick.RemoveListener(PauseMenu);
        }

        private void Update()
        {
            // Save game files are accessed by button prefabs. As long as this is the only prefab being instantiated
            // in this scene, then searching for "Clone" should return only the save games.
            if (!EventSystem.current.currentSelectedGameObject.name.Contains("Clone"))
            {
                return;
            }
            
            // Calling GetComponent in Update should probably be avoided, but we can live with it here because
            // there shouldn't be much else going on during this part of the game.
            GameObject currentGo = EventSystem.current.currentSelectedGameObject;
            _selectedGame = currentGo.GetComponentInChildren<TextMeshProUGUI>().text;
        }

        private void ParsePathsToGameNames()
        {
            string[] gamePaths = Directory.GetFiles(Application.persistentDataPath + SAVE_PATH, FILE_EXT);
            foreach (string path in gamePaths)
            {
                string fileName = Path.GetFileName(path);
                
                // As long as the player doesn't add a period to their character's name this should work fine.
                //TODO - add string validation when player name entry is added
                string[] nameWithoutExtension = fileName.Split('.');
                _gameNames.Add(nameWithoutExtension[0]);
            }
        }
        
        private void PopulateContent()
        {
            _contentButtons.Clear();
            foreach (string game in _gameNames)
            {
                GameObject go = Instantiate(_buttonPrefab, _content.transform, false);
                var buttonComponent = go.GetComponent<Button>();
                _contentButtons.Add(buttonComponent);
                var textComponent = go.GetComponentInChildren<TextMeshProUGUI>();
                textComponent.text = game;
            }
        }

        private void AddDoubleClickListeners()
        {
            foreach (Button button in _contentButtons)
            {
                button.GetComponent<LoadOnDoubleClick>().OnDoubleClicked += LoadGame;
            }
        }

        private void RemoveDoubleClickListeners()
        {
            foreach (Button button in _contentButtons)
            {
                button.GetComponent<LoadOnDoubleClick>().OnDoubleClicked -= LoadGame;
            }
        }

        private void LoadGame(string selectedGame)
        {
            if (selectedGame == null)
            {
                return;
            }
            
            GameSignals.LOAD_GAME.Dispatch(selectedGame);
            Debug.Log($"Loading game: {selectedGame}");
        }

        private void PauseMenu()
        {
            SceneManager.LoadScene("UI.Menu.Pause");
        }
    }
}