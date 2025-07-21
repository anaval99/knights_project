using Animancer;
using R3;

public class IdleState: IRxState
{
    private readonly AnimancerComponent _animancer;
    private readonly ItemSO _weaponSO;
    private readonly AnimationList _animationList;

    public IdleState(
        AnimancerComponent animancer,
        ItemSO weaponSO,
        AnimationList animationList
    )
    {
        _animancer = animancer;
        _weaponSO = weaponSO;
        _animationList = animationList;
    }

    public string Name { get; set; } = "IdleState";

    public Observable<int> Play()
    {
        return Observable.Defer(() =>
        {
            string idleClip = PlayerAnims.Idle_Battle.WithWeapon(_weaponSO.WeaponClass);
            var clip = this._animationList.GetClip(idleClip);
            this._animancer.Play(clip, 0.1f);
            return Observable.Never<int>();
        });
    }
}