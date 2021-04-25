using System.Collections;
using System.Collections.Generic;
using Itsdits.Ravar.Monster.Move;
using Itsdits.Ravar.Settings;
using Itsdits.Ravar.UI.Battle;
using Itsdits.Ravar.UI.Localization;
using Itsdits.Ravar.Util;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Itsdits.Ravar.Battle
{
    /// <summary>
    /// Controller class that handles the UI during Battle scenes.
    /// </summary>
    public class BattleUIController : MonoBehaviour
    {
        [Header("Background")]
        [Tooltip("Background image for this battle.")]
        [SerializeField] private Image _background;
        [Tooltip("List of background images that change based on region.")]
        [SerializeField] private List<Image> _backgrounds;

        [Header("Monster HUDs")]
        [Tooltip("The BattleHUD GameObject for the player's monster.")]
        [SerializeField] private BattleHud _playerHud;
        [Tooltip("The BattleHUD GameObject for the enemy's monster.")]
        [SerializeField] private BattleHud _enemyHud;
        
        [Header("Dialog Box")]
        [Tooltip("The Text element that displays the current dialog.")]
        [SerializeField] private TextMeshProUGUI _dialogText;

        [Header("Action Selector")]
        [Tooltip("GameObject that holds the Action Selector.")]
        [SerializeField] private GameObject _actionSelector;
        [Tooltip("List of Text elements that display on the Action Selection screen.")]
        [SerializeField] private List<Button> _actionButtons;
        
        [Header("Move Selector")]
        [Tooltip("GameObject that holds the Move Selector.")]
        [SerializeField] private GameObject _moveSelector;
        [Tooltip("List of Text elements that display on the Move Selection screen.")]
        [SerializeField] private List<Button> _moveButtons;
        
        [Header("Move Details")]
        [Tooltip("GameObject that holds the Move Details.")]
        [SerializeField] private GameObject _moveDetails;
        [Tooltip("Text element that displays the move's energy.")]
        [SerializeField] private TextMeshProUGUI _energyText;
        [Tooltip("Text element that displays the move's type.")]
        [SerializeField] private TextMeshProUGUI _typeText;

        [Header("Choice Selector")]
        [Tooltip("GameObject that holds the Choice Selector.")]
        [SerializeField] private GameObject _choiceSelector;
        [Tooltip("Text element that holds the Yes text.")]
        [SerializeField] private Button _yesButton;
        [Tooltip("Text element that holds the No text.")]
        [SerializeField] private Button _noButton;

        [Header("Variables")]
        [Tooltip("The gradient to apply to the text to when highlighted.")]
        [SerializeField] private TMP_ColorGradient _highlightGradient;
        [Tooltip("The gradient to apply to the text when the text is not highlighted.")]
        [SerializeField] private TMP_ColorGradient _standardGradient;

        private const string MOVE_KEY_PREFIX = "MOVE_";
        
        private PlayerControls _controls;
        private InputAction _interact;
        private InputAction _cancel;
        private Vector2 _inputVector;

        private TextLocalizer _textLocalizer;

        private void OnEnable()
        {
            _controls = new PlayerControls();
            _controls.Enable();
            _interact = _controls.Player.Interact;
            _cancel = _controls.Player.Cancel;
            _textLocalizer = _dialogText.GetComponent<TextLocalizer>();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }
        
        public IEnumerator TypeDialog(string dialog)
        {
            _textLocalizer.ChangeKey(dialog);
            _dialogText.ForceMeshUpdate();
            int totalVisibleCharacters = _dialogText.textInfo.characterCount;
            var counter = 0;

            while (true)
            {
                int visibleCount = counter % (totalVisibleCharacters + 1);
                _dialogText.maxVisibleCharacters = visibleCount;

                if (visibleCount >= totalVisibleCharacters)
                {
                    // Finished displaying all dialog. Wait until the player has finished reading to break out.
                    yield return new WaitUntil(() => _interact.triggered);
                    yield break;
                }
                
                counter += 1;
                yield return YieldHelper.TYPING_TIME;
            }
        }
        
        /// <summary>
        /// Set the move list.
        /// </summary>
        /// <param name="moves">Available moves.</param>
        public void SetMoveList(List<MoveObj> moves)
        {
            for (var i = 0; i < _moveButtons.Count; ++i)
            {
                if (i >= moves.Count)
                {
                    continue;
                }
                
                var localizer = _moveButtons[i].GetComponentInChildren<TextLocalizer>();
                localizer.ChangeKey(MOVE_KEY_PREFIX + moves[i].Base.MoveName);
            }
        }
        
        /// <summary>
        /// Deactivate the BattleHUD GameObjects to hide them.
        /// </summary>
        /// <remarks>Used in the beginning of a battle to clear the screen for battle setup.</remarks>
        public void HideHud()
        {
            _playerHud.gameObject.SetActive(false);
            _enemyHud.gameObject.SetActive(false);
        }
    }
}