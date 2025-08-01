using System;
using Animancer;
using R3;

public class RangeAttack01: IRxState
{
    private CombatParticipant player;
    private AnimancerComponent animancer;
    private AnimationList animationList;
    public RangeAttack01(
        CombatParticipant player, AnimancerComponent animancer, AnimationList animationList)
    {
        this.player = player;
        this.animancer = animancer;
        this.animationList = animationList;
    }

    public string Name { get; set; } = "RangeAttack01";

    public Observable<int> Play()
    {
        var clipName = PlayerAnims.Attack01.WithWeapon(player.weaponSOGO);
        var clip = this.animationList.GetClip(clipName);
        var controller = player.combatController;
        var state = controller.CombatStateObs.Value;
        var target = state.SkillTargets[0];
        return Observable.Defer(() =>
        {
            var playObs = this.animancer.PlayAsObservable(clip);
            var onhitObs = Observable.Timer(TimeSpan.FromMilliseconds(clip.length / 2 * 1000))
                .Take(1).Select(_ => player.ProjectileMaker.CreateProjectile("Rocket", target).Do(onCompleted: _ =>
                {
                    controller.TriggerOnHit(new OnHitEvent
                    {
                        Source = controller.CurrentParticipant,
                        Target = target
                    });
                }))
                .Switch();
            return Observable.Merge(onhitObs, playObs);
        });
    }
}