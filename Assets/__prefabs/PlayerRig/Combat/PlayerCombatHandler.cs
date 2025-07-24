using R3;
using UnityEngine;

public class PlayerCombatHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerAnimation playerAnimation;
    [SerializeField]
    private CharDataCenter charDataCenter;
    [SerializeField]
    private CombatParticipant combatParticipant;
    [SerializeField]
    private EquipmentList equipmentList;

    public void InitializeCombatHandler(Observable<CombatState> combatStateObs)
    {
        combatStateObs.Subscribe(state =>
        {
            switch (state.Phase)
            {
                case CombatPhase.Start:
                    break;
                case CombatPhase.Patrolling:
                    // Handle patrolling logic
                    this.playerAnimation.PlayPatrollingAnimation();
                    break;
                case CombatPhase.BattleStart:
                    // Handle battle start logic
                    this.playerAnimation.PlayIdleAnimation();
                    break;
                case CombatPhase.None:
                default:
                    // Handle other phases or no combat
                    Debug.Log("No combat phase active.");
                    break;
            }
        })
        .AddTo(this);
    }

    void Start()
    {
        this.charDataCenter.CharEquipmentObs.Subscribe(equipment =>
        {
            this.ComputeStatsFromEquipment(equipment);
        }).AddTo(this);
    }

    private void ComputeStatsFromEquipment(CharEquipment equipment)
    {
        // Example logic to get stats from equipment
        if (equipment != null)
        {
            var armorSOGO = this.equipmentList.GetSOGO(equipment.Armor);
            var weaponSOGO = this.equipmentList.GetSOGO(equipment.Weapon);
            // armor
            this.combatParticipant.MaxHealth = armorSOGO.Item1.Health;
            this.combatParticipant.CurrentHealth = armorSOGO.Item1.Health;
            this.combatParticipant.Defense = armorSOGO.Item1.Defense;
            // weapon
            this.combatParticipant.Damage = weaponSOGO.Item1.Damage;
        }
    }
}
