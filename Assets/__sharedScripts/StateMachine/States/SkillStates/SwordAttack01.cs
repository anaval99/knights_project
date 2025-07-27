using System;
using Animancer;
using R3;

public class SwordAttack01 : IRxState
{
    private CombatParticipant player;
    private AnimancerComponent animancer;
    private AnimationList animationList;
    public SwordAttack01(
        CombatParticipant player, AnimancerComponent animancer, AnimationList animationList)
    {
        this.player = player;
        this.animancer = animancer;
        this.animationList = animationList;
    }

    public string Name { get; set; } = "SwordAttack01";

    public Observable<int> Play()
    {
        var clipName = PlayerAnims.Attack01.WithWeapon(player.weaponSOGO.Item1.WeaponClass);
        var clip = this.animationList.GetClip(clipName);
        var controller = player.combatController;
        var state = controller.CombatStateObs.Value;
        var target = state.SkillTargets[0];
        return Observable.Defer(() =>
        {
            this.animancer.Play(clip);
            var onhitObs = Observable.Timer(TimeSpan.FromMilliseconds(clip.length / 2 * 1000))
                .Take(1).Select(_ => 1)
                .Do(_ => controller.TriggerOnHit(new OnHitEvent
                {
                    Source = controller.CurrentParticipant,
                    Target = target
                }));
            var clipEndObs = Observable.Timer(TimeSpan.FromMilliseconds(clip.length * 1000)).Take(1).Select(_ => 1);
            return Observable.Merge(onhitObs, clipEndObs);
        });
    }
}