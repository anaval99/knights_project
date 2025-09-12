using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardBox : MonoBehaviour
{
    [SerializeField]
    Button ClaimButton;
    [SerializeField]
    GameObject RewardBoxLayout;
    [SerializeField]
    RewardsContainer rewardsContainer;
    [SerializeField]
    MissionDataCenter missionDataCenter;

    List<LootedItem> loots;
    void Start()
    {
        this.loots = this.missionDataCenter.LootSO.LootedItems.Where(x =>
        {
            int dropRateResult = UnityEngine.Random.Range(1, 100);
            return dropRateResult <= x.DropRatePercent;
        }).ToList();
        this.RewardBoxLayout.SetActive(false);
        this.rewardsContainer.Render(this.loots);
        var controller = this.GetCombatController();
        if (controller != null)
        {
            controller.CombatStateObs
                .Where(state => state.Phase == CombatPhase.FinalBattleEnd)
                .Take(1)
                .Subscribe(_ => this.RewardBoxLayout.SetActive(true))
                .AddTo(this);

            this.ClaimButton.OnClickAsObservable().Take(1)
                .Subscribe(_ =>
                {
                    this.ClaimLoots();
                })
                .AddTo(this);
        }
    }

    public async void ClaimLoots()
    {
        var controller = this.GetCombatController();
        await controller.ClaimLoots(this.loots);
        await this.ClaimLootsForTeammates();
        SceneManager.LoadScene("__scenes/dashboard/dashboard");
    }

    public async System.Threading.Tasks.Task<int> ClaimLootsForTeammates()
    {
        var teamLoots = this.loots.Where(x => x.IncludeTeammates).ToList();
        if (teamLoots.Count == 0)
        {
            return 0;
        }
        var controller = this.GetCombatController();
        await controller.ClaimLootsForTeammates(teamLoots);
        return teamLoots.Count;
    }    
}
