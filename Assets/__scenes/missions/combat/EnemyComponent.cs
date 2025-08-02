using Animancer;
using Animancer.FSM;
using R3;
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
                Debug.Log("Enemey start:" + this.CombatParticipant.name);
                var state = combatController.State;
                state.Phase = CombatPhase.TurnEnd;
                combatController.SetCombatState(state);
            }).AddTo(this);

            this.combatController.OnHitObs.Where(x => x.Target == this.CombatParticipant && this.enemyDefinitionSO.OnHitAnimation != null)
                .Subscribe(x =>
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
                    this.rxStateMachine.SetState(onHitState);
                }).AddTo(this);
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
            Debug.LogError("Animancer or Idle Animation is not set.");
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
}
