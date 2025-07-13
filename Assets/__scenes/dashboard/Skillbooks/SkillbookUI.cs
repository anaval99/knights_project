using R3;
using UnityEngine;
using UnityEngine.UI;

public class SkillbookUI : MonoBehaviour
{
    [SerializeField]
    SkillBookList skillBookList;
    [SerializeField]
    UnityEngine.UI.Image itemImage;
    [SerializeField]
    UnityEngine.UI.Image backgroundImage;
    [SerializeField]
    GameObject FocusedSymbol;
    [SerializeField]
    SkillbookActions skillbookActions;
    [SerializeField]
    Button skillUIButton;

    private SkillBook currentSkillBook;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.skillUIButton.OnClickAsObservable()
            .Where(_ => this.currentSkillBook != null)
            .Subscribe(_ =>
            {
                this.skillbookActions.SelectedSkillBookObs.OnNext(this.currentSkillBook);
            })
            .AddTo(this);

        this.skillbookActions.SelectedSkillBookObs
            .Subscribe(selectedSkillBook =>
            {
                bool focused = false;
                if (selectedSkillBook != null && this.currentSkillBook != null)
                {
                    focused = selectedSkillBook.SkillBookSOId == this.currentSkillBook.SkillBookSOId;
                }
                this.FocusedSymbol.SetActive(focused);
            })
            .AddTo(this);
    }

    public void Render(SkillBook skillBook)
    {
        this.currentSkillBook = skillBook;
        this.FocusedSymbol.SetActive(false);
        this.itemImage.gameObject.SetActive(false);
        this.backgroundImage.color = Color.black;
        if (skillBook == null)
        {
            return;
        }

        var skillBookSO = skillBookList.SkillBookDictionary[skillBook.SkillBookSOId];
        if (skillBook != null)
        {
            itemImage.sprite = skillBookSO.SkillIcon;
            itemImage.gameObject.SetActive(true);
            this.backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            if (this.skillbookActions.SelectedSkillBookObs.Value != null)
            {
                this.FocusedSymbol.SetActive(this.skillbookActions.SelectedSkillBookObs.Value.SkillBookSOId == skillBook.SkillBookSOId);
            }
        }
    }
}
