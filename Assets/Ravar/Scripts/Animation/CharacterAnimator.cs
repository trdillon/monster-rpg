using System;
using Itsdits.Ravar.Character;
using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Animation
{
    /// <summary>
    /// Creates character animations during world scenes.
    /// </summary>
    public class CharacterAnimator : MonoBehaviour
    {
        [Tooltip("The default direction this character will be facing when the game is started.")]
        [SerializeField] private Direction _defaultDirection = Direction.Down;

        [Header("Sprites")]
        [Tooltip("Sprites to animate this character walking downwards.")]
        [SerializeField] private List<Sprite> _walkDownSprites;
        [Tooltip("Sprites to animate this character walking to the left.")]
        [SerializeField] private List<Sprite> _walkLeftSprites;
        [Tooltip("Sprites to animate this character walking to the right.")]
        [SerializeField] private List<Sprite> _walkRightSprites;
        [Tooltip("Sprites to animate this character walking upwards.")]
        [SerializeField] private List<Sprite> _walkUpSprites;

        private SpriteAnimator _walkDownAnimation;
        private SpriteAnimator _walkLeftAnimation;
        private SpriteAnimator _walkRightAnimation;
        private SpriteAnimator _walkUpAnimation;
        private SpriteAnimator _currentAnimation;
        private SpriteRenderer _spriteRenderer;
        private bool _wasMoving;

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
        public Direction DefaultDirection => _defaultDirection;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _walkDownAnimation = new SpriteAnimator(_walkDownSprites, _spriteRenderer);
            _walkLeftAnimation = new SpriteAnimator(_walkLeftSprites, _spriteRenderer);
            _walkRightAnimation = new SpriteAnimator(_walkRightSprites, _spriteRenderer);
            _walkUpAnimation = new SpriteAnimator(_walkUpSprites, _spriteRenderer);
            SetDirection(_defaultDirection);
            _currentAnimation = _walkDownAnimation;
        }

        private void Update()
        {
            SpriteAnimator prevAnimation = _currentAnimation;

            if (MoveY == -1)
            {
                _currentAnimation = _walkDownAnimation;
            }
            else if (MoveY == 1)
            {
                _currentAnimation = _walkUpAnimation;
            }
            else if (MoveX == -1)
            {
                _currentAnimation = _walkLeftAnimation;
            }
            else if (MoveX == 1)
            {
                _currentAnimation = _walkRightAnimation;
            }
                
            if (_currentAnimation != prevAnimation || IsMoving != _wasMoving)
            {
                _currentAnimation.Reset();
            }
                
            if (IsMoving)
            {
                _currentAnimation.PlayAnimation();
            }
            else
            {
                _spriteRenderer.sprite = _currentAnimation.Frames[0];
            }

            _wasMoving = IsMoving;
        }

        /// <summary>
        /// Set the character's default facing direction.
        /// </summary>
        /// <remarks>Mostly used for Battler's LoS.</remarks>
        /// <param name="direction">Down, Up, Left, Right</param>
        public void SetDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    MoveY = -1;
                    break;
                case Direction.Up:
                    MoveY = 1;
                    break;
                case Direction.Left:
                    MoveX = -1;
                    break;
                case Direction.Right:
                    MoveX = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction,
                                                          "Direction invalid or null.");
            }
        }
    }
}