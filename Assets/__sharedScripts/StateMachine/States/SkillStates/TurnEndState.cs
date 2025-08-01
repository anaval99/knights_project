using System;
using R3;

public class TurnEndState : IRxState
{
    public string Name { get; set; } = "turn_end";
    private CombatParticipant combatParticipant;

    public TurnEndState(CombatParticipant combatParticipant)
    {
        this.combatParticipant = combatParticipant;
    }

    public Observable<int> Play()
    {
        return Observable.Defer(() =>
        {
            var ctl = this.combatParticipant.combatController;
            return Observable.Timer(TimeSpan.FromMilliseconds(200)).Select(_ => 1).Take(1).Do(_ =>
            {
                var state = ctl.CombatStateObs.Value;
                state.Phase = CombatPhase.TurnEnd;
                ctl.SetCombatState(state);
            });
        });
    }
}