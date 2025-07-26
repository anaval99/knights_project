using Animancer;
using R3;

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
        var toLocation = target.meleeLandingSpot;
        toLocation.y = player.homeSpot.y;
        return Observable.Defer(() =>
        {
            this.animancer.Play(clip);
            return animancer.MoveTowards(startLocation, toLocation, clip.length);
        });
    }
}
