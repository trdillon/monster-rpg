using Itsdits.Ravar.Levels;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Itsdits.Ravar.Character.Player
{
    /// <summary>
    /// Controller for the Player character.
    /// </summary>
    public class PlayerController : Moveable, IInteractable
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
        /// Handle Update() when state = GameState.World. Used to sanitize the input to be passed to Move()
        /// and update the CharacterAnimator.
        /// </summary>
        public void HandleUpdate()
        {
            StartCoroutine(SanitizeInput(inputVector));
            animator.IsMoving = IsMoving;
        }

        /// <summary>
        /// Updates inputVector when input is received from the Player Input component.
        /// </summary>
        /// <param name="input">Input from the Player Input component.</param>
        public void OnMove(InputValue input)
        {
            inputVector = input.Get<Vector2>();
        }

        /// <summary>
        /// Calls Interact when interact input is received from the Player Input component.
        /// </summary>
        public void OnInteract()
        {
           Interact(transform);
        }

        /// <summary>
        /// Interacts with a character or object.
        /// </summary>
        /// <param name="interactWith">Who or what to interact with.</param>
        public void Interact(Transform interactWith)
        {
            var lookingAt = new Vector3(animator.MoveX, animator.MoveY);
            var nextTile = transform.position + lookingAt;
            var collider = Physics2D.OverlapCircle(nextTile, 0.3f, MapLayers.Instance.InteractLayer);

            if (collider != null)
            {
                collider.GetComponent<IInteractable>()?.Interact(transform);
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

        private IEnumerator SanitizeInput(Vector2 inputVector)
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
                    yield return Move(moveVector, CheckAfterMove);
                }
            }
        }
    }
}