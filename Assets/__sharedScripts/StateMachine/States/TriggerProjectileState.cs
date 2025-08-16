using R3;

public class TriggerProjectileState : IRxState
{
    public string Name { get; set; } = "TriggerProjectileState";
    public CombatParticipant Source;
    public CombatParticipant Target;
    public ProjectileType ProjectileType;
    public int Damage;
    public float TimingSeconds;

    public Observable<int> Play()
    {
        var controller = this.Source.GetCombatController();
        return Observable.Defer(() =>
        {
            return this.Source.ProjectileMaker
                .CreateProjectile(this.ProjectileType, this.Target)
                .Do(onCompleted: _ =>
                {
                    var hit = new OnHitEvent()
                    {
                        Damage = this.Damage,
                        Source = this.Source,
                        Target = this.Target
                    };
                    controller.TriggerOnHit(hit);
                });
        });
    }
}