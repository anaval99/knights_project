using Animancer;
using R3;

public class PatrolState : IRxState
{
    private readonly AnimancerComponent _animancer;
    private readonly ItemSO _weaponSO;
    private readonly AnimationList _animationList;
    
    public PatrolState(
        AnimancerComponent animancer,
        ItemSO weaponSO,
        AnimationList animationList
    )
    {
        _animancer = animancer;
        _weaponSO = weaponSO;
        _animationList = animationList;
        // Initialization code if needed
    }

    public string Name { get; set; } = "PatrolState";

    public Observable<int> Play()
    {
        return Observable.Defer(() =>
        {
            string patrolClip = PlayerAnims.SprintFWD_Battle_InPlace.WithWeapon(_weaponSO.WeaponClass);
            var clip = this._animationList.GetClip(patrolClip);
            this._animancer.Play(clip, 0.1f);
            return Observable.Never<int>();
        });
    }
}