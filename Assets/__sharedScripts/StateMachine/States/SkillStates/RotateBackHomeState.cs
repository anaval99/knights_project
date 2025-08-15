using Animancer;
using R3;

public class RotateBackHomeState : IRxState
{
    private CombatParticipant player;
    private AnimancerComponent animancer;
    public string Name { get; set; } = "rotate";
    public RotateBackHomeState(
        CombatParticipant player, AnimancerComponent animancer)
    {
        this.player = player;
        this.animancer = animancer;
    }

    public Observable<int> Play()
    {
        return Observable.Defer(() =>
        {
            return animancer.RotateSmoothly(player.homeRotation, 0.5f);
        });
    }
}
