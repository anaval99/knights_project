using R3;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class ItemActionsUI : MonoBehaviour
{
    [SerializeField]
    CharDataCenter charDataCenter;
    [SerializeField]
    EquipmentList equipmentList;
    [SerializeField]
    TMPro.TextMeshProUGUI itemNameText;
    [SerializeField]
    TMPro.TextMeshProUGUI itemDescriptionText;
    [SerializeField]
    UnityEngine.UI.Button equipButton;

    public BehaviorSubject<Item> CurrentItemObs = new(null); 

    void Start()
    {
        equipButton.onClick.AddListener(OnEquipButtonClicked);
    }

    void OnDestroy()
    {
        equipButton.onClick.RemoveListener(OnEquipButtonClicked);
    }

    void OnEnable()
    {
        this.SetItem(null);
    }

    async void OnEquipButtonClicked()
    {
        var (itemSO, part) = equipmentList.GetSOGO(CurrentItemObs.Value);
        if (itemSO.WeaponClass != WeaponClass.None)
        {
            // Equip as weapon
            charDataCenter.CurrentEquipment.Weapon = CurrentItemObs.Value;
            await charDataCenter.SaveEquipment(charDataCenter.CurrentEquipment);
        }
        else if (itemSO.ArmorPart == ArmorPart.Body)
        {
            // Equip as armor
            charDataCenter.CurrentEquipment.Armor = CurrentItemObs.Value;
            await charDataCenter.SaveEquipment(charDataCenter.CurrentEquipment);
        }
    }

    public void SetItem(Item item)
    {
        this.CurrentItemObs.OnNext(item);
        if (item == null)
        {
            this.itemNameText.text = string.Empty;
            this.itemDescriptionText.text = string.Empty;
            return;
        }
        var (itemSO, part) = equipmentList.GetSOGO(item);
        this.itemNameText.text = itemSO.ItemName;
        this.itemDescriptionText.text = itemSO.ItemDescription;
    }
}
