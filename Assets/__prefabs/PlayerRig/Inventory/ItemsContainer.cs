using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

public class ItemsContainer : ListContainer
{
    [SerializeField]
    ListPager listPager;
    [SerializeField]
    CharDataCenter charDataCenter;
    [SerializeField]
    List<ItemUI> itemUIs;

    void Start()
    {
        this.PopulateListItems(itemUIs, 28);
        this.listPager.Feed(this.charDataCenter.CharInventoryObs.Select(x => x.Items), 28)
            .Select(x => x.Data)
            .Subscribe(this.Render).AddTo(this);
    }

    void Render(List<Item> items)
    {    
        this.RenderItems(this.itemUIs, items, 0, (itemUI, data) => itemUI.Render(data));
    }

    void OnDestroy()
    {
    }
}
