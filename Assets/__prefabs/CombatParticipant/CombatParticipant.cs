using System.Collections.Generic;
using R3;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CombatParticipant : MonoBehaviour
{
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
    Button SelectorButton;
    [SerializeField]
    bool IsEnemy = true;

    [SerializeField]
    public Slider HealthSlider;
    [SerializeField]
    Image FillColor;
    [SerializeField]
    TMPro.TextMeshProUGUI HealthNumber;

    private CombatController combatController;
    public BehaviorSubject<bool> IsMyTurnStartObs = new BehaviorSubject<bool>(false);
    public BehaviorSubject<bool> IsMyTurnObs = new BehaviorSubject<bool>(false);
    public BehaviorSubject<bool> IsMyTurnSelectTargetObs = new BehaviorSubject<bool>(false);
    public BehaviorSubject<bool> IsMyTurnConfirmActionObs = new BehaviorSubject<bool>(false);

    void Start()
    {
        if (this.gameObject.name.Contains("Player"))
        {
            IsEnemy = false;
        }
        this.combatController = GameObject.FindGameObjectWithTag("CombatController").GetComponent<CombatController>();
        if (this.combatController == null)
        {
            Debug.Log("Not in combat, skipping combat participant initialization.");
            return;
        }
        this.SelectorButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (this.isCurrentTargetForConfirm())
                {
                    Debug.Log("CombatParticipant: " + this.name + " is already selected for confirm action.");
                    return;
                }
                var state = combatController.CombatStateObs.Value;
                state.SkillTargets = new List<CombatParticipant> { this };
                state.Phase = CombatPhase.TurnConfirmAction;
                combatController.SetCombatState(state);
            })
            .AddTo(this);
        this.combatController.CombatStateObs
            .Where(state => state != null)
            .Do(state =>
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

                var selectedSkill = state.SelectedSkillBook;
                bool isPlayerTargeting = !isMyTurn && state.Phase == CombatPhase.TurnSelectTarget;
                bool isValidTarget = selectedSkill != null &&
                                     (selectedSkill.TargetType == TargetType.AllySingle && !IsEnemy ||
                                      selectedSkill.TargetType == TargetType.EnemySingle && IsEnemy);
                this.SelectorButton.gameObject.SetActive((isPlayerTargeting && isValidTarget) || this.isCurrentTargetForConfirm());
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
}
