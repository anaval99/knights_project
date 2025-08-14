using System;
using Animancer;
using R3;
using UnityEngine;

public class PlayClipState : IRxState
{
    public AnimancerComponent Animancer;
    public AnimationClip AnimationClip;
    public string Name { get; set; } = "play_clip";

    public Observable<int> Play()
    {
        if (this.AnimationClip == null)
        {
            return Observable.Timer(TimeSpan.FromMilliseconds(500)).Take(1).Select(_ => 1);
        }

        return Observable.Defer(() =>
        {
            return Animancer.PlayAsObservable(this.AnimationClip);
        });
    }
}