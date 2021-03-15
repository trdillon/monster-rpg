using Itsdits.Ravar.Animation;
using Itsdits.Ravar.Levels;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Itsdits.Ravar.Character.Player
{
    public class PlayerController : MonoBehaviour, IMoveable, IInteractable
    {
        [Header("Details")]
        [SerializeField] string _name;
        [SerializeField] Sprite battleSprite;
        [SerializeField] float moveSpeed;

        private Vector2 inputVector;

        public string Name => _name;
        public Sprite BattleSprite => battleSprite;
        public float MoveSpeed => moveSpeed;
        public bool IsMoving { get; private set; }

        public event Action OnEncounter;
        public event Action<Collider2D> OnLoS;

        //TODO - refactor these dependencies. How to make the animator decoupled?
        private CharacterAnimator animator;
        public CharacterAnimator Animator => animator;
        private void Awake()
        {
            animator = GetComponent<CharacterAnimator>();
        }

        /// <summary>
        /// Handle Update() when state = GameState.World.
        /// </summary>
        public void HandleUpdate()
        {
            StartCoroutine(Move(inputVector, CheckAfterMove));
            animator.IsMoving = IsMoving;
        }

        /// <summary>
        /// Updates the moveVector when movement input is received.
        /// </summary>
        /// <param name="input">Input from the Player Input component.</param>
        public void OnMove(InputValue input)
        {
            inputVector = input.Get<Vector2>();
        }

        /// <summary>
        /// Calls Interact when interact input is received.
        /// </summary>
        public void OnInteract()
        {
           Interact(transform);
        }

        /// <summary>
        /// Move the Player character.
        /// </summary>
        /// <param name="moveVector">Where to move the Player to.</param>
        /// <param name="OnMoveFinish">What to do after the move is finished.</param>
        /// <returns></returns>
        public IEnumerator Move(Vector2 moveVector, Action OnMoveFinish)
        {
            if (inputVector != Vector2.zero)
            {
                if (!IsMoving)
                {
                    // Prevent diagonal movement.
                    if (moveVector.x != 0)
                    {
                        moveVector.y = 0;
                    }

                    // Normalize the Vector2 to avoid inputs less than 1f.
                    Vector2 moveNormalized = moveVector.normalized;

                    animator.MoveX = Mathf.Clamp(moveNormalized.x, -1f, 1f);
                    animator.MoveY = Mathf.Clamp(moveNormalized.y, -1f, 1f);

                    var targetPos = transform.position;
                    targetPos.x += moveNormalized.x;
                    targetPos.y += moveNormalized.y;

                    if (!IsPathWalkable(targetPos))
                    {
                        yield break;
                    }

                    IsMoving = true;

                    while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                        yield return null;
                    }

                    transform.position = targetPos;
                    IsMoving = false;
                    OnMoveFinish?.Invoke();
                }
            }
        }

        /// <summary>
		/// Change the direction the Player is facing. Used when interacted with to acknowledge the interaction.
		/// </summary>
		/// <param name="targetPos">The location to turn the Player towards.</param>
        public void ChangeDirection(Vector3 targetPos)
        {
            var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
            var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

            if (xdiff == 0 | ydiff == 0)
            {
                animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
                animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
            }
        }

        /// <summary>
        /// Interact with a character or object.
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

        private bool IsPathWalkable(Vector3 targetPath)
        {
            var path = targetPath - transform.position;
            var direction = path.normalized;
            // Start at the next tile over or we collide with the character.
            var origin = transform.position + direction;
            // 1 less because we start checking from the next tile over.
            var length = path.magnitude - 1;

            if (Physics2D.BoxCast(origin, new Vector2(0.2f, 0.2f), 0f, direction, length,
                MapLayers.Instance.ObjectsLayer | MapLayers.Instance.InteractLayer | MapLayers.Instance.PlayerLayer) == true)
            {
                // We found a collider on the path.
                return false;
            }

            return true;
        }
    }
}