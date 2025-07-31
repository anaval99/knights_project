using Animancer;
using R3;

public class LookAtState : IRxState
{
    private CombatParticipant player;
    private AnimancerComponent animancer;
    private AnimationList animationList;
    public string Name { get; set; } = "look_at";
    public LookAtState(
        CombatParticipant player, AnimancerComponent animancer, AnimationList animationList)
    {
        this.player = player;
        this.animancer = animancer;
        this.animationList = animationList;
    }

    public Observable<int> Play()
    {
        var target = player.combatController.CombatStateObs.Value.SkillTargets[0];
        return Observable.Defer(() =>
        {
            return animancer.LookAtSmoothly(target.Collider.bounds.center, .5f);
        });
    }
}
