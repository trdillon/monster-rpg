using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Animation
{
    public class SpriteAnimator
    {
        private int currentFrame;
        private float frameRate;
        private List<Sprite> frames;
        private SpriteRenderer spriteRenderer;
        private float timer;

        public SpriteAnimator(List<Sprite> frames, SpriteRenderer spriteRenderer, float frameRate = 0.16f)
        {
            this.frames = frames;
            this.spriteRenderer = spriteRenderer;
            this.frameRate = frameRate;
        }

        public List<Sprite> Frames => frames;

        /// <summary>
        /// Initialize the SpriteAnimator values at 0.
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
                currentFrame = (currentFrame + 1) % frames.Count;
                spriteRenderer.sprite = frames[currentFrame];
                timer -= frameRate;
            }
        }
    }
}
