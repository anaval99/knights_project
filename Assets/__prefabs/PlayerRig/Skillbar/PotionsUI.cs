using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using R3;
using UnityEngine;

public class PotionsUI : MonoBehaviour
{
    [SerializeField]
    CharDataCenter charDataCenter;
    [SerializeField]
    SkillButtonUI HPPotionButtonUI;
    [SerializeField]
    SkillButtonUI MPPotionButtonUI;
    [SerializeField]
    SkillButtonUI SkipButtonUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var dataCenter = this.charDataCenter;
        var controller = this.GetCombatController();
        if (controller != null)
        {
            // in mission, you can only use your own potions
            dataCenter = controller.playerRig.CharDataCenter;
        }
        Observable.CombineLatest(
            dataCenter.CharSkillBarObs.Where(data => data != null),
            dataCenter.CharSkillBooksObs.Where(data => data != null),
            (skillbar, skillbooks) => (skillbar, skillbooks)
        ).Subscribe(data =>
        {
            var (skillbar, skillbooks) = data;
            this.RenderPotion(this.HPPotionButtonUI, skillbar.LifePotionId, skillbooks.SkillBooks);
            this.RenderPotion(this.MPPotionButtonUI, skillbar.ManaPotionId, skillbooks.SkillBooks);
        }).AddTo(this);
        this.SkipButtonUI.Render("skip_turn");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RenderPotion(SkillButtonUI potionUI, string potionSOId, List<SkillBook> books)
    {
        if (potionSOId == null)
        {
            potionUI.Render(null);
        }
        int count = books.Where(b => b.SkillBookSOId == potionSOId).Sum(b => b.Quantity);
        potionUI.Render(potionSOId, count);
    }
}
