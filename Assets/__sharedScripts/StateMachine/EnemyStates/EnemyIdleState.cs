using Animancer;
using R3;
using UnityEngine;

public class EnemyIdleState: IRxState
{
    private readonly AnimancerComponent _animancer;
    private readonly AnimationClip _idleAnimation;

    public EnemyIdleState(
        AnimancerComponent animancer,
        AnimationClip idleAnimation
    )
    {
        _animancer = animancer;
        _idleAnimation = idleAnimation;
    }

    public string Name { get; set; } = "EnemyIdleState";

    public Observable<int> Play()
    {
        return Observable.Defer(() =>
        {
            this._animancer.Play(this._idleAnimation);
            return Observable.Never<int>();
        });
    }

    Observable<int> IRxState.Play()
    {
        throw new System.NotImplementedException();
    }
}