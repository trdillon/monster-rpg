using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] DefaultDirection defaultDirection = DefaultDirection.Down;

    SpriteAnimator walkDownAnimation;
    SpriteAnimator walkUpAnimation;
    SpriteAnimator walkLeftAnimation;
    SpriteAnimator walkRightAnimation;
    SpriteAnimator currentAnimation;
    SpriteRenderer spriteRenderer;

    bool wasMoving;

    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }

    public DefaultDirection DefaultDirection => defaultDirection;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnimation = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnimation = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkLeftAnimation = new SpriteAnimator(walkLeftSprites, spriteRenderer);
        walkRightAnimation = new SpriteAnimator(walkRightSprites, spriteRenderer);
        SetDefaultDirection(defaultDirection);
        currentAnimation = walkDownAnimation;
    }

    private void Update()
    {
        var prevAnimation = currentAnimation;

        if (MoveY == -1)
            currentAnimation = walkDownAnimation;
        else if (MoveY == 1)
            currentAnimation = walkUpAnimation;
        else if (MoveX == -1)
            currentAnimation = walkLeftAnimation;
        else if (MoveX == 1)
            currentAnimation = walkRightAnimation;

        if (currentAnimation != prevAnimation || IsMoving != wasMoving)
            currentAnimation.Start();

        if (IsMoving)
            currentAnimation.HandleUpdate();
        else
            spriteRenderer.sprite = currentAnimation.Frames[0];

        wasMoving = IsMoving;
    }

    public void SetDefaultDirection(DefaultDirection direction)
    {
        if (direction == DefaultDirection.Down)
            MoveY = -1;
        else if (direction == DefaultDirection.Up)
            MoveY = 1;
        else if (direction == DefaultDirection.Left)
            MoveX = -1;
        else if (direction == DefaultDirection.Right)
            MoveX = 1;
    }
}
