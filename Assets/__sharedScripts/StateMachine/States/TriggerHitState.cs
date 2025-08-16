using System;
using R3;

public class TriggerHitState : IRxState
{
    public string Name { get; set; } = "TriggerHit";
    public CombatParticipant Source;
    public CombatParticipant Target;
    public int Damage;
    public float TimingSeconds;


    public Observable<int> Play()
    {
        return Observable.Defer(() =>
        {
            var controller = this.Source.GetCombatController();
            var onHitEv = new OnHitEvent()
            {
                Damage = this.Damage,
                Source = this.Source,
                Target = this.Target
            };
            return Observable.Timer(TimeSpan.FromMilliseconds(this.TimingSeconds * 1000))
                .Take(1)
                .Select(_ =>
                {
                    controller.TriggerOnHit(onHitEv);
                    return 1;
                });
        });
    }
}