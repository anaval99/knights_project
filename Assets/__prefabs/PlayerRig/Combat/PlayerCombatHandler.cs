using System.Collections.Generic;
using System.Linq;
using Animancer.FSM;
using R3;
using Unity.VisualScripting;
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
    private Skillbar skillbar;

    private CombatState currentState;
    private CombatController controller;

    public void InitializeCombatHandler(Observable<CombatState> combatStateObs)
    {
        combatStateObs.Subscribe(state =>
        {
            this.currentState = state;
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
                        this.PerformTurnAction(state.SelectedSkillBook);
                    }
                    break;
                case CombatPhase.FinalBattleEnd:
                    this.VictoryPose();
                    break;
                case CombatPhase.None:
                default:

                    break;
            }
        })
        .AddTo(this);

        this.controller = this.GetCombatController();
        if (controller != null)
        {
            controller.OnHitObs.Where(ev => ev.Target == this.combatParticipant).Subscribe(this.HandleHit).AddTo(this);
        }
    }

    void Start()
    {
        this.charDataCenter.CharEquipmentObs.Subscribe(equipment => this.ComputeStatsFromEquipment(equipment)).AddTo(this);
        this.combatParticipant.IsMyTurnObs.DistinctUntilChanged().Subscribe(isMyTurn =>
        {
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

    void HandleHit(OnHitEvent ev)
    {
        bool isAlive = this.combatParticipant.OnHit(ev);
        if (isAlive)
        {
            var hitClip = this.GetClipWithWeapon(PlayerAnims.GetHit02);
            var hitState = new PlayClipState()
            {
                Animancer = this.playerAnimation.animancerComponent,
                AnimationClip = hitClip
            };
            this.playerAnimation.rxStateMachine.SetState(new CombineState(
                hitState,
                this.CreateIdleState()
            ));
        }
        else
        {
            var deathClip = this.GetClipWithWeapon(PlayerAnims.Die02);
            this.playerAnimation.rxStateMachine.SetState(new PlayClipState()
            {
                Animancer = this.playerAnimation.animancerComponent,
                AnimationClip = deathClip
            });
            this.controller.TriggerOnDeath(new OnDeathEvent
            {
                DeadParticipant = this.combatParticipant
            });            
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

    [ContextMenu("Test >>> Home")]
    public void BackToHome()
    {
        this.playerAnimation.animancerComponent.transform.position = this.combatParticipant.homeSpot;
        this.playerAnimation.animancerComponent.transform.rotation = this.combatParticipant.homeRotation;
        this.playerAnimation.PlayIdleAnimation();
        this.ForceTurn();
    }

    [ContextMenu("Test >>> Projectile")]
    public void TestProjectile()
    {
        var target = this.currentState.SkillTargets[0];
        this.combatParticipant.ProjectileMaker.CreateProjectile(ProjectileType.Rocket, target);
    }

    public AnimationClip GetClipWithWeapon(string animName)
    {
        var clipName = animName.WithWeapon(this.combatParticipant.weaponSOGO);
        var clip = this.playerAnimation.animationList.GetClip(clipName);
        return clip;
    }

    public IRxState CreateIdleState()
    {
        var clip = this.GetClipWithWeapon(PlayerAnims.Idle_Battle);
        var idleState = new IdleState(
            this.playerAnimation.animancerComponent,
            clip,
            200
        );

        return idleState;
    }

    public IRxState LookAtTargetState()
    {
        var state = new LookAtState(
            this.combatParticipant,
            this.playerAnimation.animancerComponent
        );

        return state;
    }

    public IRxState RotateBackHomeState()
    {
        var state = new RotateBackHomeState(
            this.combatParticipant,
            this.playerAnimation.animancerComponent
        );

        return state;
    }

    public IRxState CreateDashState()
    {
        var clip = this.GetClipWithWeapon(PlayerAnims.DashFWD_Battle_InPlace);
        var dash = new DashToTargetState(
            this.combatParticipant,
            this.playerAnimation.animancerComponent,
            clip
        );
        return dash;
    }

    public IRxState CreateBackState()
    {
        var back = new DashBackHomeState(
            this.combatParticipant,
            this.playerAnimation.animancerComponent,
            this.GetClipWithWeapon(PlayerAnims.DashBWD_Battle_InPlace)
        );
        return back;
    }

    public IRxState PerformPotion(SkillBookSO skillBookSO)
    {
        var targets = this.controller.State.SkillTargets;
        var heals = targets.Select(target => new TriggerHealState()
        {
            Healer = this.combatParticipant,
            SkillBookSO = this.controller.State.SelectedSkillBook,
            Target = target,
            HP = skillBookSO.SkillType == SkillType.HPPotion ? skillBookSO.HealAmount : 0,
            MP = skillBookSO.SkillType == SkillType.MPPotion ? skillBookSO.HealAmount : 0,
            Name = skillBookSO.name + "State"
        });
        var idle = this.CreateIdleState();
        var end = new TurnEndState(this.combatParticipant);
        return new CombineState(
            idle,
            new CombineState(true, heals.ToArray()),
            end
        );
    }

    public IRxState PerformRangeAttack01(SkillBookSO skillBookSO)
    {
        var idle = this.CreateIdleState();
        var lookAt = this.LookAtTargetState();
        var atk = this.RangeAttack01(skillBookSO);
        var backHome = this.RotateBackHomeState();
        var state = new CombineState(
            lookAt,
            idle,
            atk,
            backHome,
            idle,
            new TurnEndState(this.combatParticipant)
        );
        return state;
    }

    public IRxState RangeAttack01(SkillBookSO skillBookSO)
    {
        var attack01 = new RangeAttack01()
        {
            Player = this.combatParticipant,
            Animancer = this.playerAnimation.animancerComponent,
            AnimationList = this.playerAnimation.animationList,
            SkillBookSO = skillBookSO,
        };
        return attack01;
    }

    public void PerformTurnAction(SkillBookSO skillBookSO)
    {
        if (skillBookSO.SkillType == SkillType.HPPotion || skillBookSO.SkillType == SkillType.MPPotion)
        {
            this.playerAnimation.rxStateMachine.SetState(this.PerformPotion(skillBookSO));
            return;
        }
        var state = skillBookSO.name switch
        {
            "beginner_slash" => this.BeginnerSlash(skillBookSO),
            "beginner_shot" => this.PerformRangeAttack01(skillBookSO),
            "beginner_bolt" => this.PerformRangeAttack01(skillBookSO),
            _ => throw new System.Exception("uknown skill: " + skillBookSO.name),
        };
        this.playerAnimation.rxStateMachine.SetState(state);
    }

    public void VictoryPose()
    {
        var clip = this.playerAnimation.animationList.GetClip(PlayerAnims.Victory.WithWeapon(this.combatParticipant.weaponSOGO));
        var state = new PlayClipState()
        {
            Animancer = this.playerAnimation.animancerComponent,
            AnimationClip = clip
        };
        this.playerAnimation.rxStateMachine.SetState(state);
    }
}
