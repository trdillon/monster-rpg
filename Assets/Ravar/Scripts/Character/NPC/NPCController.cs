using Itsdits.Ravar.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Character.NPC { 
    public class NPCController : Moveable, IInteractable
    {
        [Header("Details")]
        [SerializeField] string _name;

        [Header("Dialog")]
        [SerializeField] Dialog dialog;

        [Header("Movement")]
        [SerializeField] List<Vector2> movementPattern;
        [SerializeField] float movementPatternDelay;

        private NPCState state;
        private float idleTimer = 0f;
        private int currentMovement = 0;

        public string Name => _name;

        private void Update()
        {
            if (state == NPCState.Idle)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer > movementPatternDelay)
                {
                    idleTimer = 0f;
                    if (movementPattern.Count > 0)
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
        /// <param name="interactWith">Who or what to interact with.</param>
        public void Interact(Transform interactChar)
        {
            if (state == NPCState.Idle)
            {
                state = NPCState.Interacting;

                ChangeDirection(interactChar.position);
                if (dialog.Strings.Count > 0)
                {
                    StartCoroutine(DialogController.Instance.ShowDialog(dialog, Name, () =>
                    {
                        idleTimer = 0f;

                        state = NPCState.Idle;
                    }));
                }
                else
                {
                    // Skip the call to ShowDialog() because it would cause an exception.
                    idleTimer = 0f;

                    state = NPCState.Idle;
                }
            }
        }

        private IEnumerator WalkPattern()
        {
            state = NPCState.Walking;

            var oldPos = transform.position;
            yield return Move(movementPattern[currentMovement], null);

            if (transform.position != oldPos)
            {
                // Loop back after the last move
                currentMovement = (currentMovement + 1) % movementPattern.Count;
            }

            state = NPCState.Idle;
        }
    }
}