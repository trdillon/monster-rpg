using System.Collections.Generic;
using Itsdits.Ravar.Settings;
using Itsdits.Ravar.UI.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Abstract controller class that provides the base functionality for Menu controllers.
    /// </summary>
    public abstract class MenuController : MonoBehaviour
    {
        [Header("Menu Elements")]
        [Tooltip("GameObject that holds the Pause Menu.")]
        [SerializeField] private GameObject _menuObject;
        [Tooltip("List of Text elements that display on the Pause Menu screen.")]
        [SerializeField] private List<TextMeshProUGUI> _menuTexts;
        
        [Header("Variables")]
        [Tooltip("The color to change the text to when highlighted.")]
        [SerializeField] private TMP_ColorGradient _highlightGradient;
        [Tooltip("The color to display when the text is not highlighted.")]
        [SerializeField] private TMP_ColorGradient _standardGradient;
        
        // Each TextMeshProUGUI will have a TextLocalizer component which contains a Localization string key.
        // We will use this key to determine which menu item the player is selecting instead of an int index.
        // This way we don't break menu selection if we change the order of the text elements.
        // We can't use TextMeshProUGUI.text because it will change to localized text on TextLocalizer.Awake.
        private readonly List<string> _menuKeys = new List<string>();

        private PlayerControls _controls;
        private InputAction _select;
        private InputAction _move;
        
        private int _menuIndex;
        private int _parsedYInput;
        
        private void Awake()
        {
            BuildKeyList();
        }

        private void OnEnable()
        {
            _controls = new PlayerControls();
            _controls.Enable();
            _select = _controls.UI.Select;
            _move = _controls.UI.Move;
            _move.performed += OnMove;
        }

        private void OnDisable()
        {
            _move.performed -= OnMove;
            _controls.Disable();
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
            // Reads the composite binding input from keyboard and d-pad only.
            ParseInput(context.ReadValue<Vector2>());
        }

        private void Update()
        {
            HandleInput();
        }
        
        /// <summary>
        /// Handles the selection of a Menu item.
        /// </summary>
        /// <param name="selection">The key of the TextLocalizer component attached to the player's selection.</param>
        protected abstract void HandleSelection(string selection);
        
        protected virtual void HandleInput()
        {
            // First we check if there is any Y input to navigate the menu.
            if (_parsedYInput == -1)
            {
                _menuIndex += 1;
                _parsedYInput = 0;
            }
            else if (_parsedYInput == 1)
            {
                _menuIndex -= 1;
                _parsedYInput = 0;
            }

            // Clamp to avoid index out of bounds. Then call UpdateIndex to give visual feedback to the player.
            _menuIndex = Mathf.Clamp(_menuIndex, 0, _menuTexts.Count - 1);
            UpdateIndex(_menuIndex);

            // Return if the player hasn't triggered a selection.
            if (!_select.triggered)
            {
                return;
            }
            
            HandleSelection(_menuKeys[_menuIndex]);
        }
        
        private void BuildKeyList()
        {
            foreach (TextMeshProUGUI t in _menuTexts)
            {
                _menuKeys.Add(t.GetComponent<TextLocalizer>().Key);
            }
        }
        
        private void ParseInput(Vector2 input)
        {
            // Only x or y should have a non-zero value to prevent diagonal movement.
            if (input.x != 0)
            {
                input.y = 0;
            }
            
            // Normalize the Vector2 because the composite mode on the input bindings can cause != 1f inputs.
            Vector2 parsedVector = input.normalized; 
            _parsedYInput = Mathf.FloorToInt(parsedVector.y);
        }

        private void UpdateIndex(int index)
        {
            for (var i = 0; i < _menuTexts.Count; ++i)
            {
                _menuTexts[i].colorGradientPreset = i == index ? _highlightGradient : _standardGradient;
            }
        }
    }
}