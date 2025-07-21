using R3;
using UnityEngine;

public class PlayerCombatHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerAnimation playerAnimation;

    public void InitializeCombatHandler(Observable<CombatState> combatStateObs)
    {
        combatStateObs.Subscribe(state =>
        {
            switch (state.Phase)
            {
                case CombatPhase.Start:
                    // Handle combat start logic
                    Debug.Log("Combat started.");
                    break;
                case CombatPhase.Patrolling:
                    // Handle patrolling logic
                    Debug.Log("Player is patrolling.");
                    this.playerAnimation.PlayPatrollingAnimation();
                    break;
                case CombatPhase.BattleStart:
                    // Handle battle start logic
                    Debug.Log("Battle has started.");
                    this.playerAnimation.PlayIdleAnimation();
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
}
