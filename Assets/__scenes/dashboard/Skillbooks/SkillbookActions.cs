using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class SkillbookActions : MonoBehaviour
{
    [SerializeField]
    SkillBookList skillBookList;
    [SerializeField]
    CharDataCenter charDataCenter;
    [SerializeField]
    List<Button> equipSkillButtons;
    [SerializeField]
    Button equipHPPotionButton;
    [SerializeField]
    Button equipMPPotionButton;
    [SerializeField]
    GameObject SkillBar;
    [SerializeField]
    GameObject PotionBar;

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
        this.SelectedSkillBookObs.Subscribe(this.SetAvailableButtons).AddTo(this);
        this.equipHPPotionButton.OnClickAsObservable()
            .Where(_ => this.SelectedSkillBookObs.Value != null)
            .Select(_ => this.SelectedSkillBookObs.Value.SkillBookSOId)
            .Subscribe(this.SetHPPotion)
            .AddTo(this);
        this.equipMPPotionButton.OnClickAsObservable()
            .Where(_ => this.SelectedSkillBookObs.Value != null)
            .Select(_ => this.SelectedSkillBookObs.Value.SkillBookSOId)
            .Subscribe(this.SetMPPotion)
            .AddTo(this);            
    }

    void OnEnable()
    {
        this.SelectedSkillBookObs.OnNext(null);
        this.SkillBar.SetActive(true);
        this.PotionBar.SetActive(true);
    }

    void OnDisable()
    {
        this.SelectedSkillBookObs.OnNext(null);
        this.SkillBar.SetActive(false);
        this.PotionBar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetAvailableButtons(SkillBook selectedSkillBook)
    {
        bool isSkill = false;
        bool isHP = false;
        bool isMP = false;
        if (selectedSkillBook != null)
        {
            var skill = this.skillBookList.SkillBookDictionary[selectedSkillBook.SkillBookSOId];
            isSkill = skill.SkillType == SkillType.Active || skill.SkillType == SkillType.Passive;
            isHP = skill.SkillType == SkillType.HPPotion;
            isMP = skill.SkillType == SkillType.MPPotion;
        }
        this.equipSkillButtons.ForEach(btn => btn.gameObject.SetActive(isSkill));
        this.equipHPPotionButton.gameObject.SetActive(isHP);
        this.equipMPPotionButton.gameObject.SetActive(isMP);
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

    async void SetHPPotion(string potionId)
    {
        var skillbar = charDataCenter.CharSkillBarObs.Value;
        skillbar.LifePotionId = potionId;
        await this.charDataCenter.SaveSkillBar(skillbar);
    }

    async void SetMPPotion(string potionId)
    {
        var skillbar = charDataCenter.CharSkillBarObs.Value;
        skillbar.ManaPotionId = potionId;
        await this.charDataCenter.SaveSkillBar(skillbar);
    }
}
