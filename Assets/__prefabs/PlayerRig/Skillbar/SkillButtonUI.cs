using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

public class SkillButtonUI : MonoBehaviour
{
    [SerializeField]
    SkillBookList skillBookList;
    [SerializeField]
    UnityEngine.UI.Image skillIcon;
    [SerializeField]
    UnityEngine.UI.Button skillButton;

    private SkillBookSO skillBookSO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var combatController = this.GetCombatController();
        if (combatController == null)
        {
            Debug.Log("Not in combat, disabling skill button.");
            return;
        }
        this.skillButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (this.skillBookSO != null && this.skillBookSO.SkillType == SkillType.Active)
                {
                    var state = combatController.CombatStateObs.Value;
                    state.SelectedSkillBook = this.skillBookSO;
                    if (this.skillBookSO.TargetType == TargetType.AllySingle || this.skillBookSO.TargetType == TargetType.EnemySingle)
                    {
                        state.Phase = CombatPhase.TurnSelectTarget;
                    }
                    else
                    {
                        var currentTurnParticipant = state.ShuffledParticipants[state.TurnIndex];
                        if (state.SelectedSkillBook.TargetType == TargetType.AllyAll)
                        {
                            state.SkillTargets = state.PlayerParticipants.Where(p => p != currentTurnParticipant).ToList();
                        }
                        else if (state.SelectedSkillBook.TargetType == TargetType.EnemyAll)
                        {
                            state.SkillTargets = state.EnemyParticipants.ToList();
                        }
                        else if (state.SelectedSkillBook.TargetType == TargetType.Self)
                        {
                            state.SkillTargets = new List<CombatParticipant> { currentTurnParticipant };
                        }
                        else if (state.SelectedSkillBook.TargetType == TargetType.AllyTeam)
                        {
                            state.SkillTargets = state.PlayerParticipants.ToList();
                        }

                        state.Phase = CombatPhase.TurnConfirmAction;
                    }
                    combatController.SetCombatState(state);
                }
            })
            .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Render(string skillBookSOId)
    {
        this.skillBookSO = null;
        if (string.IsNullOrEmpty(skillBookSOId) || !this.skillBookList.SkillBookDictionary.ContainsKey(skillBookSOId))
        {
            this.skillIcon.gameObject.SetActive(false);
            return;
        }
        var skillBookSO = skillBookList.SkillBookDictionary[skillBookSOId];
        this.skillBookSO = skillBookSO;
        this.skillIcon.sprite = skillBookSO.SkillIcon;
        this.skillIcon.gameObject.SetActive(true);
    }
}
