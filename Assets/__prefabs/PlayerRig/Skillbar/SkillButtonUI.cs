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
    [SerializeField]
    EquipmentList EquipmentList;
    [SerializeField]
    CharDataCenter CharDataCenter;
    [SerializeField]
    GameObject DisabledSymbol;
    [SerializeField]
    TMPro.TextMeshProUGUI UsageHint;

    private (ItemSO, GameObject) weaponSOGO = (null, null);

    private SkillBookSO skillBookSO;
    private CombatController combatController;
    private BehaviorSubject<(string, int)> renderObs = new BehaviorSubject<(string, int)>(("__starter", 0));
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.DisabledSymbol.SetActive(false);

        Observable.CombineLatest(
            this.CharDataCenter.CharEquipmentObs,
            this.renderObs.Where(x => x.Item1 != "__starter"),
            (equipment, renderInfo) => (equipment, renderInfo.Item1, renderInfo.Item2)
        ).Subscribe((data) =>
        {
            var (eq, soId, qty) = data;
            var sogo = this.EquipmentList.GetSOGO(eq.Weapon);
            this.weaponSOGO = sogo;
            this.ManuallyRender(soId, qty);
        })
        .AddTo(this);

        this.combatController = this.GetCombatController();
        if (this.combatController == null)
        {
            Debug.Log("Not in combat, disabling skill button.");
            return;
        }

        this.skillButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (this.skillBookSO != null && (
                    this.skillBookSO.SkillType == SkillType.Active ||
                    this.skillBookSO.SkillType == SkillType.HPPotion ||
                    this.skillBookSO.SkillType == SkillType.MPPotion))
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

    public void Render(string skillBookSOId, int quantity = -1)
    {
        this.renderObs.OnNext((skillBookSOId, quantity));
    }

    void ManuallyRender(string skillBookSOId, int quantity = -1)
    {
        this.skillBookSO = null;
        this.UsageHint.SetText("");
        if (string.IsNullOrEmpty(skillBookSOId) || !this.skillBookList.SkillBookDictionary.ContainsKey(skillBookSOId))
        {
            this.skillIcon.gameObject.SetActive(false);
            this.DisabledSymbol.SetActive(true);
            this.skillButton.interactable = false;
            return;
        }
        var skillBookSO = skillBookList.SkillBookDictionary[skillBookSOId];
        this.skillBookSO = skillBookSO;
        this.skillIcon.sprite = skillBookSO.SkillIcon;
        this.skillIcon.gameObject.SetActive(true);
        if (quantity > -1)
        {
            this.UsageHint.SetText(quantity.ToString());
        }
        else if (this.skillBookSO.SkillType == SkillType.Active)
        {
            this.UsageHint.SetText(this.skillBookSO.ManaCost.ToString());
        }
        this.SetButtonDisabledState(quantity);     
    }

    public void SetButtonDisabledState(int quantity = -1)
    {
        var weaponSO = this.weaponSOGO.Item1;
        bool skillEnabled = weaponSO != null && this.skillBookSO != null && (
            weaponSO.WeaponClass == this.skillBookSO.WeaponClass ||
            this.skillBookSO.WeaponClass == WeaponClass.None
        );
        bool isPotion = this.skillBookSO != null &&
            (this.skillBookSO.SkillType == SkillType.HPPotion || this.skillBookSO.SkillType == SkillType.MPPotion);
        bool potionBtnEnabled = isPotion && quantity > 0;
        if (isPotion)
        {
            this.DisabledSymbol.SetActive(!potionBtnEnabled);
            this.skillButton.interactable = potionBtnEnabled;
        }
        else
        {    
            this.DisabledSymbol.SetActive(!skillEnabled);
            this.skillButton.interactable = skillEnabled;
        }
    }
}
