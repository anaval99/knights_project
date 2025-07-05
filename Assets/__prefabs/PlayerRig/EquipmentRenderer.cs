using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentRenderer : MonoBehaviour
{
    [SerializeField]
    public EquipmentList equipmentList;

    private List<GameObject> currentlyActiveEquipmentParts = null;

    public void SetEquipment(CharEquipment charEquipment)
    {
        // initialize currentlyActiveEquipmentParts if null
        if (currentlyActiveEquipmentParts == null)
        {
            this.currentlyActiveEquipmentParts = this.equipmentList.EquipmentParts.ToList();
        }
        // Ensure all equipment parts are inactive before setting new ones
        foreach (var part in currentlyActiveEquipmentParts)
        {
            if (part != null)
            {
                part.SetActive(false);
            }
        }
        currentlyActiveEquipmentParts.Clear();
        // Set the weapon part
        this.SetItemPart(charEquipment.Weapon);
        // Set the armor part
        this.SetItemPart(charEquipment.Armor);
    }

    public void SetItemPart(Item item)
    {
        var (SO, GO) = this.equipmentList.GetSOGO(item);
        if (GO == null)
        {
            Debug.LogError($"EquipmentRenderer.SetItemPart: Item GameObject '{SO.GameObjectName}' not found in EquipmentPartsDictionary.");
            return;
        }
        GO.SetActive(true);
        currentlyActiveEquipmentParts.Add(GO);
    }
}
