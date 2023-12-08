using Assets.Scripts.Data;
using System.Collections.Generic;
using Rawgen.Math.Logic.Utils;
using UnityEngine;
using System;
using System.Linq;
using TMPro;
using Assets.Scripts.Logic;
using Assets.Scripts.Utils;
using NETCoreServer.Models;
using Assets.Scripts.Controller.InGame;
using Assets.Scripts.Data.MultiplayerStateModels;

public class TroopController : MonoBehaviour, IObjectAnimator, IObjectSelectable
{
    private const int GuerrillaLimit = 100;
    public const int MaxTroops = 9999;
    private TextMeshProUGUI unitsText;
    private GlobalLogicController globalLogic;
    private StateController stateController;
    private SoundEffectsController soundEffectsController;
    private AILogic aiLogic;
    private float lastAttack = 0;
    private float reloadTime = 3f;
    public CircleCollider2D circleCollider;
    public CombatLogic combatLogic = new CombatLogic();
    public TroopModel troopModel;
    public List<TroopController> combatingEnemys = new List<TroopController>();
    public CircleRotationAnimation animator;

    // Start is called before the first frame update
    void Start()
    {
        unitsText = GetComponent<TextMeshProUGUI>();
        globalLogic = FindObjectOfType<GlobalLogicController>();
        stateController = FindObjectOfType<StateController>();
        soundEffectsController = FindObjectOfType<SoundEffectsController>();
        circleCollider = GetComponent<CircleCollider2D>();
        aiLogic = globalLogic.aiLogics.Find(item => item.Player == troopModel.Player);
        unitsText.SetText(troopModel.Units.ToString());
        UpdateColliderSize();
        unitsText.color = ColorUtils.GetColorByString(troopModel.Player.Color);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("globalLogic.IsMultiplayerClient: " + globalLogic.IsMultiplayerClient);

        if (globalLogic.IsMultiplayerHost || globalLogic.IsSingleplayer)
        {
            Debug.Log("IsMultiplayerHost");

            UpdateTroopModel();
            Combat();

            if (aiLogic == null)
            {
                PlayerTroopUpdate();
            }
            else
            {
                AITroopUpdate();
            }

            if (globalLogic.IsMultiplayerHost)
            {
                stateController.SetTroopState(this.name, troopModel.Units, this.transform.position);
            }
        }
        else if (globalLogic.IsMultiplayerClient)
        {
            Debug.Log("IsMultiplayerClient");

            TroopStateModel troopStateModel = stateController.GetTroopState(this.name);
            troopModel.Units = troopStateModel.size;
            this.transform.position = troopStateModel.GetPositionAsVector3();
        }
        else
        {
            Debug.LogWarning("Unexpected gametype on CityController");
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TroopController otherTroopController;

        otherTroopController = collision.gameObject.GetComponent<TroopController>();

        if (otherTroopController != null)
        {
            if (otherTroopController.troopModel.Player != troopModel.Player)
            {
                if (troopModel.Player.Alliance == Player.NoAlliance || otherTroopController.troopModel.Player.Alliance != troopModel.Player.Alliance)
                {
                    troopModel.InCombat = true;
                    otherTroopController.troopModel.InCombat = true;
                    combatingEnemys.Add(otherTroopController);
                    soundEffectsController.PlayBattleSound();
                }
            }
            else if (otherTroopController.troopModel.Units < troopModel.Units ||
                 (otherTroopController.troopModel.Units == troopModel.Units 
                 && otherTroopController.gameObject.name.CompareTo(gameObject.name) < 0))
            {
                 if (troopModel.Units + otherTroopController.troopModel.Units <= MaxTroops)
                 {
                        troopModel.MergeTroop(otherTroopController.troopModel);
                        unitsText.text = Convert.ToString(troopModel.Units);
                        globalLogic.DestroyUnit(otherTroopController.gameObject, null);
                 }
                else
                {
                    troopModel.MergeUntilMax(otherTroopController.troopModel);
                    unitsText.text = Convert.ToString(troopModel.Units);
                    otherTroopController.unitsText.text = Convert.ToString(otherTroopController.troopModel.Units);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        TroopController enemyController;

        enemyController = collision.gameObject.GetComponent<TroopController>();

        if (enemyController != null)
        {
            combatingEnemys.Remove(enemyController);
        }
    }

    private void OnMouseDown()
    {
        globalLogic.LeftClickReceivedFromTroop(this);
        Debug.Log("On left click: " + this);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(KeyConstants.RightClick))
        {
            globalLogic.RightClickReceivedFromTroop(this);
            Debug.Log("On right click: " + this);
        }
    }

    public void ReceiveAttack(CombatResults results, TroopController origin)
    {
        troopModel.Units = results.DefensorRemainingUnits;
        unitsText.text = Convert.ToString(results.DefensorRemainingUnits);

        if (results.DefensorRemainingUnits <= 0)
        {
            globalLogic.DestroyUnit(this.gameObject, origin);
        }
    }

    public void Animate()
    {
        animator.gameObject.SetActive(true);
        animator.IterateAnimation();
    }

    public void EndAnimation()
    {
        animator.gameObject.SetActive(false);
    }

    public bool IsSelectable(int owner)
    {
        return troopModel.Player.MapSocketId == owner;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    /// <summary>
    /// En las partidas multiplayer, sirve para indicarle manualmente el objetivo a una tropa.
    /// </summary>
    public void SetTroopOrder()
    {
        //TODO: SEPT-23-008
    }

    public void EndSelection()
    {
        EndAnimation();
    }

    private void UpdateTroopModel()
    {
        int lastTroopQuantity = troopModel.Units;

        troopModel.CurrentPosition = transform.position;
        troopModel.Units = Convert.ToInt32(unitsText.text.ToString());

        if (lastTroopQuantity != troopModel.Units)
        {
            UpdateColliderSize();
        }
    }

    private void UpdateColliderSize()
    {
        int textSize = Convert.ToString(troopModel.Units).Length;

        switch (textSize)
        {
            case 1:
                circleCollider.radius = 0.5f;
                break;
            case 2:
                circleCollider.radius = 0.65f;
                break;
            case 3:
                circleCollider.radius = 0.85f;
                break;
            case 4:
                circleCollider.radius = 1.1f;
                break;
        }
    }

    private void PlayerTroopUpdate()
    {
        if (troopModel.Target != null)
        {
            MoveTroop();
        }
    }

    private void AITroopUpdate()
    {
        aiLogic.TroopMovementRequest(troopModel);

        if (troopModel.Target != null)
        {
            MoveTroop();
        }
    }

    private void MoveTroop()
    {
        const float ArrivalDistance = 0.01f;
        const float BaseSpeed = 0.0005f;
        Vector2 direction;
        Vector2 movement;
        Vector2 targetPosition;
        float newX;
        float newY;
        float targetX;
        float targeyY;

        try
        {
            if (CanMove())
            {
                targetPosition = troopModel.Target.transform.position;
                direction = targetPosition - new Vector2(this.transform.position.x, this.transform.position.y);

                if (direction.magnitude != 0)
                {
                    direction /= direction.magnitude;
                }
                else
                {
                    troopModel.SetTarget(null, globalLogic);
                }

                movement = direction * (Time.deltaTime * 100) * troopModel.Speed * BaseSpeed * globalLogic.GameSpeed * GetBonusSpeed();

                this.transform.Translate(movement.x, movement.y, 0);

                newX = this.transform.position.x;
                newY = this.transform.position.y;
                targetX = targetPosition.x;
                targeyY = targetPosition.y;

                if (ArrivalDistance > MathUtils.ExperimentalDistance(newX, newY, targetX, targeyY))
                {
                    troopModel.SetTarget(null, globalLogic);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Target Position: " + troopModel.Target.transform.position + "; Current Position: " + transform.position + " exception: " + ex.Message);
            throw;
        }
    }

    private bool CanMove()
    {
        if (troopModel.InCombat)
        {
            switch (troopModel.Player.Faction.Bonus.BonusId)
            {
                case Bonus.Id.Guerrilla:
                    return troopModel.Units < GuerrillaLimit;
                case Bonus.Id.MoveOnCombat:
                    return true;
                default:
                    return false;
            }
        }
        else
        {
            return true;
        }
    }

    private float GetBonusSpeed()
    {
        const float BonusFactor = 1.2f;
        const float NonBonusFactor = 1f;

        switch (troopModel.Player.Faction.Bonus.BonusId)
        {
            case Bonus.Id.Speed:
                return BonusFactor;
            case Bonus.Id.Guerrilla:
                return troopModel.Units < GuerrillaLimit ? BonusFactor : NonBonusFactor;
            default:
                return NonBonusFactor;
        }
    }

    private void Combat()
    {
        if (troopModel.InCombat && (Time.time > lastAttack + (reloadTime / globalLogic.GameSpeed)))
        {
            RemoveDestroyedUnits();
            if (combatingEnemys.Count > 0)
            {
                TroopController target;
                CombatResults results;

                target = combatingEnemys.OrderByDescending(enemy => enemy.troopModel.Units).First();
                results = combatLogic.Combat(this, target);

                if (results.DefensorRemainingUnits <= 0)
                {
                    combatingEnemys.Remove(target);
                }

                target.ReceiveAttack(results, this);

                troopModel.Units = results.AttackerRemainingUnits;
                unitsText.text = Convert.ToString(results.AttackerRemainingUnits);
                lastAttack = Time.time;

                if (results.AttackerRemainingUnits <= 0)
                {
                    globalLogic.DestroyUnit(this.gameObject, target);
                }
            }
            else
            {
                troopModel.InCombat = false;
            }
        }
    }

    private void RemoveDestroyedUnits()
    {
        combatingEnemys.RemoveAll(item => item == null);
    }
}
