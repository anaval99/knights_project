using UnityEngine;

public class SkillbookUI : MonoBehaviour
{
    [SerializeField]
    private SkillBookList skillBookList;
    [SerializeField]
    UnityEngine.UI.Image itemImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Render(SkillBook skillBook)
    {
        if (skillBook == null)
        {
            itemImage.gameObject.SetActive(false);
            return;
        }

        var skillBookSO = skillBookList.SkillBookDictionary[skillBook.SkillBookSOId];
        if (skillBook != null)
        {
            itemImage.sprite = skillBookSO.SkillIcon;
            itemImage.gameObject.SetActive(true);
        }
        else
        {
            itemImage.gameObject.SetActive(false);
        }
    }
}
