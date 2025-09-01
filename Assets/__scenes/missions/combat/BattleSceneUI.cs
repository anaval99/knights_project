using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleSceneUI : MonoBehaviour
{
    [SerializeField]
    Button TapToStartButton;
    [SerializeField]
    Button DashboardButton;
    [SerializeField]
    CombatController CombatController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CombatController.CombatStateObs
            .Subscribe(state =>
            {
                // Show the button when combat starts
                this.TapToStartButton.gameObject.SetActive(state.Phase == CombatPhase.Start);
            })
            .AddTo(this);
        this.TapToStartButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                // Hide the button when tapped
                this.TapToStartButton.gameObject.SetActive(false);
                // Start the combat phase
                var state = this.CombatController.CombatStateObs.Value.Clone();
                state.Phase = CombatPhase.Patrolling;
                this.CombatController.SetCombatState(state);
            }).AddTo(this);
        this.DashboardButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                SceneManager.LoadScene("__scenes/dashboard/dashboard");
            }).AddTo(this);
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
