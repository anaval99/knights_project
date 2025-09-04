using Unity.VisualScripting;
using UnityEngine;

public class RewardItemUI : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Image Glow;
    [SerializeField]
    UnityEngine.UI.Image ItemIcon;
    [SerializeField]
    TMPro.TextMeshProUGUI Qty;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Render(LootedItem lootedItem)
    {
        Sprite icon = null;
        if (lootedItem.SkillBookSO != null)
        {
            icon = lootedItem.SkillBookSO.SkillIcon;
        }
        else if (lootedItem.ItemSO != null)
        {
            icon = lootedItem.ItemSO.ItemIcon;
        }
        this.Glow.gameObject.SetActive(false);
        this.ItemIcon.sprite = icon;
        this.Qty.SetText(lootedItem.Qty.ToString());
    }
}
