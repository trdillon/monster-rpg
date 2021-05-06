using System;
using System.Collections;
using System.Collections.Generic;
using Itsdits.Ravar.Core.Signal;
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
        [Tooltip("Button for the Fight action selection.")]
        [SerializeField] private Button _fightButton;
        [Tooltip("Button for the Item action selection.")]
        [SerializeField] private Button _itemButton;
        [Tooltip("Button for the Party action selection.")]
        [SerializeField] private Button _partyButton;
        [Tooltip("Button for the Run action selection.")]
        [SerializeField] private Button _runButton;

        [Header("Move Selector")]
        [Tooltip("GameObject that holds the Move Selector.")]
        [SerializeField] private GameObject _moveSelector;
        [Tooltip("List of buttons in the Move Selection box.")]
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

        private const string MOVE_KEY_PREFIX = "MOVE_";

        private BattleState _state;
        private TextLocalizer _dialogLocalizer;
        
        private PlayerControls _controls;
        private InputAction _interact;
        private InputAction _cancel;

        private List<MoveObj> _moves;
        private int _currentMove;

        public BattleState State => _state;
        public BattleHud PlayerHud => _playerHud;
        public BattleHud EnemyHud => _enemyHud;
        
        private void OnEnable()
        {
            GameSignals.BATTLE_MOVE_UPDATE.AddListener(SetMoveList);
            _controls = new PlayerControls();
            _controls.Enable();
            _interact = _controls.Player.Interact;
            _cancel = _controls.Player.Cancel;
            _dialogLocalizer = _dialogText.GetComponent<TextLocalizer>();
            _fightButton.onClick.AddListener(() => OnActionSelected(BattleAction.Move));
            _itemButton.onClick.AddListener(() => OnActionSelected(BattleAction.UseItem));
            _partyButton.onClick.AddListener(() => OnActionSelected(BattleAction.SwitchMonster));
            _runButton.onClick.AddListener(() => OnActionSelected(BattleAction.Run));
        }

        private void OnDisable()
        {
            GameSignals.BATTLE_MOVE_UPDATE.RemoveListener(SetMoveList);
            _fightButton.onClick.RemoveListener(() => OnActionSelected(BattleAction.Move));
            _itemButton.onClick.RemoveListener(() => OnActionSelected(BattleAction.UseItem));
            _partyButton.onClick.RemoveListener(() => OnActionSelected(BattleAction.SwitchMonster));
            _runButton.onClick.RemoveListener(() => OnActionSelected(BattleAction.Run));
            _controls.Disable();
        }

        /// <summary>
        /// Teletype the dialog one character at a time.
        /// </summary>
        /// <param name="dialog">Dialog to type.</param>
        /// <returns>Typed dialog.</returns>
        public IEnumerator TypeDialog(string dialog)
        {
            // We first update the localizer key and force TMP to update.
            _dialogLocalizer.ChangeKey(dialog);
            _dialogText.ForceMeshUpdate();
            
            // Get the character count and initialize the counter.
            int totalVisibleCharacters = _dialogText.textInfo.characterCount;
            var counter = 0;

            while (true)
            {
                // Display the next invisible character.
                int visibleCount = counter % (totalVisibleCharacters + 1);
                _dialogText.maxVisibleCharacters = visibleCount;

                if (visibleCount >= totalVisibleCharacters)
                {
                    // Finished displaying all dialog. Wait until the player has finished reading to break out.
                    yield return new WaitUntil(() => _interact.triggered);
                    yield break;
                }
                
                // Increment the counter and wait a fraction of a second.
                counter += 1;
                yield return YieldHelper.TYPING_TIME;
            }
        }

        /// <summary>
        /// Handle the action selection state.
        /// </summary>
        public void ActionSelection()
        {
            // Enable the action selector and wait for an onClick event.
            _actionSelector.SetActive(true);
            _dialogLocalizer.ChangeKey("BATTLE_ACTION_SELECT");
            _state = BattleState.ActionSelection;
        }
        
        /// <summary>
        /// Gets the move list for the player monster from the BattleController.
        /// </summary>
        /// <param name="moves">List of moves the current monster has.</param>
        public void GetMoveList(List<MoveObj> moves)
        {
            _moves = moves;
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

        private void MoveSelection()
        {
            // Enable the move selector and wait for an onClick event.
            _dialogText.gameObject.SetActive(false);
            _actionSelector.SetActive(false);
            _moveSelector.SetActive(true);
            _moveDetails.SetActive(true);
            SetMoveList(true);
            _state = BattleState.MoveSelection;
        }

        private void SetMoveList(bool updated)
        {
            for (var i = 0; i < _moveButtons.Count; i++)
            {
                if (i > _moves.Count)
                {
                    return;
                }

                _currentMove = i;
                var localizer = _moveButtons[i].GetComponentInChildren<TextLocalizer>();
                localizer.ChangeKey(MOVE_KEY_PREFIX + _moves[i].Base.MoveName);
                _moveButtons[i].onClick.AddListener(() => OnMoveSelected(_currentMove));
            }
        }
        
        private void ClearMoveList()
        {
            for (var i = 0; i < _moveButtons.Count; i++)
            {
                _moveButtons[i].onClick.RemoveListener(() => OnMoveSelected(_currentMove));
            }
        }

        private void OnActionSelected(BattleAction action)
        {
            switch (action)
            {
                case BattleAction.Move:
                    MoveSelection();
                    break;
                case BattleAction.UseItem:
                    // Open inventory
                    break;
                case BattleAction.SwitchMonster:
                    GameSignals.PARTY_OPEN.Dispatch(true);
                    break;
                case BattleAction.Run:
                    // Run
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, "Invalid BattleAction.");
            }
        }

        private void OnMoveSelected(int moveNumber)
        {
            if (_state != BattleState.ForgetSelection && _state != BattleState.MoveSelection)
            {
                return;
            }
         
            GameSignals.BATTLE_MOVE_SELECT.Dispatch(new BattleMove(_state, moveNumber));
        }
    }
}