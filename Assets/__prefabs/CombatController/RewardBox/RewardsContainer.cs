using System.Collections.Generic;
using System.Linq;
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
        var loots = lootSO.LootedItems.Where(x =>
        {
            int dropRateResult = UnityEngine.Random.Range(1, 100);
            return dropRateResult <= x.DropRatePercent;
        }).ToList();
        this.PopulateListItems(this.RewardItemUIs, lootSO.LootedItems.Count);
        this.RenderItems(
            this.RewardItemUIs, lootSO.LootedItems, 0,
            (ui, data) => ui.Render(data));
    }
}
