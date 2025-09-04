using System.Collections.Generic;
using UnityEngine;

public class RewardsContainer : ListContainer
{
    [SerializeField]
    List<RewardItemUI> RewardItemUIs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Render(LootSO lootSO)
    {
        this.PopulateListItems(this.RewardItemUIs, lootSO.LootedItems.Count);
        this.RenderItems(
            this.RewardItemUIs, lootSO.LootedItems, 0,
            (ui, data) => ui.Render(data));
    }
}
