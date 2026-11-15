using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using E7.Introloop;

public class Character : MonoBehaviour
{
    [SerializeField] GameObject shadow;

    public bool ableToRun;
    public float moveSpeed;
    public float runSpeed;

    CharacterAnimator animator;

    public bool IsMoving { get; private set; }
    public bool IsRunning { get; private set; }

    public float npcOffsetY { get; set; } = 0.25f;
    public float playerOffsetY { get; set; } = 0.1875f;

    private MainGameControls gameControls;
    private InputAction runAction;
    
    private void Awake()
    {
        gameControls = new MainGameControls();
        SetPositionAndSnapToTile(transform.position);
        animator = GetComponent<CharacterAnimator>();
    }

    private void OnEnable()
    {
        runAction = gameControls.Player.Run;
        runAction.Enable();
    }

    private void OnDisable()
    {
        runAction.Disable();
    }

    private void Start()
    {
        
    }

    public void SetPositionAndSnapToTile(Vector2 position)
    {
        position.x = Mathf.Floor(position.x) + 0.5f;
        
        if (ableToRun)
            position.y = Mathf.Floor(position.y) + 0.5f + playerOffsetY;
        else
            position.y = Mathf.Floor(position.y) + 0.5f + npcOffsetY;

        transform.position = position;
    }

    public IEnumerator Move(Vector2 moveVector, Action OnMoveOver = null, bool checkCollisions = true)
    {
        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        var walkablePosition = transform.position;
        walkablePosition.x += moveVector.x / 2f;
        walkablePosition.y += moveVector.y / 2f;

        var targetPosition = transform.position;
        targetPosition.x += moveVector.x;
        targetPosition.y += moveVector.y;

        var ledge = CheckForLedge(targetPosition);

        if (ledge != null && ableToRun)
        {
            if (ledge.TryToJump(this, moveVector))
                yield break;
        }

        if (!checkCollisions || (IsPathClear(walkablePosition, targetPosition) && ableToRun) || (IsPathClear(targetPosition, targetPosition) && !ableToRun))
        {
            IsMoving = true;
            var currentState = GameController.Instance.StateMachine.GetCurrentState();
            IsRunning = (ableToRun && runAction.IsPressed() && currentState == FreeRoamState.i);

            if (animator.IsSurfing && Physics2D.OverlapCircle(targetPosition - new Vector3(0, playerOffsetY), 0.2f, GameLayers.i.WaterLayer) == null)
            {
                animator.IsSurfing = false;
                AudioManager.i.PlayMusic(GameController.Instance.CurrentScene.SceneMusic, true);
            }

            while ((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
            {
                if (IsRunning || animator.IsSurfing)
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, runSpeed * Time.deltaTime);
                else
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                yield return null;
            }
            transform.position = targetPosition;

            if (ableToRun && PlayerController.i.repelStepsRemaining >= 0)
            {
                if (ableToRun && PlayerController.i.repelStepsRemaining == 0)
                {
                    IsMoving = false;
                    IsRunning = false;
                    StartCoroutine(DialogManager.Instance.ShowDialogText($"Repel's effect wore off!"));
                }
                PlayerController.i.repelStepsRemaining--;
            }
            if (ableToRun && PlayerController.i.stepsUntilFriendshipBoost > 0)
            {
                PlayerController.i.stepsUntilFriendshipBoost--;
                if (ableToRun && PlayerController.i.stepsUntilFriendshipBoost == 0)
                {
                    var playerParty = PlayerController.i.GetComponent<PokemonParty>();
                    if (playerParty.PokemonList.Count > 0 && UnityEngine.Random.Range(0, 2) == 0)
                        playerParty.PokemonList.ForEach(p => p.ChangeFriendship(2, 1, 1));
                    PlayerController.i.stepsUntilFriendshipBoost = 128;
                }
            }

            IsMoving = false;

            OnMoveOver?.Invoke();
        }
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
        animator.IsRunning = IsRunning;
    }

    private bool IsPathClear(Vector3 walkablePosition, Vector3 targetPosition)
    {
        var difference = targetPosition - transform.position;
        var direction = difference.normalized;

        var npcLayer = GameLayers.i.InteractableLayer;

        var collisionLayers = GameLayers.i.SolidLayer | GameLayers.i.StrengthBoulderLayer;

        if (!animator.IsSurfing)
            collisionLayers = collisionLayers | GameLayers.i.WaterLayer;

        var offset = new Vector3(0f, 0f, 0f);

        var angle = 0f;

        if (animator.MoveX == 0 && animator.MoveY == 1)
            angle = 0f;
        if (animator.MoveX == 1 && animator.MoveY == 0)
            angle = 90f;
        if (animator.MoveX == 0 && animator.MoveY == -1)
            angle = 180f;
        if (animator.MoveX == -1 && animator.MoveY == 0)
            angle = -90f;

        if (ableToRun)
        {
            if (Physics2D.BoxCast(new Vector2(walkablePosition.x, walkablePosition.y - playerOffsetY), new Vector2(0.8f, 0.98f), angle, direction, difference.magnitude - 1, collisionLayers))
                return false;
            
            if (Physics2D.BoxCast(new Vector2(targetPosition.x, targetPosition.y - playerOffsetY), new Vector2(0.98f, 0.98f), angle, direction, difference.magnitude - 1, npcLayer))
                return false;

            offset = new Vector3(0f, playerOffsetY, 0f);
        }
        else
        {
            if (Physics2D.BoxCast(new Vector2(walkablePosition.x, walkablePosition.y - npcOffsetY), new Vector2(0.8f, 0.98f), angle, direction, difference.magnitude - 1, collisionLayers | npcLayer))
                return false;

            if (Physics2D.BoxCast(new Vector2(targetPosition.x, targetPosition.y - npcOffsetY), new Vector2(0.98f, 0.98f), angle, direction, difference.magnitude - 1, GameLayers.i.PlayerLayer) && GetComponent<TrainerController>() == null)
                return false;

            offset = new Vector3(0f, npcOffsetY, 0f);
        }

        if (!InteriorBounds.IsMoveAllowed(transform.position - offset, targetPosition - offset))
            return false;

        return true;
    }

    Ledge CheckForLedge(Vector3 targetPosition)
    {
        Collider2D collider;

        if (ableToRun)
            collider = Physics2D.OverlapCircle(targetPosition - new Vector3(0, playerOffsetY), 0.2f, GameLayers.i.LedgeLayer);
        else
            collider = Physics2D.OverlapCircle(targetPosition - new Vector3(0, npcOffsetY), 0.2f, GameLayers.i.LedgeLayer);

        return collider?.GetComponent<Ledge>();
    }

    public void LookTowards(Vector3 targetPosition)
    {
        var xDifference = Mathf.Floor(targetPosition.x) - Mathf.Floor(transform.position.x);
        var yDifference = Mathf.Floor(targetPosition.y) - Mathf.Floor(transform.position.y);

        if (xDifference == 0 || yDifference == 0)
        {
            animator.MoveX = Mathf.Clamp(xDifference, -1f, 1f);
            animator.MoveY = Mathf.Clamp(yDifference, -1f, 1f);
        }
        else
            Debug.LogError("Error in Look Towards function. Characters can't look diagonally.");
    }

    public CharacterAnimator Animator {
        get => animator;
    }

    public GameObject Shadow {
        get => shadow;
    }
}