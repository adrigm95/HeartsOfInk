using Assets.Scripts.Data;
using System.Collections.Generic;
using Rawgen.Math.Logic.Utils;
using UnityEngine;
using System;
using System.Linq;
using TMPro;
using Assets.Scripts.Logic;

public class TroopController : MonoBehaviour
{
    private const int MaxTroops = 9000;
    private TextMeshProUGUI unitsText;
    private GlobalLogicController globalLogic;
    private SoundEffectsController soundEffectsController;
    private CircleCollider2D circleCollider;
    public CombatLogic combatLogic = new CombatLogic();

    private AILogic aiLogic;
    private float lastAttack = 0;
    private float reloadTime = 3f;
    public TroopModel troopModel;
    public List<TroopController> combatingEnemys = new List<TroopController>();

    // Start is called before the first frame update
    void Start()
    {
        unitsText = GetComponent<TextMeshProUGUI>();
        globalLogic = FindObjectOfType<GlobalLogicController>();
        soundEffectsController = FindObjectOfType<SoundEffectsController>();
        circleCollider = GetComponent<CircleCollider2D>();

        troopModel = new TroopModel(this.name);
        troopModel.Units = Convert.ToInt32(unitsText.text.ToString());

        aiLogic = globalLogic.aiLogics.Find(item => item.FactionId == troopModel.FactionId);
        UpdateColliderSize();
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TroopController enemyController;

        enemyController = collision.gameObject.GetComponent<TroopController>();

        if (enemyController != null)
        {
            if (enemyController.troopModel.FactionId != troopModel.FactionId)
            {
                troopModel.InCombat = true;
                enemyController.troopModel.InCombat = true;
                combatingEnemys.Add(enemyController);
                soundEffectsController.PlayBattleSound();
            }
            else if (enemyController.troopModel.Units < troopModel.Units ||
                 (enemyController.troopModel.Units == troopModel.Units 
                 && enemyController.gameObject.name.CompareTo(gameObject.name) < 0))
            {
                 if (troopModel.Units + enemyController.troopModel.Units <= MaxTroops)
                 {
                        troopModel.MergeTroop(enemyController.troopModel);
                        unitsText.text = Convert.ToString(troopModel.Units);
                        globalLogic.DestroyUnit(enemyController.gameObject, null);
                 }
            }
        }
    }

    private void OnMouseDown()
    {
        globalLogic.ClickReceivedFromTroop(this);
        Debug.Log("On click: " + this);
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
        float bonusSpeed;

        try
        {
            if (!troopModel.InCombat)
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

                bonusSpeed = troopModel.FactionId == Faction.Id.NOMADS ? 1.2f : 1f;
                movement = direction * (Time.deltaTime * 100) * troopModel.Speed * BaseSpeed * globalLogic.GameSpeed * bonusSpeed;

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
            Debug.LogError("Target Position: " + troopModel.Target.transform.position + "; Current Position: " + transform.position);
            throw;
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

                if (results.DefensorRemainingUnits == 1 ||
                    results.AttackerRemainingUnits == 1)
                {
                    Debug.Log("Interruption point log line");
                }

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

    public void ReceiveAttack(CombatResults results, TroopController origin)
    {
        troopModel.Units = results.DefensorRemainingUnits;
        unitsText.text = Convert.ToString(results.DefensorRemainingUnits);

        if (results.DefensorRemainingUnits <= 0)
        {
            globalLogic.DestroyUnit(this.gameObject, origin);
        }
    }

    public void AnimateText(float textSize)
    {
        unitsText.fontSize = textSize;
    }
}
