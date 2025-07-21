using Animancer.FSM;
using UnityEngine;

public class CombatZone : MonoBehaviour
{
    bool hasTriggered = false;
    private CombatController combatController;
    void Start()
    {
        combatController = GameObject.FindGameObjectWithTag("CombatController").GetComponent<CombatController>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
            Debug.Log("CombatZone triggered by: " + other.name);
            var state = new CombatState
            {
                Phase = CombatPhase.BattleStart
            };
            combatController.SetCombatState(state);
        }
    }
}
