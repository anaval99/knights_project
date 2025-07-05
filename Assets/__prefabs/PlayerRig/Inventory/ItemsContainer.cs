using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemsContainer : MonoBehaviour
{
    [SerializeField]
    CharDataCenter charDataCenter;
    [SerializeField]
    List<ItemUI> itemUIs;

    void Start()
    {
        this.PopuplateItemsUI();
        this.charDataCenter.OnInventoryChange.AddListener(this.Render);
        if (this.charDataCenter?.CurrentInventory != null)
        {
            this.Render(this.charDataCenter.CurrentInventory);
        }
    }

    void PopuplateItemsUI()
    {
        if (this.itemUIs == null || this.itemUIs.Count == 0)
        {
            Debug.LogWarning("No ItemUI components assigned to ItemsContainer.");
            return;
        }

        while (this.itemUIs.Count < 18)
        {
            // add more items and then set parent to this.transform
            var itemUI = Instantiate(this.itemUIs[0], this.transform);
            this.itemUIs.Add(itemUI);
        }
    }

    void Render(CharInventory inventory)
    {
        var pageNumber = 0;
        var pageSize = this.itemUIs.Count;
        var renderedItems = inventory.Items
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToList();
        for (int i = 0; i < this.itemUIs.Count; i++)
        {
            // check if renderedItems has enough items
            if (i < renderedItems.Count)
            {
                this.itemUIs[i].Render(renderedItems[i]);
            }
            else
            {
                this.itemUIs[i].Render(null); // Clear the UI if no item is available
            }
        }

        var totalPages = Mathf.CeilToInt((float)inventory.Items.Count / pageSize);
        Debug.Log($"Total pages: {totalPages}, Current page: {pageNumber}");
    }

    void OnDestroy()
    {
        this.charDataCenter.OnInventoryChange.RemoveListener(this.Render);
    }
}
