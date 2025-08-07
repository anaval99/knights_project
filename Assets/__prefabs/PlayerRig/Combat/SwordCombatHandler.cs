public partial class PlayerCombatHandler
{
    public IRxState BeginnerSlash(SkillBookSO skillBookSO)
    {
        var dash = this.CreateDashState();
        var idle = this.CreateIdleState();
        var atk = this.Attack01(skillBookSO);
        var back = this.CreateBackState();
        var state = new CombineState(
            dash,
            idle,
            atk,
            back,
            idle,
            new TurnEndState(this.combatParticipant)
        );
        return state;
    }

    public IRxState Attack01(SkillBookSO skillBookSO)
    {
        var attack01 = new MeleeAttack01(
            this.combatParticipant,
            this.playerAnimation.animancerComponent,
            this.playerAnimation.animationList,
            skillBookSO
        );
        return attack01;
    }
}
