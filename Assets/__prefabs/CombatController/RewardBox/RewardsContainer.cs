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

    public void Render(List<LootedItem> loots)
    {
        this.PopulateListItems(this.RewardItemUIs, loots.Count);
        this.RenderItems(
            this.RewardItemUIs, loots, 0,
            (ui, data) => ui.Render(data));
    }
}
