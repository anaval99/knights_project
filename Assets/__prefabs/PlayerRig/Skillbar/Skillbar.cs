using R3;
using UnityEngine;

public class Skillbar : MonoBehaviour
{
    [SerializeField]
    SkillbarUI skillbarUI;
    [SerializeField]
    PotionsUI potionsUI;
    [SerializeField]
    SkillInfo skillInfo;

    public BehaviorSubject<SkillBookSO> SelectedSkillBookSO = new(null);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.SelectedSkillBookSO
            .Subscribe(skillBookSO =>
            {
                bool isCasting = skillBookSO != null;
                this.skillbarUI.gameObject.SetActive(!isCasting);
                this.potionsUI.gameObject.SetActive(!isCasting);
                this.skillInfo.gameObject.SetActive(isCasting);
                this.skillInfo.SetText(skillBookSO != null ? skillBookSO.Description : string.Empty);
            })
            .AddTo(this);
    }

    void OnEnable()
    {
        this.SelectedSkillBookSO.OnNext(null);
    }
}
