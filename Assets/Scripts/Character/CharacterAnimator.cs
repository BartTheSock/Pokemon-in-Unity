using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> idleDownSpritesFirstSkin;
    [SerializeField] List<Sprite> idleUpSpritesFirstSkin;
    [SerializeField] List<Sprite> idleRightSpritesFirstSkin;
    [SerializeField] List<Sprite> idleLeftSpritesFirstSkin;
    
    [SerializeField] List<Sprite> walkDownSpritesFirstSkin;
    [SerializeField] List<Sprite> walkUpSpritesFirstSkin;
    [SerializeField] List<Sprite> walkRightSpritesFirstSkin;
    [SerializeField] List<Sprite> walkLeftSpritesFirstSkin;

    [SerializeField] List<Sprite> runDownSpritesFirstSkin;
    [SerializeField] List<Sprite> runUpSpritesFirstSkin;
    [SerializeField] List<Sprite> runRightSpritesFirstSkin;
    [SerializeField] List<Sprite> runLeftSpritesFirstSkin;

    [SerializeField] List<Sprite> jumpDownSpritesFirstSkin;
    [SerializeField] List<Sprite> jumpUpSpritesFirstSkin;
    [SerializeField] List<Sprite> jumpRightSpritesFirstSkin;
    [SerializeField] List<Sprite> jumpLeftSpritesFirstSkin;

    [SerializeField] List<Sprite> surfDownSpritesFirstSkin;
    [SerializeField] List<Sprite> surfUpSpritesFirstSkin;
    [SerializeField] List<Sprite> surfRightSpritesFirstSkin;
    [SerializeField] List<Sprite> surfLeftSpritesFirstSkin;

    [SerializeField] List<Sprite> idleDownSpritesSecondSkin;
    [SerializeField] List<Sprite> idleUpSpritesSecondSkin;
    [SerializeField] List<Sprite> idleRightSpritesSecondSkin;
    [SerializeField] List<Sprite> idleLeftSpritesSecondSkin;
    
    [SerializeField] List<Sprite> walkDownSpritesSecondSkin;
    [SerializeField] List<Sprite> walkUpSpritesSecondSkin;
    [SerializeField] List<Sprite> walkRightSpritesSecondSkin;
    [SerializeField] List<Sprite> walkLeftSpritesSecondSkin;

    [SerializeField] List<Sprite> runDownSpritesSecondSkin;
    [SerializeField] List<Sprite> runUpSpritesSecondSkin;
    [SerializeField] List<Sprite> runRightSpritesSecondSkin;
    [SerializeField] List<Sprite> runLeftSpritesSecondSkin;

    [SerializeField] List<Sprite> jumpDownSpritesSecondSkin;
    [SerializeField] List<Sprite> jumpUpSpritesSecondSkin;
    [SerializeField] List<Sprite> jumpRightSpritesSecondSkin;
    [SerializeField] List<Sprite> jumpLeftSpritesSecondSkin;

    [SerializeField] List<Sprite> surfDownSpritesSecondSkin;
    [SerializeField] List<Sprite> surfUpSpritesSecondSkin;
    [SerializeField] List<Sprite> surfRightSpritesSecondSkin;
    [SerializeField] List<Sprite> surfLeftSpritesSecondSkin;

    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;

    // Parameters
    public CharacterSkin skin;
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }
    public bool IsRunning { get; set; }
    public bool IsJumping { get; set; }
    public bool IsJumpingToWater { get; set; }
    public bool IsSurfing { get; set; }
    public bool IsUsingFlash { get; set; }
    public bool IsUsingStrength { get; set; }

    // States (first skin)
    SpriteAnimator idleDownAnimationOne;
    SpriteAnimator idleUpAnimationOne;
    SpriteAnimator idleRightAnimationOne;
    SpriteAnimator idleLeftAnimationOne;
    SpriteAnimator walkDownAnimationOne;
    SpriteAnimator walkUpAnimationOne;
    SpriteAnimator walkRightAnimationOne;
    SpriteAnimator walkLeftAnimationOne;
    SpriteAnimator runDownAnimationOne;
    SpriteAnimator runUpAnimationOne;
    SpriteAnimator runRightAnimationOne;
    SpriteAnimator runLeftAnimationOne;
    SpriteAnimator jumpDownAnimationOne;
    SpriteAnimator jumpUpAnimationOne;
    SpriteAnimator jumpRightAnimationOne;
    SpriteAnimator jumpLeftAnimationOne;
    SpriteAnimator surfDownAnimationOne;
    SpriteAnimator surfUpAnimationOne;
    SpriteAnimator surfRightAnimationOne;
    SpriteAnimator surfLeftAnimationOne;

    // States (second skin)
    SpriteAnimator idleDownAnimationTwo;
    SpriteAnimator idleUpAnimationTwo;
    SpriteAnimator idleRightAnimationTwo;
    SpriteAnimator idleLeftAnimationTwo;
    SpriteAnimator walkDownAnimationTwo;
    SpriteAnimator walkUpAnimationTwo;
    SpriteAnimator walkRightAnimationTwo;
    SpriteAnimator walkLeftAnimationTwo;
    SpriteAnimator runDownAnimationTwo;
    SpriteAnimator runUpAnimationTwo;
    SpriteAnimator runRightAnimationTwo;
    SpriteAnimator runLeftAnimationTwo;
    SpriteAnimator jumpDownAnimationTwo;
    SpriteAnimator jumpUpAnimationTwo;
    SpriteAnimator jumpRightAnimationTwo;
    SpriteAnimator jumpLeftAnimationTwo;
    SpriteAnimator surfDownAnimationTwo;
    SpriteAnimator surfUpAnimationTwo;
    SpriteAnimator surfRightAnimationTwo;
    SpriteAnimator surfLeftAnimationTwo;

    SpriteAnimator currentAnimation;

    // References
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        idleDownAnimationOne = new SpriteAnimator(idleDownSpritesFirstSkin, spriteRenderer);
        idleUpAnimationOne = new SpriteAnimator(idleUpSpritesFirstSkin, spriteRenderer);
        idleRightAnimationOne = new SpriteAnimator(idleRightSpritesFirstSkin, spriteRenderer);
        idleLeftAnimationOne = new SpriteAnimator(idleLeftSpritesFirstSkin, spriteRenderer);
        walkDownAnimationOne = new SpriteAnimator(walkDownSpritesFirstSkin, spriteRenderer);
        walkUpAnimationOne = new SpriteAnimator(walkUpSpritesFirstSkin, spriteRenderer);
        walkRightAnimationOne = new SpriteAnimator(walkRightSpritesFirstSkin, spriteRenderer);
        walkLeftAnimationOne = new SpriteAnimator(walkLeftSpritesFirstSkin, spriteRenderer);
        runDownAnimationOne = new SpriteAnimator(runDownSpritesFirstSkin, spriteRenderer, (1f / 12f));
        runUpAnimationOne = new SpriteAnimator(runUpSpritesFirstSkin, spriteRenderer, (1f / 12f));
        runRightAnimationOne = new SpriteAnimator(runRightSpritesFirstSkin, spriteRenderer, (1f / 12f));
        runLeftAnimationOne = new SpriteAnimator(runLeftSpritesFirstSkin, spriteRenderer, (1f / 12f));
        jumpDownAnimationOne = new SpriteAnimator(jumpDownSpritesFirstSkin, spriteRenderer);
        jumpUpAnimationOne = new SpriteAnimator(jumpUpSpritesFirstSkin, spriteRenderer);
        jumpRightAnimationOne = new SpriteAnimator(jumpRightSpritesFirstSkin, spriteRenderer);
        jumpLeftAnimationOne = new SpriteAnimator(jumpLeftSpritesFirstSkin, spriteRenderer);
        surfDownAnimationOne = new SpriteAnimator(surfDownSpritesFirstSkin, spriteRenderer);
        surfUpAnimationOne = new SpriteAnimator(surfUpSpritesFirstSkin, spriteRenderer);
        surfRightAnimationOne = new SpriteAnimator(surfRightSpritesFirstSkin, spriteRenderer);
        surfLeftAnimationOne = new SpriteAnimator(surfLeftSpritesFirstSkin, spriteRenderer);

        idleDownAnimationTwo = new SpriteAnimator(idleDownSpritesSecondSkin, spriteRenderer);
        idleUpAnimationTwo = new SpriteAnimator(idleUpSpritesSecondSkin, spriteRenderer);
        idleRightAnimationTwo = new SpriteAnimator(idleRightSpritesSecondSkin, spriteRenderer);
        idleLeftAnimationTwo = new SpriteAnimator(idleLeftSpritesSecondSkin, spriteRenderer);
        walkDownAnimationTwo = new SpriteAnimator(walkDownSpritesSecondSkin, spriteRenderer);
        walkUpAnimationTwo = new SpriteAnimator(walkUpSpritesSecondSkin, spriteRenderer);
        walkRightAnimationTwo = new SpriteAnimator(walkRightSpritesSecondSkin, spriteRenderer);
        walkLeftAnimationTwo = new SpriteAnimator(walkLeftSpritesSecondSkin, spriteRenderer);
        runDownAnimationTwo = new SpriteAnimator(runDownSpritesSecondSkin, spriteRenderer, (1f / 12f));
        runUpAnimationTwo = new SpriteAnimator(runUpSpritesSecondSkin, spriteRenderer, (1f / 12f));
        runRightAnimationTwo = new SpriteAnimator(runRightSpritesSecondSkin, spriteRenderer, (1f / 12f));
        runLeftAnimationTwo = new SpriteAnimator(runLeftSpritesSecondSkin, spriteRenderer, (1f / 12f));
        jumpDownAnimationTwo = new SpriteAnimator(jumpDownSpritesSecondSkin, spriteRenderer);
        jumpUpAnimationTwo = new SpriteAnimator(jumpUpSpritesSecondSkin, spriteRenderer);
        jumpRightAnimationTwo = new SpriteAnimator(jumpRightSpritesSecondSkin, spriteRenderer);
        jumpLeftAnimationTwo = new SpriteAnimator(jumpLeftSpritesSecondSkin, spriteRenderer);
        surfDownAnimationTwo = new SpriteAnimator(surfDownSpritesSecondSkin, spriteRenderer);
        surfUpAnimationTwo = new SpriteAnimator(surfUpSpritesSecondSkin, spriteRenderer);
        surfRightAnimationTwo = new SpriteAnimator(surfRightSpritesSecondSkin, spriteRenderer);
        surfLeftAnimationTwo = new SpriteAnimator(surfLeftSpritesSecondSkin, spriteRenderer);

        SetFacingDirection(defaultDirection);

        currentAnimation = idleDownAnimationOne;
    }

    private void Update()
    {
        var previousAnimation = currentAnimation;

        if (skin == CharacterSkin.First)
        {
            if (IsSurfing)
            {
                if (MoveX == 1)
                    currentAnimation = surfRightAnimationOne;
                else if (MoveX == -1)
                    currentAnimation = surfLeftAnimationOne;
                else if (MoveY == 1)
                    currentAnimation = surfUpAnimationOne;
                else if (MoveY == -1)
                    currentAnimation = surfDownAnimationOne;
            }
            else
            {
                if (IsJumping)
                {
                    if (MoveX == 1)
                        currentAnimation = jumpRightAnimationOne;
                    else if (MoveX == -1)
                        currentAnimation = jumpLeftAnimationOne;
                    else if (MoveY == 1)
                        currentAnimation = jumpUpAnimationOne;
                    else if (MoveY == -1)
                        currentAnimation = jumpDownAnimationOne;
                }
                else
                {
                    if (IsRunning)
                    {
                        if (MoveX == 1)
                            currentAnimation = IsMoving ? runRightAnimationOne : idleRightAnimationOne;
                        else if (MoveX == -1)
                            currentAnimation = IsMoving ? runLeftAnimationOne : idleLeftAnimationOne;
                        else if (MoveY == 1)
                            currentAnimation = IsMoving ? runUpAnimationOne : idleUpAnimationOne;
                        else if (MoveY == -1)
                            currentAnimation = IsMoving ? runDownAnimationOne : idleDownAnimationOne;
                    }
                    else
                    {
                        if (MoveX == 1)
                            currentAnimation = IsMoving ? walkRightAnimationOne : idleRightAnimationOne;
                        else if (MoveX == -1)
                            currentAnimation = IsMoving ? walkLeftAnimationOne : idleLeftAnimationOne;
                        else if (MoveY == 1)
                            currentAnimation = IsMoving ? walkUpAnimationOne : idleUpAnimationOne;
                        else if (MoveY == -1)
                            currentAnimation = IsMoving ? walkDownAnimationOne : idleDownAnimationOne;
                    }
                }
            }
        }
        else
        {
            if (IsSurfing)
            {
                if (MoveX == 1)
                    currentAnimation = surfRightAnimationTwo;
                else if (MoveX == -1)
                    currentAnimation = surfLeftAnimationTwo;
                else if (MoveY == 1)
                    currentAnimation = surfUpAnimationTwo;
                else if (MoveY == -1)
                    currentAnimation = surfDownAnimationTwo;
            }
            else
            {
                if (IsJumping)
                {
                    if (MoveX == 1)
                        currentAnimation = jumpRightAnimationTwo;
                    else if (MoveX == -1)
                        currentAnimation = jumpLeftAnimationTwo;
                    else if (MoveY == 1)
                        currentAnimation = jumpUpAnimationTwo;
                    else if (MoveY == -1)
                        currentAnimation = jumpDownAnimationTwo;
                }
                else
                {
                    if (IsRunning)
                    {
                        if (MoveX == 1)
                            currentAnimation = IsMoving ? runRightAnimationTwo : idleRightAnimationTwo;
                        else if (MoveX == -1)
                            currentAnimation = IsMoving ? runLeftAnimationTwo : idleLeftAnimationTwo;
                        else if (MoveY == 1)
                            currentAnimation = IsMoving ? runUpAnimationTwo : idleUpAnimationTwo;
                        else if (MoveY == -1)
                            currentAnimation = IsMoving ? runDownAnimationTwo : idleDownAnimationTwo;
                    }
                    else
                    {
                        if (MoveX == 1)
                            currentAnimation = IsMoving ? walkRightAnimationTwo : idleRightAnimationTwo;
                        else if (MoveX == -1)
                            currentAnimation = IsMoving ? walkLeftAnimationTwo : idleLeftAnimationTwo;
                        else if (MoveY == 1)
                            currentAnimation = IsMoving ? walkUpAnimationTwo : idleUpAnimationTwo;
                        else if (MoveY == -1)
                            currentAnimation = IsMoving ? walkDownAnimationTwo : idleDownAnimationTwo;
                    }
                }
            }
        }

        if (currentAnimation != previousAnimation)
            currentAnimation.Start();

        currentAnimation.HandleUpdate();
    }
    
    public void SetFacingDirection(FacingDirection direction)
    {
        if (direction == FacingDirection.Up)
        {
            MoveX = 0;
            MoveY = 1;
        }
        else if (direction == FacingDirection.Down)
        {
            MoveX = 0;
            MoveY = -1;
        }
        else if (direction == FacingDirection.Left)
        {
            MoveX = -1;
            MoveY = 0;
        }
        else if (direction == FacingDirection.Right)
        {
            MoveX = 1;
            MoveY = 0;
        }
    }

    public FacingDirection DefaultDirection {
        get => defaultDirection;
    }

    public SpriteRenderer SpriteRenderer {
        get => spriteRenderer;
    }
}

public enum FacingDirection { Up, Down, Left, Right }
public enum CharacterSkin { First, Second }