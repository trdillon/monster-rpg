using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Animation
{
    /// <summary>
    /// This class creates an animation object that is used to build animations.
    /// </summary>
    public class SpriteAnimator
    {
        private SpriteRenderer _spriteRenderer;
        private List<Sprite> _frames;
        private float _frameRate;
        private float _timer;
        private int _currentFrame;
        
        /// <summary>
        /// Constructor for a SpriteAnimator.
        /// </summary>
        /// <param name="frames">List of Sprite frames to animate.</param>
        /// <param name="spriteRenderer">The SpriteRenderer to pass to the SpriteAnimator.</param>
        /// <param name="frameRate">Animation frameRate, default is 0.16f.</param>
        public SpriteAnimator(List<Sprite> frames, SpriteRenderer spriteRenderer, float frameRate = 0.16f)
        {
            _frames = frames;
            _spriteRenderer = spriteRenderer;
            _frameRate = frameRate;
        }

        /// <summary>
        /// List of frames in the animation.
        /// </summary>
        public List<Sprite> Frames => _frames;

        /// <summary>
        /// Reset the SpriteAnimator to the initial state.
        /// </summary>
        public void Reset()
        {
            _currentFrame = 0;
            _timer = 0;
            _spriteRenderer.sprite = _frames[0];
        }

        /// <summary>
        /// Play the animation.
        /// </summary>
        public void PlayAnimation()
        {
            _timer += Time.deltaTime;
            if (!(_timer > _frameRate))
            {
                return;
            }

            // Loop back after the last frame.
            _currentFrame = (_currentFrame + 1) % _frames.Count;
            _spriteRenderer.sprite = _frames[_currentFrame];
            _timer -= _frameRate;
        }
    }
}
