using System;
using System.Collections.Generic;
using R3;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CombatParticipant : MonoBehaviour
{
    [SerializeField]
    public ProjectileMaker ProjectileMaker;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    public int MaxHealth = 100;
    [SerializeField]
    public int CurrentHealth = 100;
    [SerializeField]
    public int Defense = 10;
    [SerializeField]
    public int Damage = 20;
    [SerializeField]
    public int AP = 0;
    [SerializeField]
    public float meleeRange = 0.1f;
    [SerializeField]
    Button SelectorButton;
    [SerializeField]
    GameObject SelectionSymbol;
    [SerializeField]
    bool IsEnemy = true;

    [SerializeField]
    public Slider HealthSlider;
    [SerializeField]
    Image FillColor;
    [SerializeField]
    TMPro.TextMeshProUGUI HealthNumber;

    public CombatController combatController;
    [HideInInspector]
    public Vector3 midRangeLandingSpot = Vector3.zero;
    [HideInInspector]
    public Vector3 homeSpot = Vector3.zero;
    [HideInInspector]
    public Quaternion homeRotation = Quaternion.identity;

    #region events
    public BehaviorSubject<bool> IsMyTurnStartObs = new BehaviorSubject<bool>(false);
    public BehaviorSubject<bool> IsMyTurnObs = new BehaviorSubject<bool>(false);
    public BehaviorSubject<bool> IsMyTurnSelectTargetObs = new BehaviorSubject<bool>(false);
    public BehaviorSubject<bool> IsMyTurnConfirmActionObs = new BehaviorSubject<bool>(false);
    public BehaviorSubject<bool> IsMyTurnActionObs = new BehaviorSubject<bool>(false);
    #endregion events

    public (ItemSO, GameObject) armorSOGO;
    public (ItemSO, GameObject) weaponSOGO;

    public Collider Collider;
    public bool IsDead => this.CurrentHealth <= 0;

    void Start()
    {
        this.Collider = this.GetComponent<BoxCollider>();
        this.homeSpot = this.transform.position;
        this.homeRotation = this.transform.rotation;
        if (this.gameObject.name.Contains("Player"))
        {
            IsEnemy = false;
        }
        this.combatController = this.GetCombatController();
        if (this.combatController == null)
        {
            Debug.Log("Not in combat, skipping combat participant initialization.");
            return;
        }
        
        this.SelectorButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                var state = combatController.CombatStateObs.Value;
                state.SkillTargets = new List<CombatParticipant> { this };
                state.Phase = CombatPhase.TurnConfirmAction;
                combatController.SetCombatState(state);
            })
            .AddTo(this);
        this.combatController.CombatStateObs
            .Where(state => state != null)
            .Do(onNext: state =>
            {
                bool isTurnPhase = state.Phase.ToString().Contains("Turn");
                bool isMyTurn = isTurnPhase && state.ShuffledParticipants[state.TurnIndex] == this;
                this.IsMyTurnObs.OnNext(isMyTurn);

                bool isMyTurnStart = isMyTurn && state.Phase == CombatPhase.TurnStart;
                this.IsMyTurnStartObs.OnNext(isMyTurnStart);

                bool isMyTurnSelectTarget = isMyTurn && state.Phase == CombatPhase.TurnSelectTarget;
                this.IsMyTurnSelectTargetObs.OnNext(isMyTurnSelectTarget);

                bool isMyTurnConfirmAction = isMyTurn && state.Phase == CombatPhase.TurnConfirmAction;
                this.IsMyTurnConfirmActionObs.OnNext(isMyTurnConfirmAction);

                bool isMyTurnAction = isMyTurn && state.Phase == CombatPhase.TurnAction;
                this.IsMyTurnActionObs.OnNext(isMyTurnAction);

                var selectedSkill = state.SelectedSkillBook;
                bool isPlayerTargeting = !isMyTurn && state.Phase == CombatPhase.TurnSelectTarget;
                bool isValidTarget = selectedSkill != null &&
                                     (selectedSkill.TargetType == TargetType.AllySingle && !IsEnemy ||
                                      selectedSkill.TargetType == TargetType.EnemySingle && IsEnemy);

                bool isDead = this.IsDead;
                this.SelectorButton.gameObject.SetActive(isPlayerTargeting && isValidTarget && !isDead);
                this.SelectionSymbol.SetActive(isCurrentTargetForConfirm() && !isDead);
            })
            .Subscribe()
            .AddTo(this);
    }

    bool isCurrentTargetForConfirm()
    {
        var state = this.combatController.CombatStateObs.Value;
        return state.SkillTargets.Contains(this) && state.Phase == CombatPhase.TurnConfirmAction;
    }

    // Update is called once per frame
    void Update()
    {
        if (HealthSlider != null)
        {
            HealthSlider.value = (float)CurrentHealth / MaxHealth;
        }
        // 23, 149, 149 by default, yellow if less than 66%, red if less than 33%
        Color color = new Color(0.090f, 0.584f, 0.584f);
        if (HealthSlider.value < 0.33f)
        {
            color = Color.red;
        }
        else if (HealthSlider.value < 0.66f)
        {
            color = Color.yellow;
        }

        if (FillColor != null)
        {
            FillColor.color = color;
        }

        if (HealthNumber != null)
        {
            HealthNumber.text = $"{CurrentHealth}/{MaxHealth}";
            HealthNumber.color = color;
        }
    }

    /// <summary>
    /// returns true if alive
    /// </summary>
    /// <param name="onHitEvent"></param>
    /// <returns></returns>
    public bool OnHit(OnHitEvent onHitEvent)
    {
        int health = this.CurrentHealth - this.ComputeDamage(onHitEvent);
        health = Math.Max(0, health);
        this.CurrentHealth = health;
        return this.CurrentHealth > 0;
    }

    int ComputeDamage(OnHitEvent onHitEvent)
    {
        int skillDamage = onHitEvent.Source.Damage * onHitEvent.SkillBookSO.DamagePercent / 100;
        int takenDmg = skillDamage - this.Defense;
        return Math.Max(0, takenDmg);
    }
}
