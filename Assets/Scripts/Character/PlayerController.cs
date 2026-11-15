using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ISavable
{
    [SerializeField] GameObject shadow;
    [SerializeField] GameObject darkLight;
    [SerializeField] GameObject flashLight;
    [SerializeField] Sprite firstPlayer;
    [SerializeField] Sprite secondPlayer;

    SaveSlotID saveSlot;

    private Vector2 moveInput;
    
    public static PlayerController i { get; private set; }
    private Character character;

    bool currentSceneIsDark;
    bool previousSceneWasDark;

    [HideInInspector]
    public int repelStepsRemaining;
    [HideInInspector]
    public int stepsUntilFriendshipBoost;
    [HideInInspector]
    public int pokemonLevelCap;

    string playerName;
    Sprite playerSprite;

    private MainGameControls gameControls;
    private InputAction moveAction;
    private InputAction interactAction;

    private void Awake()
    {
        i = this;
        gameControls = new MainGameControls();
        character = GetComponent<Character>();
    }

    private void Start()
    {
        ResetPlayerPosition();
    }

    private void OnEnable()
    {
        moveAction = gameControls.Player.Move;
        interactAction = gameControls.Player.Interact;
        moveAction.Enable();
        interactAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        interactAction.Disable();
    }

    public void ResetPlayerPosition()
    {
        transform.position = new Vector2(1.5f, -17.3125f);
        character.Animator.MoveX = 0;
        character.Animator.MoveY = 1;
        repelStepsRemaining = -1;
        stepsUntilFriendshipBoost = 128;
        pokemonLevelCap = 16;
    }

    public void HandleUpdate()
    {
        if (GlobalSettings.i.AvatarStyle)
        {
            character.Animator.skin = CharacterSkin.First;
            playerSprite = firstPlayer;
            playerName = "Brendan";
        }
        else
        {
            character.Animator.skin = CharacterSkin.Second;
            playerSprite = secondPlayer;
            playerName = "May";
        }

        moveInput = moveAction.ReadValue<Vector2>();

        if (moveInput.x != 0) moveInput.y = 0;

        if (!character.IsMoving)
        {
            if (moveInput != Vector2.zero)
                StartCoroutine(character.Move(moveInput, OnMoveOver));
        }

        character.HandleUpdate();

        if (interactAction.WasPressedThisFrame())
            StartCoroutine(Interact());

        if (GameController.Instance.CurrentScene != null)
            currentSceneIsDark = GameController.Instance.CurrentScene.IsDark;
        else
            currentSceneIsDark = false;

        if (GameController.Instance.PreviousScene != null)
            previousSceneWasDark = GameController.Instance.PreviousScene.IsDark;
        else
            previousSceneWasDark = false;

        if (currentSceneIsDark)
        {
            if (character.Animator.IsUsingFlash)
            {
                darkLight.gameObject.SetActive(false);
                flashLight.gameObject.SetActive(true);
            }
            else
            {
                darkLight.gameObject.SetActive(true);
                flashLight.gameObject.SetActive(false);
            }
        }
        else
        {
            if (previousSceneWasDark && character.Animator.IsUsingFlash)
                character.Animator.IsUsingFlash = true;
            else
                character.Animator.IsUsingFlash = false;

            darkLight.gameObject.SetActive(false);
            flashLight.gameObject.SetActive(false);
        }

        if (character.Animator.IsUsingStrength && !character.IsMoving)
        {
            var facingDirection = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
            var interactPosition = transform.position + facingDirection;
            var collider = Physics2D.OverlapCircle(interactPosition - new Vector3(0, character.npcOffsetY), 0.2f, GameLayers.i.StrengthBoulderLayer);
            
            if (collider != null && !collider.GetComponent<Interactable>().IsMoving)
                StartCoroutine(collider.GetComponent<Interactable>()?.Move(new Vector2(character.Animator.MoveX, character.Animator.MoveY), true));
        }

        if (character.Animator.IsJumping || character.Animator.IsJumpingToWater)
            shadow.gameObject.SetActive(true);
        else
            shadow.gameObject.SetActive(false);
    }

    IEnumerator Interact()
    {
        var facingDirection = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPosition = transform.position + facingDirection;
        var collisionLayers = GameLayers.i.InteractableLayer | GameLayers.i.InvisibleInteractableLayer | GameLayers.i.WaterLayer | GameLayers.i.StrengthBoulderLayer;
        var collider = Physics2D.OverlapCircle(interactPosition - new Vector3(0, character.npcOffsetY), 0.2f, collisionLayers);
        var caveCollider = Physics2D.OverlapCircle(transform.position - new Vector3(0, character.npcOffsetY), 0.2f, GameLayers.i.CaveTilesLayer);
        
        if (collider != null)
            yield return collider.GetComponent<Interactable>()?.Interact(transform);
        else if (caveCollider != null)
            yield return caveCollider.GetComponent<Interactable>()?.Interact(transform);
    }

    IPlayerTriggerable currentlyInTrigger;

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.npcOffsetY), 0.2f, GameLayers.i.TriggerableLayers);
        IPlayerTriggerable triggerable = null;
        foreach (var collider in colliders)
        {
            triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                if (triggerable == currentlyInTrigger && !triggerable.TriggerRepeatedly)
                    break;

                triggerable.OnPlayerTriggered(this);
                currentlyInTrigger = triggerable;
                break;
            }
        }

        if (colliders.Count() == 0 || triggerable != currentlyInTrigger)
            triggerable = null;
    }

    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },
            direction = new float[] { character.Animator.MoveX, character.Animator.MoveY },
            pokemonList = GetComponent<PokemonParty>().PokemonList.Select(p => p.GetSaveData()).ToList(),
            repelStepsLeft = repelStepsRemaining,
            pokemonMaxLevelCap = pokemonLevelCap,
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;
        var position = saveData.position;
        transform.position = new Vector3(position[0], position[1]);
        character.SetPositionAndSnapToTile(transform.position);
        var direction = saveData.direction;
        character.Animator.MoveX = direction[0];
        character.Animator.MoveY = direction[1];

        GetComponent<PokemonParty>().PokemonList = saveData.pokemonList.Select(s => new Pokemon(s)).ToList();
        repelStepsRemaining = saveData.repelStepsLeft;
        pokemonLevelCap = saveData.pokemonMaxLevelCap;
    }

    public Sprite PlayerSprite {
        get => playerSprite;
    }

    public string PlayerName {
        get => playerName;
    }

    public GameObject Shadow {
        get => shadow;
    }

    public string SaveSlot {
        get => saveSlot.ToString();
    }

    public Character Character => character;
}

public enum SaveSlotID { SaveSlot1, SaveSlot2, SaveSlot3 }

[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public float[] direction;
    public List<PokemonSaveData> pokemonList;
    public int repelStepsLeft;
    public int pokemonMaxLevelCap;
}