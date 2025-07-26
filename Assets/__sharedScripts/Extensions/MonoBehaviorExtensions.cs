using Unity.VisualScripting;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static CombatController GetCombatController(this MonoBehaviour mono)
    {
        var obj = GameObject.FindGameObjectWithTag("CombatController");
        if (obj != null)
        {
            var comp = obj.GetComponent<CombatController>();
            if (comp != null)
            {
                return comp;
            }
        }

        return null;
    }
}