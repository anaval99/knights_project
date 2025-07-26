using R3;
using UnityEngine;
using UnityEngine.UI;

public class Skillbar : MonoBehaviour
{
    [SerializeField]
    public SkillbarUI skillbarUI;
    [SerializeField]
    public PotionsUI potionsUI;
    [SerializeField]
    public SkillInfo skillInfo;
    [SerializeField]
    private Button skillCancelButton;
    [SerializeField]
    public Button turnConfirmButton;

    private CombatController combatController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var combatController = this.GetCombatController();
        this.combatController = combatController;
        if (combatController == null)
        {
            this.skillbarUI.gameObject.SetActive(true);
            Debug.Log("Not in combat, disabling skillbar.");
            return;
        }
        this.skillCancelButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                var state = combatController.CombatStateObs.Value;
                state.Phase = CombatPhase.TurnStart;
                state.SkillTargets.Clear();
                state.SelectedSkillBook = null;
                combatController.SetCombatState(state);
            })
            .AddTo(this);
        this.turnConfirmButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                var state = combatController.CombatStateObs.Value;
                state.Phase = CombatPhase.TurnAction;
                combatController.SetCombatState(state);
            }).AddTo(this);
    }
}
