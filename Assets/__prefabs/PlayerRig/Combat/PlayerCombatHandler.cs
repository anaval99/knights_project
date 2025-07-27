using System.Collections.Generic;
using R3;
using UnityEditor.PackageManager;
using UnityEngine;

public partial class PlayerCombatHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerAnimation playerAnimation;
    [SerializeField]
    private CharDataCenter charDataCenter;
    [SerializeField]
    private CombatParticipant combatParticipant;
    [SerializeField]
    private EquipmentList equipmentList;
    [SerializeField]
    private UnderGlow underGlow;
    [SerializeField]
    private Skillbar skillbar;

    public void InitializeCombatHandler(Observable<CombatState> combatStateObs)
    {
        combatStateObs.Subscribe(state =>
        {
            switch (state.Phase)
            {
                case CombatPhase.Start:
                    break;
                case CombatPhase.Patrolling:
                    // Handle patrolling logic
                    this.playerAnimation.PlayPatrollingAnimation();
                    break;
                case CombatPhase.BattleStart:
                    // Handle battle start logic
                    this.ResetHomeSpots();
                    this.playerAnimation.PlayIdleAnimation();
                    break;
                case CombatPhase.TurnAction:
                    if (this.combatParticipant == state.ShuffledParticipants[state.TurnIndex])
                    {
                        var skillSOId = state.SelectedSkillBook.name;
                        this.PerformTurnAction(skillSOId);
                    }
                    break;
                case CombatPhase.None:
                default:
                    // Handle other phases or no combat
                    Debug.Log("No combat phase active.");
                    break;
            }
        })
        .AddTo(this);
    }

    void Start()
    {
        this.charDataCenter.CharEquipmentObs.Subscribe(equipment => this.ComputeStatsFromEquipment(equipment)).AddTo(this);
        this.combatParticipant.IsMyTurnObs.DistinctUntilChanged().Subscribe(isMyTurn =>
        {
            this.underGlow.gameObject.SetActive(isMyTurn);
            this.skillbar.gameObject.SetActive(isMyTurn);
        }).AddTo(this);
        this.combatParticipant.IsMyTurnStartObs.DistinctUntilChanged().Subscribe(isMyTurnStart =>
        {
            this.skillbar.skillbarUI.gameObject.SetActive(isMyTurnStart);
            this.skillbar.potionsUI.gameObject.SetActive(isMyTurnStart);
        }).AddTo(this);
        this.combatParticipant.IsMyTurnConfirmActionObs.DistinctUntilChanged().Subscribe(isMyTurnConfirm =>
        {
            this.skillbar.turnConfirmButton.gameObject.SetActive(isMyTurnConfirm);
        }).AddTo(this);
        Observable.CombineLatest(this.combatParticipant.IsMyTurnSelectTargetObs, this.combatParticipant.IsMyTurnConfirmActionObs).Subscribe(tuple =>
        {
            var isMyTurnSelectTarget = tuple[0];
            var isMyTurnConfirmAction = tuple[1];
            this.skillbar.skillInfo.gameObject.SetActive(isMyTurnSelectTarget || isMyTurnConfirmAction);
        }).AddTo(this);
    }

    private void ResetHomeSpots()
    {
        this.combatParticipant.homeSpot = this.playerAnimation.animancerComponent.transform.position;
        this.combatParticipant.homeRotation = this.playerAnimation.animancerComponent.transform.rotation;
    }

    private void ComputeStatsFromEquipment(CharEquipment equipment)
    {
        // Example logic to get stats from equipment
        if (equipment != null)
        {
            var armorSOGO = this.equipmentList.GetSOGO(equipment.Armor);
            var weaponSOGO = this.equipmentList.GetSOGO(equipment.Weapon);
            // armor
            this.combatParticipant.armorSOGO = armorSOGO;
            this.combatParticipant.MaxHealth = armorSOGO.Item1.Health;
            this.combatParticipant.CurrentHealth = armorSOGO.Item1.Health;
            this.combatParticipant.Defense = armorSOGO.Item1.Defense;
            // weapon
            this.combatParticipant.weaponSOGO = weaponSOGO;
            this.combatParticipant.Damage = weaponSOGO.Item1.Damage;
        }
    }

    [ContextMenu("Test >>> Turn")]
    public void ForceTurn()
    {
        var controller = this.GetCombatController();
        var state = controller.CombatStateObs.Value;
        state.SelectedSkillBook = null;
        state.SkillTargets = new();
        state.TurnIndex = state.ShuffledParticipants.IndexOf(this.combatParticipant);
        state.Phase = CombatPhase.TurnStart;
        controller.SetCombatState(state);
    }

    [ContextMenu("Test >>> Play")]
    public void TestPlay()
    {
        var attack01 = new SwordAttack01(
            this.combatParticipant,
            this.playerAnimation.animancerComponent,
            this.playerAnimation.animationList
        );
        var dash = this.CreateDashState();
        var idle = this.CreateIdleState();
        var back = this.CreateBackState();
        var combined = new CombineState(dash, idle, attack01, back, idle);
        this.playerAnimation.rxStateMachine.SetState(combined);
    }

    [ContextMenu("Test >>> Home")]
    public void BackToHome()
    {
        this.playerAnimation.animancerComponent.transform.position = this.combatParticipant.homeSpot;
        this.playerAnimation.animancerComponent.transform.rotation = this.combatParticipant.homeRotation;
        this.playerAnimation.PlayIdleAnimation();
        this.ForceTurn();
    }

    public IRxState CreateIdleState()
    {
        var idleState = new IdleState(
            this.playerAnimation.animancerComponent,
            this.combatParticipant.weaponSOGO.Item1,
            this.playerAnimation.animationList,
            200
        );

        return idleState;
    }

    public IRxState CreateDashState()
    {
        var dash = new DashToTargetState(
            this.combatParticipant,
            this.playerAnimation.animancerComponent,
            this.playerAnimation.animationList
        );
        return dash;
    }

    public IRxState CreateBackState()
    {
        var back = new DashBackHomeState(
            this.combatParticipant,
            this.playerAnimation.animancerComponent,
            this.playerAnimation.animationList
        );
        return back;
    }

    public void PerformTurnAction(string skillSOId)
    {
        var state = skillSOId switch
        {
            "beginner_slash" => this.BeginnerSlash(),
            _ => throw new System.Exception("uknown skill: " + skillSOId),
        };
        this.playerAnimation.rxStateMachine.SetState(state);
    }
}
