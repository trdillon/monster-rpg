using Itsdits.Ravar.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Character.NPC { 
    public class NPCController : MonoBehaviour, IInteractable
    {
        [SerializeField] string _name;
        [SerializeField] Dialog dialog;
        [SerializeField] List<Vector2> movementPattern;
        [SerializeField] float movementPatternTime;

        private Character character;
        private NPCState state;
        private float idleTimer = 0f;
        private int currentMovement = 0;

        public string Name => _name;

        private void Awake()
        {
            character = GetComponent<Character>();
        }

        private void Update()
        {
            if (state == NPCState.Idle)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer > movementPatternTime)
                {
                    idleTimer = 0f;
                    if (movementPattern.Count > 0)
                    {
                        StartCoroutine(Walk());
                    }  
                }
            }
            character.HandleUpdate();
        }

        private IEnumerator Walk()
        {
            state = NPCState.Walking;

            var oldPos = transform.position;
            yield return character.Move(movementPattern[currentMovement]);

            if (transform.position != oldPos)
            {
                // Loop back after the last move
                currentMovement = (currentMovement + 1) % movementPattern.Count;
            }

            state = NPCState.Idle;
        }

        /// <summary>
        /// Interact with the player.
        /// </summary>
        /// <param name="interactChar">Player</param>
        public void Interact(Transform interactChar)
        {
            if (state == NPCState.Idle)
            {
                state = NPCState.Interacting;

                character.TurnToInteract(interactChar.position);
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
                    // Error if there's no dialog to show.
                    Debug.LogError($"NC001: {Name} doesn't have interaction dialog.");
                    idleTimer = 0f;

                    state = NPCState.Idle;
                }
            }
        }
    }
}