using System.Collections.Generic;
using System.IO;
using Itsdits.Ravar.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Controller class for the Load Menu. Inherits from <see cref="MenuController"/>.
    /// </summary>
    public class LoadMenuController : MenuController
    {
        [Header("Load Games Scroll View")]
        [Tooltip("The Content element of the Scroll View used to display the games to load.")]
        [SerializeField] private GameObject _content;
        [Tooltip("The prefab of the TextMeshProUGUI element that will populate the Scroll View.")]
        [SerializeField] private GameObject _textPrefab;
        
        private const string SAVE_PATH = "/save/";
        private const string FILE_EXT = "*.ravar";

        private List<string> _gameNames = new List<string>();
        private string _selectedGame;
        
        private void Start()
        {
            ParsePathsToGameNames();
            PopulateContent();
        }

        protected override void HandleSelection(string selection)
        {
            if (selection == "UI_LOAD")
            {
                GameData.LoadGameData(_selectedGame);
            }
            else if (selection == "UI_BACK")
            {
                //TODO - check if player came from pause or main menu, then send them back to the proper scene.
                SceneManager.LoadScene("UI.Menu.Main");
            }
        }

        private void ParsePathsToGameNames()
        {
            string[] gamePaths = Directory.GetFiles(Application.persistentDataPath + SAVE_PATH, FILE_EXT);
            foreach (string path in gamePaths)
            {
                string fileName = Path.GetFileName(path);
                string[] nameWithoutExtension = fileName.Split('.');
                _gameNames.Add(nameWithoutExtension[0]);
            }
        }
        
        private void PopulateContent()
        {
            foreach (string game in _gameNames)
            {
                GameObject go = Instantiate(_textPrefab, _content.transform, false);
                var tmp = go.GetComponent<TextMeshProUGUI>();
                tmp.text = game;
            }
        }
    }
}