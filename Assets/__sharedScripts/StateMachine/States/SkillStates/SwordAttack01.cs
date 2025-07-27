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
        return Observable.Defer(() =>
        {
            this.animancer.Play(clip);
            return Observable.Timer(TimeSpan.FromMilliseconds(clip.length * 1000)).Take(1).Select(_ => 1);
        });
    }
}