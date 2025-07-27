public partial class PlayerCombatHandler
{
    public IRxState BeginnerSlash()
    {
        var dash = this.CreateDashState();
        var idle = this.CreateIdleState();
        var atk = this.Attack01();
        var back = this.CreateBackState();
        var state = new CombineState(
            dash,
            idle,
            atk,
            back,
            idle
        );
        return state;
    }

    public IRxState Attack01()
    {
        var attack01 = new SwordAttack01(
            this.combatParticipant,
            this.playerAnimation.animancerComponent,
            this.playerAnimation.animationList
        );
        return attack01;
    }
}
