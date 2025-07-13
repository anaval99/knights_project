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
        var skillBarObs = this.charDataCenter.CharSkillBarObs.Where(x => x != null);
        equipIndexObs.CombineLatest(skillBarObs, (equipIndex, skillBar) =>
        {
            return (equipIndex, skillBar);
        })
        .Where(_ => this.SelectedSkillBookObs.Value != null)
        .Subscribe(index_skillbar =>
        {
            var (equipIndex, skillBar) = index_skillbar;
            Debug.Log($"Equip index: {equipIndex}, SkillBar: {this.SelectedSkillBookObs.Value.SkillBookSOId}");
        })
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
}
