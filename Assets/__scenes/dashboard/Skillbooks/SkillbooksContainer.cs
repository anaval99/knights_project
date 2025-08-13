using System.Collections.Generic;
using R3;
using UnityEngine;

public class SkillbooksContainer : ListContainer
{
    [SerializeField]
    ListPager listPager;
    [SerializeField]
    private List<SkillbookUI> skillbookUIs = new();
    [SerializeField]
    private CharDataCenter charDataCenter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.PopulateListItems(skillbookUIs, 21);
        var source = this.charDataCenter.CharSkillBooksObs.Select(x => x.SkillBooks);
        this.listPager.Feed(source, 21).Subscribe(this.Render).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Render(PagedList<SkillBook> pageDSkillbookList)
    {
        this.RenderItems(this.skillbookUIs, pageDSkillbookList.Data, 0, (skillbookUI, data) => skillbookUI.Render(data));
    }
}
