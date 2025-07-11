using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EquipmentList : MonoBehaviour
{
    [SerializeField]
    public GameObject[] EquipmentParts;
    [SerializeField]
    public ItemSO[] EquipmentItems;

    private Dictionary<string, GameObject> equipmentPartsDictionary;
    private Dictionary<string, ItemSO> equipmentItemsDictionary;

    public Dictionary<string, GameObject> EquipmentPartsDictionary
    {
        get
        {
            if (equipmentPartsDictionary == null)
            {
                equipmentPartsDictionary = new Dictionary<string, GameObject>();
                foreach (var part in EquipmentParts)
                {
                    if (part != null)
                    {
                        equipmentPartsDictionary[part.name] = part;
                    }
                }
            }
            return equipmentPartsDictionary;
        }
    }

    public Dictionary<string, ItemSO> EquipmentItemsDictionary
    {
        get
        {
            if (equipmentItemsDictionary == null)
            {
                equipmentItemsDictionary = new Dictionary<string, ItemSO>();
                foreach (var item in EquipmentItems)
                {
                    if (item != null)
                    {
                        equipmentItemsDictionary[item.name] = item;
                    }
                }
            }
            return equipmentItemsDictionary;
        }
    }

    public (ItemSO, GameObject) GetSOGO(Item item)
    {
        if (item == null || string.IsNullOrEmpty(item.ItemSOId))
        {
            return (null, null);
        }

        if (EquipmentItemsDictionary.TryGetValue(item.ItemSOId, out ItemSO itemSO) &&
            EquipmentPartsDictionary.TryGetValue(itemSO.GameObjectName, out GameObject part))
        {
            return (itemSO, part);
        }

        return (null, null);
    }
    
    private void OnValidate()
    {
        // ensure no duplicates when adding in editor
        if (this.EquipmentParts != null)
        {
            this.EquipmentParts = this.EquipmentParts.DistinctBy(part => part.name).ToArray();
        }

        if (this.EquipmentItems != null)
        {
            this.EquipmentItems = this.EquipmentItems.DistinctBy(item => item.name).ToArray();
        }
    }
}
