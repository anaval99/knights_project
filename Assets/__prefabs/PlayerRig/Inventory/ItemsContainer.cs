using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

public class ItemsContainer : ListContainer
{
    [SerializeField]
    CharDataCenter charDataCenter;
    [SerializeField]
    List<ItemUI> itemUIs;

    void Start()
    {
        this.PopulateListItems(itemUIs, 28);
        this.charDataCenter.CharInventoryObs.Subscribe(this.Render).AddTo(this);
    }

    void Render(CharInventory inventory)
    {    
        this.RenderItems(this.itemUIs, inventory.Items, 0, (itemUI, data) => itemUI.Render(data));
    }

    void OnDestroy()
    {
    }
}
