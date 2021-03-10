using Itsdits.Ravar.Animation;
using Itsdits.Ravar.Levels;
using System;
using System.Collections;
using UnityEngine;

namespace Itsdits.Ravar.Character
{
    public class Character : MonoBehaviour
    {
        private CharacterAnimator animator;

        public float moveSpeed;

        public bool IsMoving { get; private set; }
        public CharacterAnimator Animator => animator;

        private void Awake()
        {
            animator = GetComponent<CharacterAnimator>();
        }

        /// <summary>
        /// Move the character.
        /// </summary>
        /// <param name="moveVector">Where the character will move to.</param>
        /// <param name="OnMoveFinish">What happens after moving.</param>
        /// <returns>OnMoveFinish</returns>
        public IEnumerator Move(Vector2 moveVector, Action OnMoveFinish = null)
        {
            animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
            animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

            var targetPos = transform.position;
            targetPos.x += moveVector.x;
            targetPos.y += moveVector.y;

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

        /// <summary>
        /// Handle updates to the CharacterAnimator.
        /// </summary>
        public void HandleUpdate()
        {
            animator.IsMoving = IsMoving;
        }

        /// <summary>
        /// Turn the character towards the character requesting interaction.
        /// </summary>
        /// <param name="targetPos">Where to turn to.</param>
        public void TurnToInteract(Vector3 targetPos)
        {
            var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
            var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

            if (xdiff == 0 | ydiff == 0)
            {
                animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
                animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
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