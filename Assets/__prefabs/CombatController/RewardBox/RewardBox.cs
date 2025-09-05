using R3;
using UnityEngine;

public class RewardBox : MonoBehaviour
{
    [SerializeField]
    GameObject RewardBoxLayout;
    [SerializeField]
    RewardsContainer rewardsContainer;
    [SerializeField]
    MissionDataCenter missionDataCenter;

    void Start()
    {
        this.RewardBoxLayout.SetActive(false);
        this.rewardsContainer.Render(this.missionDataCenter.LootSO);
        var controller = this.GetCombatController();
        if (controller != null)
        {
            controller.CombatStateObs
                .Where(state => state.Phase == CombatPhase.FinalBattleEnd)
                .Take(1)
                .Subscribe(_ => this.RewardBoxLayout.SetActive(true))
                .AddTo(this);
        }
    }
}
