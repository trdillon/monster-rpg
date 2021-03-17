using Itsdits.Ravar.Character.Player;
using Itsdits.Ravar.UI;
using System.Collections;
using UnityEngine;

namespace Itsdits.Ravar.Character.Battler
{
    /// <summary>
    /// Controller class for Battler characters. Handles encounter triggers and line of sight.
    /// </summary>
    public class BattlerController : Moveable, IInteractable
    {
        [Header("Details")]
        [SerializeField] string _name;
        [SerializeField] Sprite sprite;
        [SerializeField] BattlerState state = BattlerState.Ready;

        [Header("Dialog")]
        [SerializeField] Dialog introDialog;
        [SerializeField] Dialog outroDialog;

        [Header("Line of Sight")]
        [SerializeField] GameObject alert;
        [SerializeField] GameObject los;

        public string Name => _name;
        public Sprite Sprite => sprite;
        public BattlerState State => state;

        private void Start()
        {
            SetBattlerState(state);
            RotateLoS(animator.DefaultDirection);
        }

        private void Update()
        {
            animator.IsMoving = IsMoving;
        }

        /// <summary>
        /// Trigger a battle after player walks into LoS.
        /// </summary>
        /// <param name="player">The player.</param>
        public IEnumerator TriggerBattle(PlayerController player)
        {
            // Show alert over head.
            alert.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            alert.gameObject.SetActive(false);

            // Move to the player, stopping 1 tile before them.
            var path = player.transform.position - transform.position;
            var tile = path - path.normalized;
            tile = new Vector2(Mathf.Round(tile.x), Mathf.Round(tile.y));
            yield return Move(tile, null);

            // Show dialog for trash talk then start battle.
            player.ChangeDirection(transform.position);
            if (introDialog.Strings.Count > 0) 
            {
                yield return DialogController.Instance.ShowDialog(introDialog, Name, () => 
                {
                    GameController.Instance.StartCharBattle(this);
                });
            }
            else
            {
                // An exception occurs if the Battler is missing dialog, so we call ReleasePlayer()
                // to set state = GameState.World. Otherwise the player is stuck on the crashed dialog box.
                GameController.Instance.ReleasePlayer();
            }
        }

        /// <summary>
        /// Rotate the line of sight for the Battler. Used to keep the BoxCollider facing the same direction as the Battler and
        /// to set a default facing direction for a Battler.
        /// </summary>
        /// <param name="direction">Direction to rotate.</param>
        public void RotateLoS(Direction direction)
        {
            float angle = 0f;
            if (direction == Direction.Down)
            {
                angle = 0f;
            }
            else if (direction == Direction.Up)
            {
                angle = 180f;
            }
            else if (direction == Direction.Left)
            {
                angle = 270f;
            }
            else if (direction == Direction.Right)
            {
                angle = 90f;
            }

            los.transform.eulerAngles = new Vector3(0f, 0f, angle);
        }

        /// <summary>
        /// Interact with the player.
        /// </summary>
        /// <param name="interactor">Who or what to interact with.</param>
        public void InteractWith(Transform interactor)
        {
            if (GameController.Instance.State == GameState.World) { 
                ChangeDirection(interactor.position);
                if (introDialog.Strings.Count > 0 && outroDialog.Strings.Count > 0)
                {
                    if (state == BattlerState.Ready)
                    {
                        StartCoroutine(DialogController.Instance.ShowDialog(introDialog, Name, () =>
                        {
                            GameController.Instance.StartCharBattle(this);
                        }));
                    }
                    else
                    {
                        StartCoroutine(DialogController.Instance.ShowDialog(outroDialog, Name));
                    }
                }
                else
                {
                    // An exception occurs if the Battler is missing dialog, so we call ReleasePlayer()
                    // to set state = GameState.World. Otherwise the player is stuck on the crashed dialog box.
                    GameController.Instance.ReleasePlayer();
                }
            }
        }

        /// <summary>
        /// Set the BattlerState of a Battler character.
        /// </summary>
        /// <param name="newState">Ready = ready to battle, Defeated = already defeated, 
        /// Locked = not able to battle yet, possibly need to finish a Quest first.</param>
        public void SetBattlerState(BattlerState newState)
        {
            state = newState;

            if (state == BattlerState.Ready)
            {
                los.gameObject.SetActive(true);
            }
            else if (state == BattlerState.Defeated)
            {
                los.gameObject.SetActive(false);
                //TODO - have the battler return to its starting position
            }
            else if (state == BattlerState.Locked)
            {
                los.gameObject.SetActive(false);
                //TODO - implement locked/later quest features
            }
        }
    }
}