using R3;
using UnityEngine;
using UnityEngine.UI;

public class BattleSceneUI : MonoBehaviour
{
    [SerializeField]
    Button TapToStartButton;
    [SerializeField]
    CombatController CombatController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.TapToStartButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                // Hide the button when tapped
                this.TapToStartButton.gameObject.SetActive(false);
                // Start the combat phase
                var state = this.CombatController.CombatStateObs.Value;
                state.Phase = CombatPhase.Patrolling;
                this.CombatController.SetCombatState(state);
            })
            .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
