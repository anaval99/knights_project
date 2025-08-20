using System;
using R3;

public class TriggerHealState : IRxState
{
    public string Name { get; set; } = "TriggerHealState";
    public CombatParticipant Healer;
    public CombatParticipant Target;
    public SkillBookSO SkillBookSO;
    public int HP;
    public int MP;

    public Observable<int> Play()
    {
        var controller = Healer.GetCombatController();
        return Observable.Defer(() =>
        {
            return Observable.Timer(TimeSpan.FromSeconds(0.3)).Take(1).Select(_ => 1)
                .Do(_ =>
                {
                    var healEv = new OnHealEvent
                    {
                        HP = this.HP,
                        MP = this.MP,
                        SkillBookSO = this.SkillBookSO,
                        Target = this.Target
                    };
                    controller.TriggerOnHeal(healEv);
                });
        });
    }
}