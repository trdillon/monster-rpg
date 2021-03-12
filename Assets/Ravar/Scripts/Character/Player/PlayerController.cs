using Itsdits.Ravar.Levels;
using System;
using UnityEngine;

namespace Itsdits.Ravar.Character.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] string _name;
        [SerializeField] Sprite sprite;

        private Character character;
        private Vector2 input;

        public string Name => _name;
        public Sprite Sprite => sprite;
        public Character Character => character;

        public event Action OnEncounter;
        public event Action<Collider2D> OnLoS;

        private void Awake()
        {
            character = GetComponent<Character>();
        }

        /// <summary>
        /// Handle user input.
        /// </summary>
        public void HandleUpdate()
        {
            if (!character.IsMoving)
            {
                input.x = Input.GetAxisRaw("Horizontal");
                input.y = Input.GetAxisRaw("Vertical");

                // prevent diagonal movement
                if (input.x != 0)
                {
                    input.y = 0;
                }
                
                if (input != Vector2.zero)
                {
                    StartCoroutine(character.Move(input, CheckAfterMove));
                }
            }

            character.HandleUpdate();
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Interact();
            }
        }

        private void Interact()
        {
            var lookingAt = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
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

        private void CheckForEncounters()
        {
            if (Physics2D.OverlapCircle(transform.position - new Vector3(0, 0.3f), 0.2f, MapLayers.Instance.EncountersLayer) != null)
            {
                if (UnityEngine.Random.Range(1, 101) <= 7)
                {
                    character.Animator.IsMoving = false;
                    OnEncounter();
                }
            }
        }

        private void CheckForBattlers()
        {
            var collider = Physics2D.OverlapCircle(transform.position - new Vector3(0, 0.3f), 0.2f, MapLayers.Instance.LosLayer);

            if (collider != null)
            {
                character.Animator.IsMoving = false;
                OnLoS?.Invoke(collider);
            }
        }
    }
}