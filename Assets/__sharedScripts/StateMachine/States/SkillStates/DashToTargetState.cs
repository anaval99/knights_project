using System.Collections.Generic;
using Animancer;
using R3;
using UnityEngine;

public class DashToTargetState : IRxState
{
    public string Name { get; set; } = "dash_forward";

    private CombatParticipant _participant;
    private AnimancerComponent _animancer;
    private AnimationClip _clip;
    public DashToTargetState(
        CombatParticipant participant, AnimancerComponent animancer, AnimationClip clip)
    {
        this._participant = participant;
        this._animancer = animancer;
        this._clip = clip;
    }

    public Observable<int> Play()
    {
        var target = _participant.combatController.CombatStateObs.Value.SkillTargets[0];
        var startLocation = _animancer.transform.position;
        var toSpot = this.GetTargetSpot(target);

        return Observable.Defer(() =>
        {
            this._animancer.Play(this._clip);
            return _animancer.MoveTowards(startLocation, toSpot, this._clip.length);
        });
    }

    public Vector3 GetTargetSpot(CombatParticipant target)
    {
        // Define your desired melee range
        float meleeRange = _participant.meleeRange;
        // Get the closest point on the target's collider to the player's position
        Vector3 closestPoint = target.Collider.ClosestPoint(_participant.transform.position);
        // Get the direction from the target's surface (closest point) to the player's position
        Vector3 directionFromSurface = (_participant.transform.position - closestPoint).normalized;
        // Calculate the final destination point by moving back from the closest point on the collider's surface
        var toLocation = closestPoint + directionFromSurface * meleeRange;
        // Ensure the player stays on their homeSpot's y-level
        toLocation.y = _participant.homeSpot.y;
        toLocation.z = target.Collider.bounds.center.z;
        return toLocation;
    }
}
