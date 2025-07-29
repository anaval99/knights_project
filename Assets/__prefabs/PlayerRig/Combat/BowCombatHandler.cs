public partial class PlayerCombatHandler
{
    public IRxState BeginnerShot()
    {
        var idle = this.CreateIdleState();
        var atk = this.BowAttack01();
        var state = new CombineState(
            idle,
            atk,
            idle
        );
        return state;
    }

    public IRxState BowAttack01()
    {
        var attack01 = new BowAttack01(
            this.combatParticipant,
            this.playerAnimation.animancerComponent,
            this.playerAnimation.animationList
        );
        return attack01;
    }
}
