using System;
using Animancer;
using R3;
using UnityEngine;

public class IdleState: IRxState
{
    private readonly AnimancerComponent _animancer;
    private AnimationClip _clip;
    private int _durationMS = 0;

    public IdleState(
        AnimancerComponent animancer,
        AnimationClip clip,
        int durationMS = 0
    )
    {
        this._animancer = animancer;
        this._clip = clip;
        this._durationMS = durationMS;
    }

    public string Name { get; set; } = "IdleState";

    public Observable<int> Play()
    {
        return Observable.Defer(() =>
        {
            this._animancer.Play(this._clip, 0.1f);
            if (this._durationMS > 0)
            {
                return Observable.Timer(TimeSpan.FromMilliseconds(this._durationMS)).Select(_ => 1).Take(1);
            }
            return Observable.Never<int>();
        });
    }
}