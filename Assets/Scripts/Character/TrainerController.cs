using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using E7.Introloop;

public class TrainerController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] Sprite trainerSprite;
    [SerializeField] string trainerName;
    [SerializeField] string trainerClass;
    public Dialog dialog;
    public string dialogEndBattle;
    public Dialog dialogAfterBattle;
    public Dialog dialogRefuseBattle;
    public GameObject exclamation;
    public List<Sprite> exclamationSprites;
    public GameObject fov;
    public int moneyToGive;
    public int battleUnitCount = 1;
    public bool usesBadGuyMusic;

    public IntroloopAudio trainerAppearsMusic;

    public BattleTrigger trainerBattleTrigger;

    [SerializeField] bool isSpecialTrainer;
    [SerializeField] ItemBase requiredBadge;
    public QuestBase questToCompleteBeforeBattle;
    public QuestBase questToCompleteAfterBattle;

    public bool basicAI;
    public bool setupAI;
    public bool typesAI;
    public bool offensiveAI;
    public bool aggressiveAI;
    public bool cautiousAI;
    public bool riskyAI;
    public bool opportunistAI;
    public bool statusAI;

    // State
    public bool battleLost = false;
    Character trainerCharacter;
    public Quest activeQuest;
    SpriteRenderer exclamationRenderer;
    public Vector3 originalTrainerPosition;

    private void Awake()
    {
        trainerCharacter = GetComponent<Character>();
        exclamationRenderer = exclamation.GetComponent<SpriteRenderer>();
        originalTrainerPosition = trainerCharacter.transform.localPosition;
    }

    private void Update()
    {
        trainerCharacter.HandleUpdate();
    }

    public virtual void Start()
    {
        SetFOVRotation(trainerCharacter.Animator.DefaultDirection);
    }

    public virtual IEnumerator Interact(Transform initiator)
    {
        trainerCharacter.LookTowards(initiator.position);

        if (!battleLost)
        {
            var playerPartyCount = initiator.GetComponent<PokemonParty>().PokemonList.Count;
            if (battleUnitCount > playerPartyCount)
            {
                yield return DialogManager.Instance.ShowDialog(dialogRefuseBattle);
                if (trainerCharacter.Animator.DefaultDirection == FacingDirection.Up)
                    trainerCharacter.LookTowards(originalTrainerPosition + new Vector3(0, 1));
                else if (trainerCharacter.Animator.DefaultDirection == FacingDirection.Down)
                    trainerCharacter.LookTowards(originalTrainerPosition + new Vector3(0, -1));
                else if (trainerCharacter.Animator.DefaultDirection == FacingDirection.Left)
                    trainerCharacter.LookTowards(originalTrainerPosition + new Vector3(-1, 0));
                else if (trainerCharacter.Animator.DefaultDirection == FacingDirection.Right)
                    trainerCharacter.LookTowards(originalTrainerPosition + new Vector3(1, 0));
            }
            else
            {
                if (isSpecialTrainer)
                {
                    AudioManager.i.PlaySfx(AudioID.TrainerExclaim, pauseMusic: true);

                    exclamation.SetActive(true);
                    exclamationRenderer.sprite = exclamationSprites[0];
                    yield return new WaitForSeconds(0.05f);
                    exclamationRenderer.sprite = exclamationSprites[1];
                    yield return new WaitForSeconds(0.05f);

                    AudioManager.i.PlayMusic(trainerAppearsMusic, true);

                    exclamationRenderer.sprite = exclamationSprites[2];
                    yield return new WaitForSeconds(0.4f);
                    exclamation.SetActive(false);

                    if (questToCompleteBeforeBattle != null)
                    {
                        var quest = new Quest(questToCompleteBeforeBattle);
                        yield return quest.CompleteQuest(initiator.GetComponent<PlayerController>());
                        questToCompleteBeforeBattle = null;
                    }

                    yield return DialogManager.Instance.ShowDialog(dialog);
                    GameController.Instance.StartTrainerBattle(this, trainerBattleTrigger);
                }
                else
                {
                    AudioManager.i.PlaySfx(AudioID.TrainerExclaim, pauseMusic: true);

                    exclamation.SetActive(true);
                    exclamationRenderer.sprite = exclamationSprites[0];
                    yield return new WaitForSeconds(0.05f);
                    exclamationRenderer.sprite = exclamationSprites[1];
                    yield return new WaitForSeconds(0.05f);

                    AudioManager.i.PlayMusic(trainerAppearsMusic, true);

                    exclamationRenderer.sprite = exclamationSprites[2];
                    yield return new WaitForSeconds(0.4f);
                    exclamation.SetActive(false);

                    yield return DialogManager.Instance.ShowDialog(dialog);
                    GameController.Instance.StartTrainerBattle(this, trainerBattleTrigger);
                }
            }
        }
        else
        {
            if (isSpecialTrainer && questToCompleteAfterBattle != null)
            {
                var quest = new Quest(questToCompleteAfterBattle);
                yield return quest.CompleteQuest(initiator.GetComponent<PlayerController>());
                questToCompleteAfterBattle = null;

                yield return DialogManager.Instance.ShowDialog(dialogAfterBattle);
                if (trainerClass == "Stinky Grump" && trainerName == "Sunny")
                {
                    for (int i = 0; i < 3; i++)
                    {
                        GetComponent<SpriteRenderer>().enabled = false;
                        yield return new WaitForSeconds(0.1f);
                        GetComponent<SpriteRenderer>().enabled = true;
                        yield return new WaitForSeconds(0.1f);
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        GetComponent<SpriteRenderer>().enabled = false;
                        yield return new WaitForSeconds(0.05f);
                        GetComponent<SpriteRenderer>().enabled = true;
                        yield return new WaitForSeconds(0.05f);
                    }

                    for (int i = 0; i < 7; i++)
                    {
                        GetComponent<SpriteRenderer>().enabled = false;
                        yield return new WaitForSeconds(0.02f);
                        GetComponent<SpriteRenderer>().enabled = true;
                        yield return new WaitForSeconds(0.02f);
                    }

                    GetComponent<SpriteRenderer>().enabled = false;
                    GetComponent<BoxCollider2D>().enabled = false;
                }
            }
            else
                yield return DialogManager.Instance.ShowDialog(dialogAfterBattle);
        }
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        var playerPartyCount = player.GetComponent<PokemonParty>().PokemonList.Count;
        if (battleUnitCount <= playerPartyCount)
        {
            GameController.Instance.StateMachine.Push(CutsceneState.i);

            AudioManager.i.PlaySfx(AudioID.TrainerExclaim, pauseMusic: true);

            // Show exclamation
            exclamation.SetActive(true);
            exclamationRenderer.sprite = exclamationSprites[0];
            yield return new WaitForSeconds(0.05f);
            exclamationRenderer.sprite = exclamationSprites[1];
            yield return new WaitForSeconds(0.05f);

            AudioManager.i.PlayMusic(trainerAppearsMusic, true);

            exclamationRenderer.sprite = exclamationSprites[2];
            yield return new WaitForSeconds(0.4f);
            exclamation.SetActive(false);

            // Walk towards player
            var difference = player.transform.position - transform.position;
            var moveVector = difference - difference.normalized;
            moveVector = new Vector2(Mathf.Round(moveVector.x), Mathf.Round(moveVector.y));

            yield return trainerCharacter.Move(moveVector);

            player.Character.LookTowards(transform.position);

            // Show dialog
            yield return DialogManager.Instance.ShowDialog(dialog);

            GameController.Instance.StateMachine.Pop();

            GameController.Instance.StartTrainerBattle(this, trainerBattleTrigger);
        }
    }

    public virtual IEnumerator LostBattle(Transform initiator)
    {
        battleLost = true;
        fov.SetActive(false);
        yield break;
    }

    public void SetFOVRotation(FacingDirection direction)
    {
        float angle = 0f;
        if (direction == FacingDirection.Up)
            angle = 180f;
        else if (direction == FacingDirection.Left)
            angle = 270f;
        else if (direction == FacingDirection.Right)
            angle = 90f;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public object CaptureState()
    {
        var saveData = new TrainerSaveData()
        {
            lostTheBattle = battleLost,
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (TrainerSaveData)state;
        battleLost = saveData.lostTheBattle;

        if (battleLost)
        {
            fov.gameObject.SetActive(false);
            if (trainerClass == "Stinky Grump" && trainerName == "Sunny")
            {
                GetComponent<SpriteRenderer>().enabled = false;
                trainerCharacter.Shadow.GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    public Sprite TrainerSprite {
        get => trainerSprite;
    }

    public string TrainerName {
        get => trainerName;
    }

    public string TrainerClass {
        get => trainerClass;
    }

    public string DialogEndBattle {
        get => dialogEndBattle;
    }

    public int MoneyToGive {
        get => moneyToGive;
    }

    public bool UsesBadGuyMusic {
        get => usesBadGuyMusic;
    }

    public IEnumerator Move(Vector2 moveVector, bool checkCollisions)
    {
        yield break;
    }

    public bool IsMoving => false;
    public int BattleUnitCount => battleUnitCount;
}

[Serializable]
public class TrainerSaveData
{
    public bool lostTheBattle;
}