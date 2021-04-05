using Itsdits.Ravar.Core;
using Itsdits.Ravar.Data;
using Itsdits.Ravar.Levels;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Itsdits.Ravar.Character
{
    /// <summary>
    /// Controller for the Player character that implements <see cref="Moveable"/>. Handles input and interaction.
    /// </summary>
    public class PlayerController : Moveable
    {
        [Header("Details")]
        [Tooltip("Unique ID for this player to differentiate between save games. Automatically generated at runtime.")]
        [SerializeField] string id;
        [Tooltip("The name of this player.")]
        [SerializeField] string _name;
        [Tooltip("The sprite of the character to be displayed in the battle screen.")]
        [SerializeField] Sprite battleSprite;

        private Vector2 inputVector;
        private Vector2 moveVector;

        /// <summary>
        /// The player's name.
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// The character sprite to be used in the battle screen.
        /// </summary>
        public Sprite BattleSprite => battleSprite;

        private void Start()
        {
            id = _name + Random.Range(0, 65534);
        }
        
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

        /// <summary>
        /// Saves the current player's data.
        /// </summary>
        /// <returns>PlayerData with current data.</returns>
        public PlayerData SavePlayerData()
        {
            GameController.Instance.UpdateCurrentScene();

            var playerData = new PlayerData(
                id,
                GameController.Instance.CurrentScene,
                GetPositionAsIntArray()
                );

            return playerData;
        }

        /// <summary>
        /// Loads the player data into the current player.
        /// </summary>
        /// <param name="loadData">PlayerData to load into this player.</param>
        public void LoadPlayerData(PlayerData loadData)
        {
            id = loadData.id;
            GameController.Instance.UpdateCurrentScene();
            var currentScene = GameController.Instance.CurrentScene;

            if (loadData.currentScene != currentScene)
            {
                GameController.Instance.LoadScene(loadData.currentScene);
            }

            var newPosition = new Vector2(loadData.currentPosition[0], loadData.currentPosition[1]);
            SetOffsetOnTile(newPosition);
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

        private int[] GetPositionAsIntArray()
        {
            int[] positionAsArray = new int[2];
            positionAsArray[0] = Mathf.FloorToInt(transform.position.x);
            positionAsArray[1] = Mathf.FloorToInt(transform.position.y);

            return positionAsArray;
        }
    }
}