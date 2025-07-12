using UnityEngine;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Render(SkillBook skillBook)
    {
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
        }
    }
}
