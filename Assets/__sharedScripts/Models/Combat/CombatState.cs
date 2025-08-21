using System.Collections.Generic;

public class CombatState
{
    public CombatPhase Phase { get; set; } = CombatPhase.None;
    public List<CombatParticipant> PlayerParticipants { get; set; } = new List<CombatParticipant>();
    public List<CombatParticipant> EnemyParticipants { get; set; } = new List<CombatParticipant>();

    public int TurnIndex { get; set; } = 0;
    public List<CombatParticipant> ShuffledParticipants { get; set; } = new List<CombatParticipant>();
    public SkillBookSO SelectedSkillBook { get; set; } = null;
    public List<CombatParticipant> SkillTargets { get; set; } = new List<CombatParticipant>();
    public bool isFinalZone { get; set; } = false;

    public CombatState Clone()
    {
        var clone = new CombatState
        {
            Phase = this.Phase,
            TurnIndex = this.TurnIndex,
            isFinalZone = this.isFinalZone,
            SelectedSkillBook = this.SelectedSkillBook,
        };

        clone.PlayerParticipants = new List<CombatParticipant>(this.PlayerParticipants);
        clone.EnemyParticipants = new List<CombatParticipant>(this.EnemyParticipants);
        clone.ShuffledParticipants = new List<CombatParticipant>(this.ShuffledParticipants);
        clone.SkillTargets = new List<CombatParticipant>(this.SkillTargets);

        return clone;
    }
}