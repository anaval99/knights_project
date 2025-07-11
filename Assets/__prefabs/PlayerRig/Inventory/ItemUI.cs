using System;
using R3;
using Unity.VisualScripting;
using UnityEngine;

public class ItemUI : MonoBehaviour
{
    [SerializeField]
    CharDataCenter charDataCenter;
    [SerializeField]
    ItemActionsUI itemActionsUI;
    [SerializeField]
    EquipmentList equipmentList;
    [SerializeField]
    UnityEngine.UI.Image itemImage;
    [SerializeField]
    UnityEngine.UI.Image RarityImage;
    [SerializeField]
    GameObject FocusedSymbol;
    [SerializeField]
    GameObject equippedSymbol;
    [SerializeField]
    UnityEngine.UI.Button button;

    private Item item;

    void Start()
    {
        button.onClick.AddListener(this.OnButtonClicked);
        this.itemActionsUI.CurrentItemObs.Subscribe(this.SetFocused).AddTo(this);
        this.charDataCenter.CharEquipmentObs.Subscribe(this.SetEquippedSymbol).AddTo(this);
    }

    void OnDestroy()
    {
        button.onClick.RemoveListener(this.OnButtonClicked);
    }

    void OnButtonClicked()
    {
        this.itemActionsUI.SetItem(this.item);
    }

    void SetFocused(Item item)
    {
        this.FocusedSymbol.SetActive(item != null && item == this.item);
    }

    void SetEquippedSymbol(CharEquipment equipment)
    {
        this.equippedSymbol.SetActive(false);
        if (equipment == null || this.item == null || equipment.Armor == null || equipment.Weapon == null)
        {
            return;
        }

        if (equipment.Weapon.InstanceId == this.item.InstanceId ||
            equipment.Armor.InstanceId == this.item.InstanceId)
        {
            this.equippedSymbol.SetActive(true);
        }
    }

    public void Render(Item item)
    {
        this.RarityImage.color = this.GetRarityColor(Rarity.None);
        this.item = item;
        this.SetEquippedSymbol(this.charDataCenter.CharEquipmentObs.Value);
        if (item == null || string.IsNullOrEmpty(item.ItemSOId))
        {
            this.itemImage.sprite = null;
            this.itemImage.gameObject.SetActive(false);
            return;
        }
        var itemSO = this.equipmentList.EquipmentItemsDictionary[item.ItemSOId];
        this.itemImage.sprite = itemSO.ItemIcon;
        this.RarityImage.color = this.GetRarityColor(itemSO.Rarity);
        this.itemImage.gameObject.SetActive(true);
    }

    public Color GetRarityColor(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => new Color(94f / 255f, 47f / 255f, 0f, 1f),// Dark Brown
            Rarity.Rare => new Color(20f / 255f, 46f / 255f, 34f / 255f, 1f),// Dark Green
            Rarity.Epic => new Color(30f / 255f, 20f / 255f, 150f / 255f, 1f),// Dark Blue
            Rarity.Legendary => new Color(180f / 255f, 30f / 255f, 30f / 255f, 1f),// Dark Red
            _ => new Color(0f, 0f, 0f, 1f),// Black
        };
    }
}
