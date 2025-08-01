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
    private AnimationClip idleAnimation;
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
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PlayIdleAnimation()
    {
        if (animancer == null || idleAnimation == null)
        {
            Debug.LogError("Animancer or Idle Animation is not set.");
            return;
        }
        var idleState = new EnemyIdleState(
            animancer,
            idleAnimation);
        rxStateMachine.SetState(idleState);
    }
}
