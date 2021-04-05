using Itsdits.Ravar.Character;
using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Animation
{
    /// <summary>
    /// Animates the character sprites.
    /// </summary>
    public class CharacterAnimator : MonoBehaviour
    {
        [Tooltip("The default direction this character will be facing when the game is started.")]
        [SerializeField] Direction defaultDirection = Direction.Down;

        [Header("Sprites")]
        [Tooltip("Sprites to animate this character walking downwards.")]
        [SerializeField] List<Sprite> walkDownSprites;
        [Tooltip("Sprites to animate this character walking to the left.")]
        [SerializeField] List<Sprite> walkLeftSprites;
        [Tooltip("Sprites to animate this character walking to the right.")]
        [SerializeField] List<Sprite> walkRightSprites;
        [Tooltip("Sprites to animate this character walking upwards.")]
        [SerializeField] List<Sprite> walkUpSprites;

        private SpriteAnimator walkDownAnimation;
        private SpriteAnimator walkLeftAnimation;
        private SpriteAnimator walkRightAnimation;
        private SpriteAnimator walkUpAnimation;

        private SpriteAnimator currentAnimation;
        private SpriteRenderer spriteRenderer;
        private bool wasMoving;

        /// <summary>
        /// Input value on the X axis.
        /// </summary>
        /// <remarks>Range is -1 to 1.</remarks>
        public float MoveX { get; set; }
        /// <summary>
        /// Input value on the Y axis.
        /// </summary>
        /// <remarks>Range is -1 to 1.</remarks>
        public float MoveY { get; set; }
        /// <summary>
        /// Is the character currently moving or not.
        /// </summary>
        /// <remarks>Used to determine whether to animate the character sprite or not.</remarks>
        public bool IsMoving { get; set; }
        /// <summary>
        /// Default direction this character will face when the game is started.
        /// </summary>
        public Direction DefaultDirection => defaultDirection;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            walkDownAnimation = new SpriteAnimator(walkDownSprites, spriteRenderer);
            walkLeftAnimation = new SpriteAnimator(walkLeftSprites, spriteRenderer);
            walkRightAnimation = new SpriteAnimator(walkRightSprites, spriteRenderer);
            walkUpAnimation = new SpriteAnimator(walkUpSprites, spriteRenderer);
            SetDirection(defaultDirection);
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
        /// Set the character's default facing direction.
        /// </summary>
        /// <remarks>Mostly used for Battler's LoS.</remarks>
        /// <param name="direction">Down, Up, Left, Right</param>
        public void SetDirection(Direction direction)
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