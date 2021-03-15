using Itsdits.Ravar.Character;
using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Animation
{
    public class CharacterAnimator : MonoBehaviour
    {
        #region config
        [SerializeField] Direction defaultDirection = Direction.Down;
        [SerializeField] List<Sprite> walkDownSprites;
        [SerializeField] List<Sprite> walkLeftSprites;
        [SerializeField] List<Sprite> walkRightSprites;
        [SerializeField] List<Sprite> walkUpSprites;

        private SpriteAnimator currentAnimation;
        private SpriteRenderer spriteRenderer;
        private SpriteAnimator walkDownAnimation;
        private SpriteAnimator walkLeftAnimation;
        private SpriteAnimator walkRightAnimation;
        private SpriteAnimator walkUpAnimation;
        private bool wasMoving;
        #endregion
        public float MoveX { get; set; }
        public float MoveY { get; set; }
        public bool IsMoving { get; set; }
        public Direction DefaultDirection => defaultDirection;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            walkDownAnimation = new SpriteAnimator(walkDownSprites, spriteRenderer);
            walkLeftAnimation = new SpriteAnimator(walkLeftSprites, spriteRenderer);
            walkRightAnimation = new SpriteAnimator(walkRightSprites, spriteRenderer);
            walkUpAnimation = new SpriteAnimator(walkUpSprites, spriteRenderer);
            SetDefaultDirection(defaultDirection);
            currentAnimation = walkDownAnimation;
        }

        private void Update()
        {
            var prevAnimation = currentAnimation;

            if (MoveY == -1)
            {
                currentAnimation = walkDownAnimation;
            }
            else if (MoveY == 1)
            {
                currentAnimation = walkUpAnimation;
            }
            else if (MoveX == -1)
            {
                currentAnimation = walkLeftAnimation;
            }
            else if (MoveX == 1)
            {
                currentAnimation = walkRightAnimation;
            }
                
            if (currentAnimation != prevAnimation || IsMoving != wasMoving)
            {
                currentAnimation.Start();
            }
                
            if (IsMoving)
            {
                currentAnimation.HandleUpdate();
            }  
            else
            {
                spriteRenderer.sprite = currentAnimation.Frames[0];
            }
                
            wasMoving = IsMoving;
        }

        /// <summary>
        /// Set the character's default facing direction. Mostly used for Battler's LoS.
        /// </summary>
        /// <param name="direction">Down, Left, Right, Up</param>
        public void SetDefaultDirection(Direction direction)
        {
            if (direction == Direction.Down)
            {
                MoveY = -1;
            }   
            else if (direction == Direction.Up)
            {
                MoveY = 1;
            }  
            else if (direction == Direction.Left)
            {
                MoveX = -1;
            }   
            else if (direction == Direction.Right)
            {
                MoveX = 1;
            }   
        }
    }
}