using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class SkillbookActions : MonoBehaviour
{
    [SerializeField]
    CharDataCenter charDataCenter;
    [SerializeField]
    List<Button> equipSkillButtons;

    public BehaviorSubject<SkillBook> SelectedSkillBookObs = new(null);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var equipIndexObs = Observable.Merge(this.equipSkillButtons.Select(button => button.OnClickAsObservable().Select(_ => this.equipSkillButtons.IndexOf(button))));
        equipIndexObs
        .Where(_ => this.SelectedSkillBookObs.Value != null)
        .Select(equipIndex => (equipIndex, this.charDataCenter.CharSkillBarObs.Value))
        .Subscribe(this.SetSkillBar)
        .AddTo(this);
    }

    void OnEnable()
    {
        this.SelectedSkillBookObs.OnNext(null);
    }

    // Update is called once per frame
    void Update()
    {

    }

    async void SetSkillBar((int equipIndex, CharSkillBar skillBar) index_skillbar)
    {
        var (equipIndex, skillBar) = index_skillbar;
        if (this.SelectedSkillBookObs.Value != null)
        {
            // check first if equipped to other index, if so, remove it and set to null
            var existingIndex = skillBar.SkillBar.ToList().FindIndex(skillSOId => skillSOId == this.SelectedSkillBookObs.Value.SkillBookSOId);
            if (existingIndex != -1 && existingIndex != equipIndex)
            {
                skillBar.SkillBar[existingIndex] = null;
            }

            skillBar.SkillBar[equipIndex] = this.SelectedSkillBookObs.Value.SkillBookSOId;
            await this.charDataCenter.SaveSkillBar(skillBar);
            Debug.Log($"Set SkillBar at index {equipIndex} to {this.SelectedSkillBookObs.Value.SkillBookSOId}");
        }
    }
}
