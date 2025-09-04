using UnityEngine;

public class RewardBox : MonoBehaviour
{
    [SerializeField]
    RewardsContainer rewardsContainer;
    [SerializeField]
    LootSO lootSO;

    void OnEnable()
    {
        this.rewardsContainer.Render(this.lootSO);
    }
}
