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
}