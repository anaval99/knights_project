using Animancer;
using R3;

public class DashBackHomeState : IRxState
{
    private CombatParticipant player;
    private AnimancerComponent animancer;
    private AnimationList animationList;
    public string Name { get; set; } = "dash_home";
    public DashBackHomeState(
        CombatParticipant player, AnimancerComponent animancer, AnimationList animationList)
    {
        this.player = player;
        this.animancer = animancer;
        this.animationList = animationList;
    }

    public Observable<int> Play()
    {
        var clipName = PlayerAnims.DashBWD_Battle_InPlace.WithWeapon(player.weaponSOGO.Item1.WeaponClass);
        var clip = this.animationList.GetClip(clipName);
        var currentLocation = animancer.transform.position;
        return Observable.Defer(() =>
        {
            this.animancer.Play(clip);
            return animancer.MoveTowards(currentLocation, player.homeSpot, clip.length);
        });
    }
}