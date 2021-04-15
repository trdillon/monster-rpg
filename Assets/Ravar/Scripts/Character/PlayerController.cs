using Itsdits.Ravar.Core;
using Itsdits.Ravar.Core.Signal;
using Itsdits.Ravar.Data;
using Itsdits.Ravar.Levels;
using Itsdits.Ravar.Settings;
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
        [SerializeField] private string _id;
        [Tooltip("The name of this player.")]
        [SerializeField] private string _name;
        [Tooltip("The sprite of the character to be displayed in the battle screen.")]
        [SerializeField] private Sprite _battleSprite;
        
        private PlayerControls _controls;
        private InputAction _move;
        private InputAction _interact;
        private InputAction _pause;
        private Vector2 _inputVector;

        /// <summary>
        /// The player's Id.
        /// </summary>
        public string Id => _id;
        /// <summary>
        /// The player's name.
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// The character sprite to be used in the battle screen.
        /// </summary>
        public Sprite BattleSprite => _battleSprite;

        private void Start()
        {
            if (_id.Equals(null))
            {
                _id = _name + Random.Range(0, 65534);
            }
            GameSignals.UNPAUSE_GAME.AddListener(OnResume);
        }

        private void OnEnable()
        {
            _controls = new PlayerControls();
            _controls.Enable();
            _move = _controls.Player.Move;
            _interact = _controls.Player.Interact;
            _pause = _controls.Player.Pause;
            _move.performed += OnMove;
            _interact.performed += OnInteract;
            _pause.performed += OnPause;
        }

        private void OnDisable()
        {
            _move.performed -= OnMove;
            _interact.performed -= OnInteract;
            _pause.performed -= OnPause;
            _controls.Disable();
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
            _inputVector = context.ReadValue<Vector2>();
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            var lookingAt = new Vector3(animator.MoveX, animator.MoveY);
            Vector3 nextTile = transform.position + lookingAt;
            Collider2D overlapCircle = Physics2D.OverlapCircle(nextTile, 0.3f, MapLayers.Instance.InteractLayer);
            if (overlapCircle != null)
            {
                overlapCircle.GetComponent<IInteractable>()?.InteractWith(transform);
            }
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            StartCoroutine(GameController.Instance.PauseGame(true));
        }

        private void OnResume(bool resume)
        {
            if (resume)
            {
                GetComponent<SpriteRenderer>().enabled = true;
                GetComponentInChildren<AudioListener>().enabled = true;
            }
        }

        /// <summary>
        /// Handles Update lifecycle when GameState.World.
        /// </summary>
        public void HandleUpdate()
        {
            HandleInput(_inputVector);
            animator.IsMoving = IsMoving;
        }
        
        /// <summary>
        /// Saves the current player's data.
        /// </summary>
        /// <returns>PlayerData with current data.</returns>
        public PlayerData SavePlayerData()
        {
            GameController.Instance.UpdateCurrentScene();

            var playerData = new PlayerData(
                _id,
                GameController.Instance.CurrentSceneName,
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
            _id = loadData.id;
            GameController.Instance.UpdateCurrentScene();
            string currentScene = GameController.Instance.CurrentSceneName;
            if (loadData.currentScene != currentScene)
            {
                StartCoroutine(GameController.Instance.LoadScene(loadData.currentScene));
            }

            var newPosition = new Vector2(loadData.currentPosition[0], loadData.currentPosition[1]);
            SetOffsetOnTile(newPosition);
        }

        private void CheckAfterMove()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, 0.3f), 0.2f, MapLayers.Instance.ActionLayers);

            foreach (Collider2D c in colliders)
            {
                var trigger = c.GetComponent<ITriggerable>();
                if (trigger == null)
                {
                    continue;
                }

                animator.IsMoving = false;
                trigger.OnTriggered(this);
                break;
            }
        }

        private void HandleInput(Vector2 inputVector)
        {
            // Return if no input or character is already moving.
            if (inputVector == Vector2.zero)
            {
                return;
            }

            if (IsMoving)
            {
                return;
            }

            // Only x or y should have a non-zero value to prevent diagonal movement.
            if (inputVector.x != 0)
            {
                inputVector.y = 0;
            }

            // Normalize the Vector2 because the composite mode on the input bindings can cause != 1f inputs.
            Vector2 moveVector = inputVector.normalized;
            StartCoroutine(Move(moveVector, CheckAfterMove));
        }

        private int[] GetPositionAsIntArray()
        {
            // We save the player's position as an int array because it's easy to serialize for save data.
            // After loading the position we create a new Vector2 from the array and call SetOffsetOnTile() on it
            // to put the player on the correct position of the tile.
            Vector3 position = transform.position;
            var positionAsArray = new int[2];
            positionAsArray[0] = Mathf.FloorToInt(position.x);
            positionAsArray[1] = Mathf.FloorToInt(position.y);

            return positionAsArray;
        }
    }
}