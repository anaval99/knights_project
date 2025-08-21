using System.Collections.Generic;
using Animancer;
using Animancer.FSM;
using R3;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(RxStateMachine))]
public class EnemyComponent : MonoBehaviour
{
    [SerializeField]
    private AnimancerComponent animancer;
    [SerializeField]
    private EnemyDefinitionSO enemyDefinitionSO;
    [SerializeField]
    public CombatParticipant CombatParticipant;
    [SerializeField]
    public int TestAtkIndex = 0;

    CombatController combatController;

    private RxStateMachine rxStateMachine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rxStateMachine = GetComponent<RxStateMachine>();
        combatController = this.CombatParticipant.GetCombatController();
        if (combatController != null)
        {
            this.PlayIdleAnimation();

            this.CombatParticipant.IsMyTurnStartObs.Subscribe(isMyTurn =>
            {
                if (!isMyTurn)
                {
                    return;
                }
                this.CombatParticipant.homeSpot = this.animancer.transform.position;
                this.CombatParticipant.homeRotation = this.animancer.transform.rotation;
                var hasAttacks = this.enemyDefinitionSO.GenericEnemyAttacks != null
                    && this.enemyDefinitionSO.GenericEnemyAttacks.Length > 0;
                Debug.Log("Enemy start:" + this.enemyDefinitionSO.name);
                var state = combatController.State.Clone();
                if (!hasAttacks)
                {
                    state.Phase = CombatPhase.TurnEnd;
                    combatController.SetCombatState(state);
                }
                else
                {
                    var target = state.PlayerParticipants[0];
                    state.Phase = CombatPhase.TurnAction;
                    state.SkillTargets = new List<CombatParticipant>() { target };
                    combatController.SetCombatState(state);
                    Debug.Log("Enemy target:" + target.gameObject.name);
                }
            }).AddTo(this);

            this.combatController.OnHitObs.Where(ev => ev.Target == this.CombatParticipant).Subscribe(this.HandleHit).AddTo(this);
        }
    }

    [ContextMenu("SetTurn()")]
    public void SetTurn()
    {
        var controller = this.GetCombatController();
        var state = controller.CombatStateObs.Value.Clone();
        state.SelectedSkillBook = null;
        state.SkillTargets = new();
        state.TurnIndex = state.ShuffledParticipants.IndexOf(this.CombatParticipant);
        state.Phase = CombatPhase.TurnStart;
        controller.SetCombatState(state);        
    }

    [ContextMenu("TestAtk()")]
    public void TestAtk()
    {
        this.PerformGenericAttack(this.enemyDefinitionSO.GenericEnemyAttacks[this.TestAtkIndex]);
    }

    public void PerformGenericAttack(GenericEnemyAttack atk)
    {
        var actualAtk = new PlayClipState()
        {
            Animancer = this.animancer,
            AnimationClip = atk.Animation,
        };
        var turnEnd = new TurnEndState(this.CombatParticipant);
        var idle = new IdleState(this.animancer, this.enemyDefinitionSO.IdleAnimation, 200);
        var target = this.combatController.State.SkillTargets[0];
        if (atk.IsMelee)
        {
            var dash = new DashToTargetState(this.CombatParticipant, this.animancer, this.enemyDefinitionSO.DashAnimation);
            var back = new DashBackHomeState(this.CombatParticipant, this.animancer, this.enemyDefinitionSO.BackAnimation);
            var hit = new TriggerHitState()
            {
                Damage = atk.Damage,
                Source = this.CombatParticipant,
                Target = target,
                TimingSeconds = atk.Animation.length / 2
            };
            this.rxStateMachine.SetState(new CombineState(
                dash,
                new CombineState(true, actualAtk, hit),
                back,
                idle,
                turnEnd
            ));
        }
        else
        {
            var lookat = new LookAtState(this.CombatParticipant, this.animancer);
            var rotateBack = new RotateBackHomeState(this.CombatParticipant, this.animancer);
            var hit = new TriggerProjectileState()
            {
                Damage = atk.Damage,
                Name = atk.AttackName,
                Source = this.CombatParticipant,
                Target = target,
                TimingSeconds = atk.Animation.length / 2,
                ProjectileType = atk.ProjectileType
            };
            this.rxStateMachine.SetState(new CombineState(
                lookat,
                new CombineState(true, hit, actualAtk),
                rotateBack,
                idle,
                turnEnd
            ));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PlayIdleAnimation()
    {
        if (animancer == null || this.enemyDefinitionSO.IdleAnimation == null)
        {
            return;
        }

        rxStateMachine.SetState(this.IdleState());
    }

    private IRxState IdleState()
    {
        return new PlayClipState()
        {
            Animancer = this.animancer,
            AnimationClip = this.enemyDefinitionSO.IdleAnimation
        };
    }

    private IRxState DeathState()
    {
        return new PlayClipState()
        {
            Animancer = this.animancer,
            AnimationClip = this.enemyDefinitionSO.DeathAnimation
        };
    }

    private void HandleHit(OnHitEvent ev)
    {
        bool isAlive = this.CombatParticipant.OnHit(ev);
        if (isAlive)
        {
            var onHitState = new PlayClipState()
            {
                Animancer = this.animancer,
                AnimationClip = this.enemyDefinitionSO.OnHitAnimation,
            };
            var state = new CombineState(
                onHitState,
                this.IdleState()
            );
            this.rxStateMachine.SetState(state);
        }
        else
        {
            this.rxStateMachine.SetState(this.DeathState());
            this.combatController.TriggerOnDeath(new OnDeathEvent
            {
                DeadParticipant = this.CombatParticipant
            });
        }
    }
}
