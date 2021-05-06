using System;
using System.Collections;
using System.Collections.Generic;
using Itsdits.Ravar.Core.Signal;
using Itsdits.Ravar.Monster;
using Itsdits.Ravar.Util;
using UnityEngine;

namespace Itsdits.Ravar.Character
{
    /// <summary>
    /// Controller class for Battler characters. Handles encounter triggers and line of sight.
    /// </summary>
    public class BattlerController : Moveable, IInteractable
    {
        [Header("Details")]
        [Tooltip("Name of this Battler.")]
        [SerializeField] private string _name;
        [Tooltip("Sprite of this Battler.")]
        [SerializeField] private Sprite _sprite;
        [Tooltip("State of this battler. Default is Ready. LoS is disabled on Defeated and Locked states.")]
        [SerializeField] private BattlerState _state = BattlerState.Ready;
        [Tooltip("Starting position on the world grid.")]
        [SerializeField] private Vector2 _startingPos;

        [Header("Dialog")]
        [Tooltip("Dialog this Battler will display when BattlerState is Ready.")]
        [SerializeField] private List<string> _introDialog;
        [Tooltip("Dialog this Battler will display when BattlerState is Defeated.")]
        [SerializeField] private List<string> _outroDialog;

        [Header("Line of Sight")]
        [Tooltip("Alert icon to be displayed above the Battler's head when LoS is triggered.")]
        [SerializeField] private GameObject _alert;
        [Tooltip("GameObject representing this Battler's LoS.")]
        [SerializeField] private GameObject _los;

        private MonsterParty _party;
        private bool _isInteracting;

        /// <summary>
        /// Name of this Battler.
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// Sprite of this Battler.
        /// </summary>
        public Sprite Sprite => _sprite;
        /// <summary>
        /// Exposes the Battler's monster party.
        /// </summary>
        public MonsterParty Party => _party;

        private void Start()
        {
            SetBattlerState(_state);
            RotateLoS(animator.DefaultDirection);
            _party = GetComponent<MonsterParty>();
        }

        private void OnEnable()
        {
            GameSignals.DIALOG_CLOSE.AddListener(OnDialogClose);
        }

        private void OnDisable()
        {
            GameSignals.DIALOG_CLOSE.RemoveListener(OnDialogClose);
        }

        private void Update()
        {
            animator.IsMoving = IsMoving;
        }

        /// <summary>
        /// Trigger a battle after player walks into LoS.
        /// </summary>
        /// <param name="player">The player.</param>
        public IEnumerator TriggerEncounter(PlayerController player)
        {
            // Show alert over head.
            _alert.gameObject.SetActive(true);
            yield return YieldHelper.ONE_SECOND;
            _alert.gameObject.SetActive(false);

            // Move to the player, stopping 1 tile before them.
            Vector3 path = player.transform.position - transform.position;
            Vector3 targetTile = path - path.normalized;
            targetTile = new Vector2(Mathf.Round(targetTile.x), Mathf.Round(targetTile.y));
            yield return Move(targetTile, null);

            // Show dialog for trash talk then start battle.
            _isInteracting = true;
            player.ChangeDirection(transform.position);
            if (_introDialog.Count > 0) 
            {
                var dialog = new DialogItem(_introDialog.ToArray(), _name);
                GameSignals.DIALOG_OPEN.Dispatch(dialog);
            }
            else
            {
                // If we are missing intro dialog we lock the Battler and throw an exception. This should allow us to
                // gracefully recover and continue gameplay.
                _state = BattlerState.Locked;
                _isInteracting = false;
                throw new ArgumentException($"Null dialog in Battler. Battler: {_name}");
            }
        }
        
        /// <summary>
        /// Interact with the player.
        /// </summary>
        /// <param name="interactingCharacter">Who or what to interact with.</param>
        public void InteractWith(Transform interactingCharacter)
        {
            if (_isInteracting)
            {
                return;
            }
            
            _isInteracting = true;
            ChangeDirection(interactingCharacter.position);
            if (_introDialog.Count < 1 || _outroDialog.Count < 1)
            {
                // If we are missing intro or outro dialog we lock the Battler and throw an exception. This should
                // allow us to gracefully recover and continue gameplay.
                _state = BattlerState.Locked;
                _isInteracting = false;
                throw new ArgumentException($"Null dialog in Battler. Battler: {_name}");
            }
            
            if (_state == BattlerState.Ready)
            {
                var dialog = new DialogItem(_introDialog.ToArray(), _name);
                GameSignals.DIALOG_OPEN.Dispatch(dialog);
            }
            else
            {
                var dialog = new DialogItem(_outroDialog.ToArray(), _name);
                GameSignals.DIALOG_OPEN.Dispatch(dialog);
            }
        }

        /// <summary>
        /// Set the BattlerState of a Battler character.
        /// </summary>
        /// <param name="newState">New state to set the Battler to.</param>
        public void SetBattlerState(BattlerState newState)
        {
            _state = newState;
            if (_state == BattlerState.Ready)
            {
                _los.gameObject.SetActive(true);
            }
            else if (_state == BattlerState.Defeated)
            {
                _los.gameObject.SetActive(false);
                StartCoroutine(Move(_startingPos, null));
                animator.SetDirection(animator.DefaultDirection);
            }
            else if (_state == BattlerState.Locked)
            {
                _los.gameObject.SetActive(false);
                //TODO - implement locked/later quest features
            }
        }

        private void OnDialogClose(string speakerName)
        {
            if (speakerName != _name)
            {
                return;
            }

            if (_state != BattlerState.Ready)
            {
                return;
            }
            
            GameSignals.BATTLE_OPEN.Dispatch(new BattlerEncounter(this));
        }
        
        private void RotateLoS(Direction direction)
        {
            var angle = 0f;
            if (direction == Direction.Down)
            {
                angle = 0f;
            }
            else if (direction == Direction.Up)
            {
                angle = 180f;
            }
            else if (direction == Direction.Left)
            {
                angle = 270f;
            }
            else if (direction == Direction.Right)
            {
                angle = 90f;
            }

            _los.transform.eulerAngles = new Vector3(0f, 0f, angle);
        }
    }
}