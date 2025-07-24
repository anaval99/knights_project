using System.Collections.Generic;
using R3;
using UnityEngine;

public class SkillbarUI : ListContainer
{
    [SerializeField]
    List<SkillButtonUI> skillButtons;
    [SerializeField]
    CharDataCenter charDataCenter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.PopulateListItems(
            this.skillButtons,
            4
        );
        this.charDataCenter.CharSkillBarObs
            .Where(skillBar => skillBar != null)
            .Subscribe(skillBar =>
            {
                for (int i = 0; i < this.skillButtons.Count; i++)
                {
                    var skillBookSOId = skillBar.SkillBar[i];
                    this.skillButtons[i].Render(skillBookSOId);
                }
            })
            .AddTo(this);
    }
}
