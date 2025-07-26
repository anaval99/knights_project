using System;
using Animancer;
using R3;

public class IdleState: IRxState
{
    private readonly AnimancerComponent _animancer;
    private readonly ItemSO _weaponSO;
    private readonly AnimationList _animationList;
    private int durationMS = 0;

    public IdleState(
        AnimancerComponent animancer,
        ItemSO weaponSO,
        AnimationList animationList,
        int duration = 0
    )
    {
        _animancer = animancer;
        _weaponSO = weaponSO;
        _animationList = animationList;
        this.durationMS = duration;
    }

    public string Name { get; set; } = "IdleState";

    public Observable<int> Play()
    {
        return Observable.Defer(() =>
        {
            string idleClip = PlayerAnims.Idle_Battle.WithWeapon(_weaponSO.WeaponClass);
            var clip = this._animationList.GetClip(idleClip);
            this._animancer.Play(clip, 0.1f);
            if (this.durationMS > 0)
            {
                return Observable.Timer(TimeSpan.FromMilliseconds(this.durationMS)).Select(_ => 1);
            }
            return Observable.Never<int>();
        });
    }
}