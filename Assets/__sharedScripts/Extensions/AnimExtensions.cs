using System;
using Animancer;
using R3;
using UnityEngine;

public static class AnimExtensions
{
    public static Observable<int> PlayAsObservable(this AnimancerComponent anim, AnimationClip clip)
    {
        return Observable.Defer(() =>
        {
            anim.Play(clip);
            var clipEndObs = Observable.Timer(TimeSpan.FromMilliseconds(clip.length * 1000)).Take(1).Select(_ => 1);
            return clipEndObs;
        });
    }
}