public partial class PlayerCombatHandler
{
    public IRxState BeginnerShot()
    {
        var idle = this.CreateIdleState();
        var lookAt = this.LookAtTargetState();
        var atk = this.BowAttack01();
        var backHome = this.RotateBackHomeState();
        var state = new CombineState(
            lookAt,
            idle,
            atk,
            backHome,
            idle,
            new TurnEndState(this.combatParticipant)
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
