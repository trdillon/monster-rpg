using Itsdits.Ravar.Levels;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Itsdits.Ravar.Character.Player
{
    /// <summary>
    /// Controller for the Player character. Handles input and encounters.
    /// </summary>
    public class PlayerController : Moveable
    {
        [Header("Details")]
        [SerializeField] string _name;
        [SerializeField] Sprite battleSprite;

        private Vector2 inputVector;
        private Vector2 moveVector;

        public string Name => _name;
        public Sprite BattleSprite => battleSprite;

        public event Action OnEncounter;
        public event Action<Collider2D> OnLoS;

        /// <summary>
        /// Handles Update lifecycle when GameState.World.
        /// </summary>
        public void HandleUpdate()
        {
            HandleInput(inputVector);
            animator.IsMoving = IsMoving;
        }

        /// <summary>
        /// Update inputVector when Move event is triggered.
        /// </summary>
        /// <param name="context">Callbacks from the InputAction cycle. Contains the Vector2 from the Player Input.</param>
        public void OnMove(InputAction.CallbackContext context)
        {
            inputVector = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// Call Interact when Interact event is triggered.
        /// </summary>
        /// <param name="context">Callbacks from the InputAction cycle.</param>
        public void OnInteract(InputAction.CallbackContext context)
        {
            // Avoid calling during start and canceled callbacks to prevent duplicate calls.
            if (context.performed)
            {
                Interact();
            }
        }

        private void CheckAfterMove()
        {
            CheckForEncounters();
            CheckForBattlers();
        }

        private void CheckForBattlers()
        {
            var collider = Physics2D.OverlapCircle(transform.position - new Vector3(0, 0.3f), 0.2f, MapLayers.Instance.LosLayer);
            if (collider != null)
            {
                animator.IsMoving = false;
                OnLoS?.Invoke(collider);
            }
        }

        private void CheckForEncounters()
        {
            if (Physics2D.OverlapCircle(transform.position - new Vector3(0, 0.3f), 0.2f, MapLayers.Instance.EncountersLayer) != null)
            {
                if (UnityEngine.Random.Range(1, 101) <= 7)
                {
                    animator.IsMoving = false;
                    OnEncounter();
                }
            }
        }

        private void HandleInput(Vector2 inputVector)
        {
            if (inputVector != Vector2.zero)
            {
                if (!IsMoving)
                {
                    if (inputVector.x != 0)
                    {
                        inputVector.y = 0;
                    }

                    // Normalize the Vector2 to avoid inputs less than 1f.
                    moveVector = inputVector.normalized;
                    StartCoroutine(Move(moveVector, CheckAfterMove));
                }
            }
        }

        private void Interact()
        {
            var lookingAt = new Vector3(animator.MoveX, animator.MoveY);
            var nextTile = transform.position + lookingAt;
            var collider = Physics2D.OverlapCircle(nextTile, 0.3f, MapLayers.Instance.InteractLayer);
            if (collider != null)
            {
                collider.GetComponent<IInteractable>()?.InteractWith(transform);
            }
        }
    }
}