using System;
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
    UnityEngine.UI.Image FocusedImage;
    [SerializeField]
    GameObject equippedSymbol;
    [SerializeField]
    UnityEngine.UI.Button button;
    [SerializeField]
    ImageList rarityFrameList;

    private Item item;

    void Start()
    {
        button.onClick.AddListener(this.OnButtonClicked);
        this.itemActionsUI.OnItemSelected.AddListener(this.SetFocused);
        this.charDataCenter.OnEquipmentChanged.AddListener(this.SetEquippedSymbol);
    }

    void OnDestroy()
    {
        button.onClick.RemoveListener(this.OnButtonClicked);
        this.itemActionsUI.OnItemSelected.RemoveListener(this.SetFocused);
        this.charDataCenter.OnEquipmentChanged.RemoveListener(this.SetEquippedSymbol);
    }

    void OnButtonClicked()
    {
        this.itemActionsUI.SetItem(this.item);
    }

    void SetFocused(Item item)
    {
        if (item == null || this.item == null)
        {
            this.FocusedImage.gameObject.SetActive(false);
            return;
        }
        if (this.itemActionsUI.currentItem == this.item)
        {
            this.FocusedImage.gameObject.SetActive(true);
        }
        else
        {
            this.FocusedImage.gameObject.SetActive(false);
        }
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
        this.item = item;
        this.SetFocused(this.itemActionsUI.currentItem);
        this.SetEquippedSymbol(this.charDataCenter.CurrentEquipment);
        if (item == null || string.IsNullOrEmpty(item.ItemSOId))
        {
            this.itemImage.sprite = null;
            this.itemImage.gameObject.SetActive(false);
            this.RarityImage.sprite = this.rarityFrameList.GetRarityFrame(Rarity.None);
            return;
        }
        var itemSO = this.equipmentList.EquipmentItemsDictionary[item.ItemSOId];
        this.itemImage.sprite = itemSO.ItemIcon;
        this.RarityImage.sprite = this.rarityFrameList.GetRarityFrame(itemSO.Rarity);
        this.itemImage.gameObject.SetActive(true);
    }
}
