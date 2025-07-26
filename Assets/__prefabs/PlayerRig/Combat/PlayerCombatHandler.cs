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
    [SerializeField]
    private UnderGlow underGlow;
    [SerializeField]
    private Skillbar skillbar;

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
        this.charDataCenter.CharEquipmentObs.Subscribe(equipment => this.ComputeStatsFromEquipment(equipment)).AddTo(this);
        this.combatParticipant.IsMyTurnObs.DistinctUntilChanged().Subscribe(isMyTurn =>
        {
            this.underGlow.gameObject.SetActive(isMyTurn);
            this.skillbar.gameObject.SetActive(isMyTurn);
        }).AddTo(this);
        this.combatParticipant.IsMyTurnStartObs.DistinctUntilChanged().Subscribe(isMyTurnStart =>
        {
            this.skillbar.skillbarUI.gameObject.SetActive(isMyTurnStart);
            this.skillbar.potionsUI.gameObject.SetActive(isMyTurnStart);
        }).AddTo(this);
        this.combatParticipant.IsMyTurnConfirmActionObs.DistinctUntilChanged().Subscribe(isMyTurnConfirm =>
        {
            this.skillbar.turnConfirmButton.gameObject.SetActive(isMyTurnConfirm);
        }).AddTo(this);        
        Observable.CombineLatest(this.combatParticipant.IsMyTurnSelectTargetObs, this.combatParticipant.IsMyTurnConfirmActionObs).Subscribe(tuple =>
        {
            var isMyTurnSelectTarget = tuple[0];
            var isMyTurnConfirmAction = tuple[1];
            this.skillbar.skillInfo.gameObject.SetActive(isMyTurnSelectTarget || isMyTurnConfirmAction);
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
