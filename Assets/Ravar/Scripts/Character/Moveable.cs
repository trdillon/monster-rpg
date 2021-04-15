using Itsdits.Ravar.Animation;
using Itsdits.Ravar.Levels;
using System;
using System.Collections;
using Itsdits.Ravar.Core.Signal;
using UnityEngine;

namespace Itsdits.Ravar.Character
{
	/// <summary>  
	/// Abstract class for moveable characters and objects.
	/// </summary>
	public abstract class Moveable : MonoBehaviour
	{
        protected CharacterAnimator animator;
        private float moveSpeed = 5f;

        /// <summary>
        /// If the character is currently moving or not.
        /// </summary>
        protected bool IsMoving { get; private set; }

        private void Awake()
        {
            animator = GetComponent<CharacterAnimator>();
            SetOffsetOnTile(transform.position);
        }

        protected IEnumerator Move(Vector2 moveVector, Action onMoveFinish)
        {
            animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
            animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

            Vector3 targetPos = transform.position;
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
            onMoveFinish?.Invoke();
        }

        /// <summary>
        /// Changes the direction the character or object is facing.
        /// </summary>
        /// <remarks>Used to acknowledge interaction or set default facing directions.</remarks>
        /// <param name="targetPos">Location of the tile the character or object should face to.</param>
        public void ChangeDirection(Vector3 targetPos)
        {
            Vector3 position = transform.position;
            float xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(position.x);
            float ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(position.y);

            if (!((xdiff == 0) | (ydiff == 0)))
            {
                return;
            }

            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }

        /// <summary>
        /// Ensures that characters have consistent positioning relative to the tile grid.
        /// </summary>
        /// <param name="position">Position of the Moveable character to re-position.</param>
        public void SetOffsetOnTile(Vector2 position)
        {
            position.x = Mathf.Floor(position.x) + 0.5f;
            position.y = Mathf.Floor(position.y) + 0.4f;
            transform.position = position;
        }

        private bool IsPathWalkable(Vector3 targetPath)
        {
            Vector3 position = transform.position;
            Vector3 path = targetPath - position;
            Vector3 direction = path.normalized;
            // Start at the next tile over or we collide with the character.
            Vector3 origin = position + direction;
            // 1 less because we start checking from the next tile over.
            float length = path.magnitude - 1;

            return Physics2D.BoxCast(origin, new Vector2(0.2f, 0.2f), 0f, direction, length,
                                     MapLayers.Instance.ObjectsLayer | 
                                     MapLayers.Instance.InteractLayer | 
                                     MapLayers.Instance.PlayerLayer) != true;
        }
    }
}
