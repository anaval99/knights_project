using Animancer;
using R3;
using UnityEngine;

public class DashToTargetState : IRxState
{
    private CombatParticipant player;
    private AnimancerComponent animancer;
    private AnimationList animationList;
    public string Name { get; set; } = "dash_forward";
    public DashToTargetState(
        CombatParticipant player, AnimancerComponent animancer, AnimationList animationList)
    {
        this.player = player;
        this.animancer = animancer;
        this.animationList = animationList;
    }

    public Observable<int> Play()
    {
        var clipName = PlayerAnims.DashFWD_Battle_InPlace.WithWeapon(player.weaponSOGO.Item1.WeaponClass);
        var clip = this.animationList.GetClip(clipName);
        var target = player.combatController.CombatStateObs.Value.SkillTargets[0];
        var startLocation = animancer.transform.position;

        // Define your desired melee range
        float meleeRange = player.meleeRange;
        // Get the closest point on the target's collider to the player's position
        Vector3 closestPoint = target.Collider.ClosestPoint(player.transform.position);
        // Get the direction from the target's surface (closest point) to the player's position
        Vector3 directionFromSurface = (player.transform.position - closestPoint).normalized;
        // Calculate the final destination point by moving back from the closest point on the collider's surface
        var toLocation = closestPoint + directionFromSurface * meleeRange;
        // Ensure the player stays on their homeSpot's y-level
        toLocation.y = player.homeSpot.y;
        toLocation.z = target.Collider.bounds.center.z;

        return Observable.Defer(() =>
        {
            this.animancer.Play(clip);
            return animancer.MoveTowards(startLocation, toLocation, clip.length);
        });
    }
}
