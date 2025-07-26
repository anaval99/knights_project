using System.Collections.Generic;
using System.Linq;
using Animancer.FSM;
using UnityEngine;

public class CombatZone : MonoBehaviour
{
    bool hasTriggered = false;
    private CombatController combatController;
    private Collider selfCollider;

    void Start()
    {
        combatController = this.GetCombatController();
        selfCollider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        string otherLayer = LayerMask.LayerToName(other.gameObject.layer);
        if (!hasTriggered)
        {
            hasTriggered = true;
            Debug.Log("CombatZone triggered by: " + other.name);
            this.SetBattleStart();
        }
    }

    [ContextMenu("Set Battle Start")]
    void SetBattleStart()
    {
        var state = combatController.CombatStateObs.Value;
        state.Phase = CombatPhase.BattleStart;
        state.EnemyParticipants = GetCombatParticipants();
        combatController.SetCombatState(state);
        Debug.Log("CombatZone found " + state.EnemyParticipants + " combat participants: " + string.Join(", ", state.EnemyParticipants.Select(e => e.name)));
    }

    List<CombatParticipant> GetCombatParticipants()
    {
        // get all the <CombatParticipant> within the combat zone
        List<CombatParticipant> participants = new List<CombatParticipant>();
        Collider[] colliders = Physics.OverlapBox(this.transform.position, selfCollider.bounds.extents, Quaternion.identity);
        foreach (Collider collider in colliders)
        {
            CombatParticipant participant = collider.GetComponent<CombatParticipant>();
            if (participant != null && !participant.name.Contains("Player"))
            {
                participants.Add(participant);
            }
        }
        return participants;
    }

    void OnDrawGizmos()
    {   
        if (selfCollider == null)
        {
            selfCollider = GetComponent<Collider>();
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position, selfCollider.bounds.size);
    }    
}
