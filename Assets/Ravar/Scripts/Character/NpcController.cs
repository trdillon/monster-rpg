using System.Collections;
using System.Collections.Generic;
using Itsdits.Ravar.UI.Dialog;
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
        [SerializeField] private DialogObj _dialog;

        [Header("Movement")]
        [Tooltip("List of movements this character will make to complete their movement pattern.")]
        [SerializeField] private List<Vector2> _movementPattern;
        [Tooltip("Delay, in seconds, between each movement in the movement pattern.")]
        [SerializeField] private float _movementPatternDelay;

        private NpcState _state;
        private float _idleTimer;
        private int _currentMovement;

        /// <summary>
        /// Name of this NPC.
        /// </summary>
        public string Name => _name;

        private void Update()
        {
            if (_state == NpcState.Idle)
            {
                _idleTimer += Time.deltaTime;
                if (_idleTimer > _movementPatternDelay)
                {
                    _idleTimer = 0f;
                    if (_movementPattern.Count > 0)
                    {
                        StartCoroutine(WalkPattern());
                    }  
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
            if (_dialog.Strings.Count > 0)
            {
                StartCoroutine(DialogController.Instance.ShowDialog(_dialog, Name, () =>
                {
                    _idleTimer = 0f;
                    _state = NpcState.Idle;
                }));
            }
            else
            {
                // Skip the call to ShowDialog() because it would cause an exception.
                _idleTimer = 0f;
                _state = NpcState.Idle;
            }
        }

        private IEnumerator WalkPattern()
        {
            _state = NpcState.Walking;
            Vector3 oldPos = transform.position;
            yield return Move(_movementPattern[_currentMovement], null);
            if (transform.position != oldPos)
            {
                // Loop back after the last move
                _currentMovement = (_currentMovement + 1) % _movementPattern.Count;
            }

            _state = NpcState.Idle;
        }
    }
}