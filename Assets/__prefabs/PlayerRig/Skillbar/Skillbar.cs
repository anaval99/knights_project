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

    private CombatController combatController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var combatController = GameObject.FindGameObjectWithTag("CombatController").GetComponent<CombatController>();
        this.combatController = combatController;
        if (combatController == null)
        {
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
        // Subscribe to combat state changes
        combatController.CombatStateObs
            .Where(state => state != null)
            .Subscribe(state =>
            {
                if (state.Phase == CombatPhase.TurnSelectTarget || state.Phase == CombatPhase.TurnConfirmAction)
                {
                    this.skillbarUI.gameObject.SetActive(false);
                    this.potionsUI.gameObject.SetActive(false);
                    this.skillInfo.gameObject.SetActive(true);
                    this.skillInfo.SetText(state.SelectedSkillBook != null ? state.SelectedSkillBook.Description : string.Empty);
                }
                else
                {
                    this.skillbarUI.gameObject.SetActive(true);
                    this.potionsUI.gameObject.SetActive(true);
                    this.skillInfo.gameObject.SetActive(false);
                }
            })
            .AddTo(this);
    }
}
