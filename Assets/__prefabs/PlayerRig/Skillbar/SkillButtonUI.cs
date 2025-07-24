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
        var combatController = GameObject.FindGameObjectWithTag("CombatController").GetComponent<CombatController>();
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
                    state.Phase = CombatPhase.TurnSelectTarget;
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
