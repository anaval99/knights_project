using System.Collections.Generic;
using R3;
using UnityEngine;

public class SkillbooksContainer : ListContainer
{
    [SerializeField]
    private List<SkillbookUI> skillbookUIs = new();
    [SerializeField]
    private CharDataCenter charDataCenter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.PopulateListItems(skillbookUIs, 21);
        this.charDataCenter.CharSkillBooksObs.Subscribe(this.Render).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Render(CharSkillBooks charSkillBooks)
    {
        if (charSkillBooks == null || charSkillBooks.SkillBooks == null)
        {
            return;
        }
        this.RenderItems(this.skillbookUIs, charSkillBooks.SkillBooks, 0, (skillbookUI, data) => skillbookUI.Render(data));
    }
}
