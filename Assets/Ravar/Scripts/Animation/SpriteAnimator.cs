using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Animation
{
    /// <summary>
    /// This class creates an animation object that is used to build animations.
    /// </summary>
    public class SpriteAnimator
    {
        private int currentFrame;
        private float frameRate;
        private float timer;
        private List<Sprite> frames;
        private SpriteRenderer spriteRenderer;
        
        /// <summary>
        /// Constructor for a SpriteAnimator.
        /// </summary>
        /// <param name="frames">List<Sprite> of frames to animate.</param>
        /// <param name="spriteRenderer">A SpriteRenderer reference.</param>
        /// <param name="frameRate">Animation frameRate, default is 0.16f.</param>
        public SpriteAnimator(List<Sprite> frames, SpriteRenderer spriteRenderer, float frameRate = 0.16f)
        {
            this.frames = frames;
            this.spriteRenderer = spriteRenderer;
            this.frameRate = frameRate;
        }

        public List<Sprite> Frames => frames;

        /// <summary>
        /// Initialize the SpriteAnimator.
        /// </summary>
        public void Start()
        {
            currentFrame = 0;
            timer = 0;
            spriteRenderer.sprite = frames[0];
        }

        /// <summary>
        /// Handle the sprite switching for frames.
        /// </summary>
        public void HandleUpdate()
        {
            timer += Time.deltaTime;
            if (timer > frameRate)
            {
                // Loop back after the last frame
                currentFrame = (currentFrame + 1) % frames.Count;
                spriteRenderer.sprite = frames[currentFrame];
                timer -= frameRate;
            }
        }
    }
}
