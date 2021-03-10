using Itsdits.Ravar.Character.Player;
using Itsdits.Ravar.UI;
using System.Collections;
using UnityEngine;

namespace Itsdits.Ravar.Character.Battler
{
    public class BattlerController : MonoBehaviour, IInteractable
    {
        [SerializeField] string _name;
        [SerializeField] Sprite sprite;
        [SerializeField] Dialog introDialog;
        [SerializeField] Dialog outroDialog;
        [SerializeField] GameObject alert;
        [SerializeField] GameObject los;

        private Character character;
        private bool isDefeated = false;

        public string Name => _name;
        public Sprite Sprite => sprite;

        private void Awake()
        {
            character = GetComponent<Character>();
        }

        private void Start()
        {
            RotateLoS(character.Animator.DefaultDirection);
        }

        private void Update()
        {
            character.HandleUpdate();
        }

        /// <summary>
        /// Interact with the player.
        /// </summary>
        /// <param name="interactChar">Player</param>
        public void Interact(Transform interactChar)
        {
            character.TurnToInteract(interactChar.position);

            if (!isDefeated)
            {
                StartCoroutine(DialogController.Instance.ShowDialog(introDialog, () =>
                {
                    GameController.Instance.StartCharBattle(this);
                }));
            }
            else
            {
                StartCoroutine(DialogController.Instance.ShowDialog(outroDialog));
            }  
        }

        /// <summary>
        /// Trigger a battle after player walks into LoS
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>Battle</returns>
        public IEnumerator TriggerBattle(PlayerController player)
        {
            // Show alert over head.
            alert.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            alert.gameObject.SetActive(false);

            // Move to the player.
            var path = player.transform.position - transform.position;
            // Stop next to the player, not on them.
            var tile = path - path.normalized;
            // Move value should always be int.
            tile = new Vector2(Mathf.Round(tile.x), Mathf.Round(tile.y));
            yield return character.Move(tile);

            // Show dialog for trash talk then start battle.
            player.Character.TurnToInteract(transform.position);
            yield return DialogController.Instance.ShowDialog(introDialog, () => {
                GameController.Instance.StartCharBattle(this);
            });
        }

        /// <summary>
        /// Rotate the LoS after movement.
        /// </summary>
        /// <param name="direction">Direction to rotate.</param>
        public void RotateLoS(DefaultDirection direction)
        {
            float angle = 0f;
            if (direction == DefaultDirection.Down)
            {
                angle = 0f;
            }
            else if (direction == DefaultDirection.Up)
            {
                angle = 180f;
            }
            else if (direction == DefaultDirection.Left)
            {
                angle = 270f;
            }
            else if (direction == DefaultDirection.Right)
            {
                angle = 90f;
            }

            los.transform.eulerAngles = new Vector3(0f, 0f, angle);
        }

        /// <summary>
        /// Set Battler to defeated mode.
        /// </summary>
        public void SetDefeated()
        {
            isDefeated = true;
            los.gameObject.SetActive(false);
            //TODO - have the battler return to its starting position
        }
    }
}