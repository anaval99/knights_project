using Animancer;
using R3;
using UnityEngine;

public class DashBackHomeState : IRxState
{
    private CombatParticipant _participant;
    private AnimancerComponent _animancer;
    private AnimationClip _clip;
    public string Name { get; set; } = "dash_home";
    public DashBackHomeState(
        CombatParticipant player, AnimancerComponent animancer, AnimationClip clip)
    {
        this._participant = player;
        this._animancer = animancer;
        this._clip = clip;
    }

    public Observable<int> Play()
    {
        var currentLocation = _animancer.transform.position;
        return Observable.Defer(() =>
        {
            this._animancer.Play(this._clip);
            return _animancer.MoveTowards(currentLocation, _participant.homeSpot, this._clip.length);
        });
    }
}