using System.Collections.Generic;
using UnityEngine;

public class LandingSpot : MonoBehaviour
{
    [SerializeField]
    private List<CombatParticipant> meleeSpotOwners = new();
    [SerializeField]
    private List<CombatParticipant> midRangeSpotOwners = new();

    void Start()
    {
        meleeSpotOwners.ForEach(x => x.meleeLandingSpot = this.transform.position);
        midRangeSpotOwners.ForEach(x => x.midRangeLandingSpot = this.transform.position);
    }

    private void OnDrawGizmos()
    {
        // Set the color to orange
        Gizmos.color = new Color(1f, 0.5f, 0f); // Red = 1, Green = 0.5, Blue = 0, which is orange

        // Draw a sphere gizmo with a radius of 0.5 at the transform's position
        Gizmos.DrawSphere(transform.position, 0.5f);
    }    
}
