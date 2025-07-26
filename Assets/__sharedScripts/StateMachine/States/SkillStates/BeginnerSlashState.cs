using Animancer;
using R3;

public class BeginnerSlashState : IRxState
{
    private CombatParticipant player;
    private AnimancerComponent animancer;
    private AnimationList animationList;
    public string Name { get; set; } = "beginner_slash";
    public BeginnerSlashState(
        CombatParticipant player, AnimancerComponent animancer, AnimationList animationList)
    {
        this.player = player;
        this.animancer = animancer;
        this.animationList = animationList;
    }


    public Observable<int> Play()
    {
        var controller = this.player.combatController;
        var target = controller.CombatStateObs.Value.SkillTargets[0];
        throw new System.NotImplementedException();
    }

    public Observable<int> DashForward()
    {
        string idleClip = PlayerAnims.DashFWD_Battle_InPlace.WithWeapon(this.player.weaponSOGO.Item1.WeaponClass);
        var clip = this.animationList.GetClip(idleClip);
        var animState = this.animancer.Play(clip);
        return Observable.Never<int>();
    }
}