using Animancer;
using R3;

public class RotateBackHomeState : IRxState
{
    private CombatParticipant player;
    private AnimancerComponent animancer;
    private AnimationList animationList;
    public string Name { get; set; } = "rotate";
    public RotateBackHomeState(
        CombatParticipant player, AnimancerComponent animancer, AnimationList animationList)
    {
        this.player = player;
        this.animancer = animancer;
        this.animationList = animationList;
    }

    public Observable<int> Play()
    {
        return Observable.Defer(() =>
        {
            return animancer.RotateSmoothly(player.homeRotation, 0.5f);
        });
    }
}
