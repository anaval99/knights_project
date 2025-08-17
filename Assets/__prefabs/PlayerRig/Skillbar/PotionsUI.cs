using System.Collections.Generic;
using System.Linq;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Observable.CombineLatest(
            this.charDataCenter.CharSkillBarObs.Where(data => data != null),
            this.charDataCenter.CharSkillBooksObs.Where(data => data != null),
            (skillbar, skillbooks) => (skillbar, skillbooks)
        ).Subscribe(data =>
        {
            var (skillbar, skillbooks) = data;
            this.RenderPotion(this.HPPotionButtonUI, skillbar.LifePotionId, skillbooks.SkillBooks);
            this.RenderPotion(this.MPPotionButtonUI, skillbar.ManaPotionId, skillbooks.SkillBooks);
        }).AddTo(this);
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
        int count = books.Where(book => book.SkillBookSOId == potionSOId).Count();
        potionUI.Render(potionSOId, count);
    }
}
