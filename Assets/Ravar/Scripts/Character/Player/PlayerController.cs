using Itsdits.Ravar.Core;
using Itsdits.Ravar.Levels;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Itsdits.Ravar.Character
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

        /// <summary>
        /// Pause the game when the Pause event is triggered.
        /// </summary>
        /// <param name="context">Callbacks from the InputAction cycle.</param>
        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                GameController.Instance.PauseGame(true);
            }
        }

        private void CheckAfterMove()
        {
            var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, 0.3f), 0.2f, MapLayers.Instance.ActionLayers);

            foreach (var collider in colliders)
            {
                var trigger = collider.GetComponent<ITriggerable>();
                if (trigger != null)
                {
                    animator.IsMoving = false;
                    trigger.OnTriggered(this);
                    break;
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