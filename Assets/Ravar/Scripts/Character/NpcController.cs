using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Itsdits.Ravar.Core.Signal;
using UnityEngine;

namespace Itsdits.Ravar.Character
{
    /// <summary>
    /// Controller class for NPC characters. Handles dialog and movement patterns.
    /// </summary>
    public class NpcController : Moveable, IInteractable
    {
        [Header("Details")]
        [Tooltip("Name of this NPC.")]
        [SerializeField] private string _name;

        [Header("Dialog")]
        [Tooltip("Dialog this NPC will display when interacted with.")]
        [SerializeField] private List<string> _dialog;
        [Tooltip("Dialog this NPC will display relating to Quests it is involved with.")]
        [SerializeField] private List<string> _questDialog;

        [Header("Movement")]
        [Tooltip("List of movements this character will make to complete their movement pattern.")]
        [SerializeField] private List<Vector2> _movementPattern;
        [Tooltip("Delay, in seconds, between each movement in the movement pattern.")]
        [SerializeField] private float _movementPatternDelay;

        private NpcState _state;
        private float _idleTimer;
        private int _currentMovement;

        // NPC should have a (Dictionary<Quest, int>)? to track which Quests they are involved in and the current stage
        // of the Quest the player is on.

        /// <summary>
        /// Name of this NPC.
        /// </summary>
        public string Name => _name;

        private void OnEnable()
        {
            GameSignals.DIALOG_FINISH.AddListener(OnDialogFinish);
        }

        private void OnDisable()
        {
            GameSignals.DIALOG_FINISH.RemoveListener(OnDialogFinish);
        }

        private void Update()
        {
            // We don't need to track idle time if there's no movement pattern. If the pattern is only 1 move then we
            // don't track it either, because that NPC doesn't have an actual pattern, just a linear path until it
            // collides with something.
            if (_movementPattern.Count < 2)
            {
                return;
            }
            
            // Start the idle counter.
            if (_state == NpcState.Idle)
            {
                _idleTimer += Time.deltaTime;
                // Pattern delay time is reached. Time to move.
                if (_idleTimer > _movementPatternDelay)
                {
                    _idleTimer = 0f;
                    StartCoroutine(WalkPattern());
                }
            }

            animator.IsMoving = IsMoving;
        }

        /// <summary>
        /// Interacts with a character or object.
        /// </summary>
        /// <param name="interactingCharacter">Who or what to interact with.</param>
        public void InteractWith(Transform interactingCharacter)
        {
            if (_state != NpcState.Idle)
            {
                return;
            }

            _state = NpcState.Interacting;
            ChangeDirection(interactingCharacter.position);
            if (_dialog.Count > 0)
            {
                var dialog = new DialogItem(_dialog.ToArray(), _name);
                GameSignals.DIALOG_OPEN.Dispatch(true);
                GameSignals.DIALOG_SHOW.Dispatch(dialog);
            }
            else
            {
                // Skip the call to ShowDialog() because it would cause an exception.
                _idleTimer = 0f;
                _state = NpcState.Idle;
            }
        }

        private void OnDialogFinish(string npcName)
        {
            if (npcName != _name)
            {
                return;
            }

            _idleTimer = 0f;
            _state = NpcState.Idle;
        }

        private IEnumerator WalkPattern()
        {
            _state = NpcState.Walking;
            Vector3 oldPos = transform.position;
            yield return Move(_movementPattern[_currentMovement], null);
            if (transform.position != oldPos)
            {
                // Loop back after the last move.
                _currentMovement = (_currentMovement + 1) % _movementPattern.Count;
            }

            _state = NpcState.Idle;
        }
    }
}