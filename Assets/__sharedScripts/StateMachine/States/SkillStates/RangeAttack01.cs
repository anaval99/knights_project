using System;
using Animancer;
using R3;

public class RangeAttack01: IRxState
{
    public CombatParticipant Player;
    public AnimancerComponent Animancer;
    public AnimationList AnimationList;
    public SkillBookSO SkillBookSO;
    public RangeAttack01(
        )
    {

    }

    public string Name { get; set; } = "RangeAttack01";

    public Observable<int> Play()
    {
        var clipName = PlayerAnims.Attack01.WithWeapon(Player.weaponSOGO);
        var clip = this.AnimationList.GetClip(clipName);
        var controller = Player.combatController;
        var state = controller.CombatStateObs.Value;
        var target = state.SkillTargets[0];
        return Observable.Defer(() =>
        {
            var playObs = this.Animancer.PlayAsObservable(clip);
            var onhitObs = Observable.Timer(TimeSpan.FromMilliseconds(clip.length / 2 * 1000))
                .Take(1).Select(_ => Player.ProjectileMaker.CreateProjectile("Rocket", target).Do(onCompleted: _ =>
                {
                    controller.TriggerOnHit(new OnHitEvent
                    {
                        SkillBookSO = this.SkillBookSO,
                        Source = controller.CurrentParticipant,
                        Target = target
                    });
                }))
                .Switch();
            return Observable.Merge(onhitObs, playObs);
        });
    }
}